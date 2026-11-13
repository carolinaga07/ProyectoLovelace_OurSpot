
using Microsoft.AspNetCore.Identity;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Security;

namespace OurSpot.Services;

public class RegistroClientesService(
    Contexto contexto,
    UserManager<ApplicationUser> userManager, AuditoriaService auditoriaService)
{
    public async Task<IdentityResult> Registrar(
        string nombre,
        string correo,
        string? telefono,
        string contrasena)
    {
        nombre = nombre.Trim();
        correo = correo.Trim();

        await using var transaccion =
            await contexto.Database.BeginTransactionAsync();

        var usuario = new ApplicationUser
        {
            UserName = correo,
            Email = correo,
            NombreCompleto = nombre
        };

        var resultado = await userManager.CreateAsync(
            usuario, contrasena);

        if (!resultado.Succeeded)
            return resultado;

        resultado = await userManager.AddToRoleAsync(
            usuario, "Cliente");

        if (!resultado.Succeeded)
            return resultado;

        var cliente = new Clientes
        {
            Nombre = nombre,
            Correo = correo,
            Telefono = telefono,
            ApplicationUserId = usuario.Id
        };

        contexto.Clientes.Add(cliente);

        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();

        await auditoriaService.RegistrarAsync("Clientes", "Registrar", $"Se registró un nuevo cliente: {correo}");

        return IdentityResult.Success;
    }
}
