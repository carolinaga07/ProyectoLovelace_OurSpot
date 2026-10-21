using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using System.Linq.Expressions;

namespace OurSpot.Services
{
    public class ContratacionesService(IDbContextFactory<Contexto> contextFactory) : IService<Contrataciones, int>
    {
        public async Task<bool> Guardar(Contrataciones contratacion)
        {
            if (!await Existe(contratacion.Id))
            {
                return await Insertar(contratacion);
            }
            else
            {
                return await Modificar(contratacion);
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

            var contratacion = await contexto.Contrataciones.FirstOrDefaultAsync(c => c.Id == idContratacion && !c.Eliminado);

            if (contratacion == null)
                return false;

            contratacion.Eliminado = true;

            return await contexto.SaveChangesAsync() > 0;
        }
    }
}
