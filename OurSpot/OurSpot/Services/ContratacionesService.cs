using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using System.Linq.Expressions;

namespace OurSpot.Services
{
    public class ContratacionesService(IDbContextFactory<Contexto> contextFactory, AuditoriaService auditoriaService) : IService<Contrataciones, int>
    {
        public async Task<bool> Guardar(Contrataciones contratacion)
        {
            if (!await Existe(contratacion.ContratacionId))
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
                .AnyAsync(c => c.ContratacionId == id);
        }

        public async Task<bool> Insertar(Contrataciones contratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Contrataciones.Add(contratacion);
            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
                await auditoriaService.RegistrarAsync("Contrataciones", "Insertar", $"Se creó la contratación de servicio: {contratacion.Servicio}");

            return guardado;
        }

        public async Task<bool> Modificar(Contrataciones contratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Update(contratacion);
            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
                await auditoriaService.RegistrarAsync("Contrataciones", "Modificar", $"Se modificó la contratación ID: {contratacion.ContratacionId}");

            return guardado;
        }

        public async Task<Contrataciones?> Buscar(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .FirstOrDefaultAsync(c => c.ContratacionId == id);
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

            var contratacion = await contexto.Contrataciones.FirstOrDefaultAsync(c => c.ContratacionId == idContratacion && !c.Eliminado);

            if (contratacion == null)
                return false;

            contratacion.Eliminado = true;

            var eliminado = await contexto.SaveChangesAsync() > 0;

            if (eliminado)
                await auditoriaService.RegistrarAsync("Contrataciones", "Eliminar", $"Se eliminó la contratación ID: {idContratacion}");

            return eliminado;
        }

        public async Task<bool> Recuperar(int idContratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();

            var contratacion = await contexto.Contrataciones.FirstOrDefaultAsync(c => c.ContratacionId == idContratacion && c.Eliminado);

            if (contratacion == null)
                return false;

            contratacion.Eliminado = false;

            var recuperado = await contexto.SaveChangesAsync() > 0;

            if (recuperado)
                await auditoriaService.RegistrarAsync("Contrataciones", "Recuperar", $"Se recuperó la contratación ID: {idContratacion}");

            return recuperado;
        }
    }
}
