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
            if (contratacion.ProveedorId is null)
                return false;

            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            var proveedor = await contexto.Proveedores
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == contratacion.ProveedorId.Value &&
                    !p.Eliminado);

            if (proveedor is null)
                return false;

            // El servidor determina el nombre correcto.
            contratacion.Proveedor = proveedor.Nombre;

            if (!await Existe(contratacion.ContratacionId))
            {
                return await Insertar(contratacion);
            }

            return await Modificar(contratacion);
        }


        public async Task<bool> Existe(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Contrataciones
                .AnyAsync(c => c.ContratacionId == id);
        }


        public async Task<bool> Insertar(Contrataciones contratacion)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            // Las contrataciones creadas por el administrador
            // comienzan activas.
            contratacion.Estado = "Activa";

            contexto.Contrataciones.Add(contratacion);

            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
            {
                await auditoriaService.RegistrarAsync(
                    "Contrataciones",
                    "Insertar",
                    $"Se creó la contratación de servicio: {contratacion.Servicio}"
                );
            }

            return guardado;
        }

        public async Task<bool> Modificar(Contrataciones contratacion)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Update(contratacion);
            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
                await auditoriaService.RegistrarAsync("Contrataciones", "Modificar", $"Se modificó la contratación: {contratacion.Servicio}");

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
                await auditoriaService.RegistrarAsync("Contrataciones", "Eliminar", $"Se eliminó la contratación: {contratacion.Servicio}");

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
                await auditoriaService.RegistrarAsync("Contrataciones", "Recuperar", $"Se recuperó la contratación: {contratacion.Servicio}");

            return recuperado;
        }
    }
}
