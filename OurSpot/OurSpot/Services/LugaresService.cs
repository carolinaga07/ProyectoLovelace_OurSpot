using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;
using OurSpot.Context;
using System.Linq.Expressions;

namespace OurSpot.Services;

public class LugaresService(IDbContextFactory<Contexto> contextFactory, AuditoriaService auditoriaService) : IService<Lugares, int>
{
    public async Task<bool> Guardar(Lugares lugar)
    {
        if (!await Existe(lugar.Id))
        {
            return await Insertar(lugar);
        }
        else
        {
            return await Modificar(lugar);
        }
    }

    public async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Lugares
            .AnyAsync(l => l.Id == id);
    }

    public async Task<bool> Insertar(Lugares lugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        // Se conserva el nombre ingresado al registrar el lugar.
        lugar.Nombre = lugar.Nombre.Trim();

        lugar.Estado = "Disponible";
        lugar.Eliminado = false;

        contexto.Lugares.Add(lugar);
        var guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
            await auditoriaService.RegistrarAsync("Lugares", "Insertar", $"Se creó el lugar: {lugar.Nombre}");

        return guardado;
    }

    public async Task<bool> Modificar(Lugares lugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        lugar.Nombre = lugar.Nombre.Trim();

        contexto.Update(lugar);
        var guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
            await auditoriaService.RegistrarAsync("Lugares", "Modificar", $"Se modificó el lugar: {lugar.Nombre}");

        return guardado;
    }

    public async Task<Lugares?> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Lugares
            .FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado);
    }

    public async Task<List<Lugares>> GetList(Expression<Func<Lugares, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Lugares
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int idLugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lugar = await contexto.Lugares
            .FirstOrDefaultAsync(l => l.Id == idLugar && !l.Eliminado);

        if (lugar == null)
            return false;

        lugar.Eliminado = true;

        var eliminado = await contexto.SaveChangesAsync() > 0;

        if (eliminado)
            await auditoriaService.RegistrarAsync("Lugares", "Eliminar", $"Se eliminó el lugar: {lugar.Nombre}");

        return eliminado;
    }

    public async Task<bool> Recuperar(int idLugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lugar = await contexto.Lugares
            .FirstOrDefaultAsync(l => l.Id == idLugar && l.Eliminado);

        if (lugar == null)
            return false;

        lugar.Eliminado = false;

        var recuperado = await contexto.SaveChangesAsync() > 0;

        if (recuperado)
            await auditoriaService.RegistrarAsync("Lugares", "Recuperar", $"Se recuperó el lugar: {lugar.Nombre}");

        return recuperado;
    }
}
