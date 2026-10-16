using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;
using OurSpot.Context;
using System.Linq.Expressions;

namespace OurSpot.Services
{
    public class ContratacionesService(IDbContextFactory<Contexto> contextFactory) : IService<ContratacionesService, int>
    {
        public async Task<bool> Guardar(Autores autor)
        {
            if (!await Existe(autor.IdAutor))
            {
                return await Insertar(autor);
            }
            else
            {
                return await Modificar(autor);
            }
        }

        public async Task<bool> Existe(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .AnyAsync(c => c.Id== id);
        }

        public async Task<bool> Insertar(Contrataciones contratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Contrataciones.Add(contratacion);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Modificar(Contrataciones contratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Update(contratacion);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Contrataciones?> Buscar(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contrataciones>> GetList(Expression<Func<Contrataciones, bool>> criterio)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Eliminar(int idContratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .Where(c => c.Id == idContratacion)
                .ExecuteDeleteAsync() > 0;
        }
    }
}
