using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models.DTOs;

namespace OurSpot.Services;

public class ClientesService(IDbContextFactory<Contexto> contextFactory, AuditoriaService auditoriaService)
{
    public async Task<PerfilClienteDto?> ObtenerPerfil(
       string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Clientes
            .AsNoTracking()
            .Where(c =>
                c.ApplicationUserId == applicationUserId)
            .Select(c => new PerfilClienteDto
            {
                Nombre = c.Nombre,
                Correo = c.Correo,
                Telefono = c.Telefono
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ActualizarPerfil(
        string applicationUserId,
        PerfilClienteDto datos)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var cliente = await contexto.Clientes
            .FirstOrDefaultAsync(c =>
                c.ApplicationUserId == applicationUserId);

        if (cliente == null ||
            string.IsNullOrWhiteSpace(datos.Nombre))
        {
            return false;
        }

        cliente.Nombre = datos.Nombre.Trim();
        cliente.Telefono = datos.Telefono?.Trim();

        var usuario = await contexto.Users
            .FirstOrDefaultAsync(u =>
                u.Id == applicationUserId);

        if (usuario != null)
        {
            usuario.NombreCompleto = cliente.Nombre;
        }

        var actualizado = await contexto.SaveChangesAsync() > 0;

        if (actualizado)
            await auditoriaService.RegistrarAsync("Clientes", "ActualizarPerfil", $"El cliente {cliente.Nombre} actualizó su perfil.");

        return actualizado;
    }

}
