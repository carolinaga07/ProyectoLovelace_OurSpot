using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;
using OurSpot.Context;
using System.Linq.Expressions;

namespace OurSpot.Services;

public class LugaresService(IDbContextFactory<Contexto> contextFactory) : IService<Lugares, int>
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

        lugar.Estado = "Disponible";
        lugar.Eliminado = false;

        contexto.Lugares.Add(lugar);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Lugares lugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(lugar);
        return await contexto.SaveChangesAsync() > 0;
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
            .Where(l => !l.Eliminado)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int idLugar)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lugar = await contexto.Lugares.FirstOrDefaultAsync(l => l.Id == idLugar && !l.Eliminado);

        if (lugar == null)
            return false;

        lugar.Eliminado = true;

        return await contexto.SaveChangesAsync() > 0;
    }
}
