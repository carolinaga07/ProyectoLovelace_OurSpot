using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using System.Linq.Expressions;

namespace OurSpot.Services;

    public class PersonalService(IDbContextFactory<Contexto> contextFactory) : Aplicada1.Core.IService<Personal, int>
    {
        public async Task<bool> Guardar(Personal personal)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();

            if (personal.Id == 0)
            {
                contexto.Personales.Add(personal);
            }
            else
            {
                contexto.Update(personal);
            }
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Personal?> Buscar(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Personales.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            var personal = await contexto.Personales.FindAsync(id);
            if (personal == null)
            {
                return false;
            }
            contexto.Personales.Remove(personal);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<List<Personal>> GetList(Expression<Func<Personal, bool>> criterio)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Personales.Where(criterio).AsNoTracking().ToListAsync();
        }

        public async Task<bool> ExisteCorreo(string correo, int id = 0)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Personales.AnyAsync(p => p.Correo == correo && p.Id != id);
        }
    }
