using Microsoft.AspNetCore.Identity;
using OurSpot.Models.DTOs;
using OurSpot.Security;

namespace OurSpot.Services;

public class ContrasenasService(UserManager<ApplicationUser> userManager)
{
    public async Task<IdentityResult> CambiarContrasena(
        string applicationUserId,
        CambiarContrasenaDto datos)
    {
        var usuario = await userManager.FindByIdAsync(
            applicationUserId);

        if (usuario == null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Description = "No se encontró el usuario."
                });
        }

        return await userManager.ChangePasswordAsync(
            usuario,
            datos.ContrasenaActual,
            datos.NuevaContrasena);
    }
}
