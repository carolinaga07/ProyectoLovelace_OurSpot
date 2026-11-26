using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Security;
using System.Linq.Expressions;

namespace OurSpot.Services;

public class PersonalService(
    IDbContextFactory<Contexto> contextFactory,
    AuditoriaService auditoriaService,
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager) : Aplicada1.Core.IService<Personal, int>
{
    public async Task<bool> Guardar(Personal personal)
    {
        if (!await Existe(personal.Id))
        {
            return await Insertar(personal);
        }
        else
        {
            return await Modificar(personal);
        }
    }

    public async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Personales.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> Insertar(Personal personal)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Personales.Add(personal);

        var guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
        {
            await auditoriaService.RegistrarAsync("Personal", "Insertar", $"Se creó el personal: {personal.Nombre}");
        }

        return guardado;
    }

    public async Task<bool> Modificar(Personal personal)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(personal);

        var guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
        {
            await auditoriaService.RegistrarAsync("Personal", "Modificar", $"Se actualizó el personal: {personal.Nombre}");
        }

        return guardado;
    }

    public async Task<Personal?> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Personales.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        var usuarioActual = httpContextAccessor.HttpContext?.User;

        if (usuarioActual?.Identity?.IsAuthenticated != true ||
            !usuarioActual.IsInRole("Administrador"))
        {
            return false;
        }

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var personal = await contexto.Personales.FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        if (personal == null)
            return false;

        var usuarioActualId = userManager.GetUserId(usuarioActual);
        var correoActual = usuarioActual.Identity?.Name;

        bool esMismaCuenta =
            (!string.IsNullOrEmpty(personal.ApplicationUserId) && personal.ApplicationUserId == usuarioActualId) ||
            (!string.IsNullOrEmpty(correoActual) && string.Equals(personal.Correo, correoActual, StringComparison.OrdinalIgnoreCase));

        if (esMismaCuenta)
            return false;

        bool esAdministrador =
            string.Equals(personal.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);

        if (!esAdministrador && !string.IsNullOrEmpty(personal.ApplicationUserId))
        {
            var usuarioObjetivo = await userManager.FindByIdAsync(personal.ApplicationUserId);
            if (usuarioObjetivo != null)
            {
                esAdministrador = await userManager.IsInRoleAsync(usuarioObjetivo, "Administrador");
            }
        }

        if (esAdministrador)
            return false;

        personal.Eliminado = true;

        var eliminado = await contexto.SaveChangesAsync() > 0;

        if (eliminado)
        {
            await auditoriaService.RegistrarAsync("Personal", "Eliminar", $"Se eliminó el personal: {personal.Nombre}");
        }

        return eliminado;
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

    public async Task<bool> Recuperar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var personal = await contexto.Personales.FirstOrDefaultAsync(c => c.Id == id && c.Eliminado);

        if (personal == null)
            return false;

        personal.Eliminado = false;

        var recuperado = await contexto.SaveChangesAsync() > 0;

        if (recuperado)
        {
            await auditoriaService.RegistrarAsync("Personal", "Recuperar", $"Se recuperó el personal: {personal.Nombre}");
        }

        return recuperado;
    }
}
