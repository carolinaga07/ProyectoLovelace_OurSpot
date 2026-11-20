
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Security;

namespace OurSpot.Services;

public class RegistroProveedoresService(
    Contexto contexto,
    UserManager<ApplicationUser> userManager,
    AuditoriaService auditoriaService)
{
    public async Task<IdentityResult> Registrar(
        int proveedorId,
        string correo,
        string contrasena)
    {
        correo = correo.Trim();

        var proveedor = await contexto.Proveedores
            .FirstOrDefaultAsync(p =>
                p.Id == proveedorId && !p.Eliminado);

        if (proveedor is null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El proveedor no existe."
                });
        }

        if (!string.IsNullOrEmpty(proveedor.ApplicationUserId))
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El proveedor ya tiene una cuenta."
                });
        }

        var usuarioExistente =
            await userManager.FindByEmailAsync(correo);

        if (usuarioExistente is not null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "El correo ya está registrado."
                });
        }

        await using var transaccion =
            await contexto.Database.BeginTransactionAsync();

        var usuario = new ApplicationUser
        {
            UserName = correo,
            Email = correo,
            NombreCompleto = proveedor.Nombre
        };

        var resultado = await userManager.CreateAsync(
            usuario, contrasena);

        if (!resultado.Succeeded)
            return resultado;

        resultado = await userManager.AddToRoleAsync(
            usuario, "Proveedor");

        if (!resultado.Succeeded)
            return resultado;

        proveedor.ApplicationUserId = usuario.Id;

        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();

        await auditoriaService.RegistrarAsync(
            "Proveedores",
            "RegistrarCuenta",
            $"Se creó una cuenta para el proveedor ID: {proveedorId}");

        return IdentityResult.Success;
    }
}
