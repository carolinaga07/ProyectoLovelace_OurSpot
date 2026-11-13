using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;
using OurSpot.Context;
using System.Linq.Expressions;

namespace OurSpot.Services
{
    public class ProveedorService(IDbContextFactory<Contexto> contextFactory, AuditoriaService auditoriaService) : IService<Proveedores, int>
    {
        public async Task<bool> Guardar(Proveedores proveedor)
        {
            if (!await Existe(proveedor.Id))
            {
                return await Insertar(proveedor);
            }
            else
            {
                return await Modificar(proveedor);
            }
        }

        public async Task<bool> Existe(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Proveedores
                .AnyAsync(c => c.Id == id);
        }

        public async Task<bool> Insertar(Proveedores proveedor)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Proveedores.Add(proveedor);
            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
                await auditoriaService.RegistrarAsync("Proveedores", "Insertar", $"Se creó el proveedor ID: {proveedor.Id}");

            return guardado;
        }

        public async Task<bool> Modificar(Proveedores proveedor)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Update(proveedor);
            var guardado = await contexto.SaveChangesAsync() > 0;

            if (guardado)
                await auditoriaService.RegistrarAsync("Proveedores", "Modificar", $"Se modificó el proveedor ID: {proveedor.Id}");

            return guardado;
        }

        public async Task<Proveedores?> Buscar(int id)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Proveedores
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Proveedores>> GetList(Expression<Func<Proveedores, bool>> criterio)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Proveedores
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Eliminar(int idProveedor)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();

            var proveedor = await contexto.Proveedores.FirstOrDefaultAsync(c => c.Id == idProveedor && !c.Eliminado);

            if (proveedor == null)
                return false;

            proveedor.Eliminado = true;

            var eliminado = await contexto.SaveChangesAsync() > 0;

            if (eliminado)
                await auditoriaService.RegistrarAsync("Proveedores", "Eliminar", $"Se eliminó el proveedor ID: {idProveedor}");

            return eliminado;
        }
    }
}
