using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Models.DTOs;

namespace OurSpot.Services;

public class EventoImagenesService(
    IDbContextFactory<Contexto> contextFactory,
    IWebHostEnvironment environment)
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private const int MaxImagenes = 20;

    private string CarpetaImagenes =>
        Path.Combine(environment.ContentRootPath,
            "ArchivosPrivados", "Eventos");

    public async Task<List<EventoImagenDto>> ObtenerImagenes(
        int eventoId,
        string usuarioId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.EventoImagenes
            .AsNoTracking()
            .Where(i =>
                i.EventoId == eventoId &&
                !i.Evento.Eliminado &&
                i.Evento.Clientes != null &&
                i.Evento.Clientes.ApplicationUserId == usuarioId)
            .OrderBy(i => i.FechaSubida)
            .Select(i => new EventoImagenDto
            {
                EventoImagenId = i.EventoImagenId,
                RutaImagen = i.RutaImagen,
                FechaSubida = i.FechaSubida
            })
            .ToListAsync();
    }

    public async Task<(bool Exito, string Mensaje)> SubirImagen(
        int eventoId,
        string usuarioId,
        IBrowserFile archivo)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == eventoId &&
                !e.Eliminado &&
                e.Estado == "Finalizado" &&
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == usuarioId);

        if (evento == null)
            return (false, "No tienes permiso para subir imágenes.");

        if (archivo.Size == 0 || archivo.Size > MaxBytes)
            return (false, "La imagen debe pesar entre 1 byte y 5 MB.");

        var cantidad = await contexto.EventoImagenes
            .CountAsync(i => i.EventoId == eventoId);

        if (cantidad >= MaxImagenes)
            return (false, "El evento ya tiene 20 fotografías.");

        byte[] contenido;

        try
        {
            await using var origen = archivo.OpenReadStream(MaxBytes);
            using var memoria = new MemoryStream();
            await origen.CopyToAsync(memoria);
            contenido = memoria.ToArray();
        }
        catch (IOException)
        {
            return (false, "No se pudo leer el archivo.");
        }

        // Identificar el formato por la firma del archivo.
        string? extension = DetectarExtension(contenido);

        if (extension == null)
            return (false, "Solo se permiten imágenes JPG, PNG o WebP.");

        Directory.CreateDirectory(CarpetaImagenes);

        var nombre = $"{Guid.NewGuid():N}{extension}";
        var rutaFisica = Path.Combine(CarpetaImagenes, nombre);

        await File.WriteAllBytesAsync(rutaFisica, contenido);

        var imagen = new EventoImagen
        {
            EventoId = eventoId,
            RutaImagen = nombre,
            FechaSubida = DateTime.UtcNow
        };

        try
        {
            contexto.EventoImagenes.Add(imagen);
            await contexto.SaveChangesAsync();

            return (true, "Imagen subida correctamente.");
        }
        catch
        {
            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);

            throw;
        }
    }

    public async Task<bool> EliminarImagen(
        int imagenId,
        string usuarioId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var imagen = await contexto.EventoImagenes
            .FirstOrDefaultAsync(i =>
                i.EventoImagenId == imagenId &&
                !i.Evento.Eliminado &&
                i.Evento.Estado == "Finalizado" &&
                i.Evento.Clientes != null &&
                i.Evento.Clientes.ApplicationUserId == usuarioId);

        if (imagen == null)
            return false;

        contexto.EventoImagenes.Remove(imagen);
        await contexto.SaveChangesAsync();

        var rutaFisica = Path.Combine(
            CarpetaImagenes,
            Path.GetFileName(imagen.RutaImagen));

        if (File.Exists(rutaFisica))
            File.Delete(rutaFisica);

        return true;
    }

    private static string? DetectarExtension(byte[] datos)
    {
        if (datos.Length >= 3 &&
            datos[0] == 0xFF &&
            datos[1] == 0xD8 &&
            datos[2] == 0xFF)
            return ".jpg";

        if (datos.Length >= 8 &&
            datos.AsSpan(0, 8).SequenceEqual(
                new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
            return ".png";

        if (datos.Length >= 12 &&
            datos.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
            datos.AsSpan(8, 4).SequenceEqual("WEBP"u8))
            return ".webp";

        return null;
    }
}
