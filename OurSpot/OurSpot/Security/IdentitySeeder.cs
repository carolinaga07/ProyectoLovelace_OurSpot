using Microsoft.AspNetCore.Identity;
using OurSpot.Security;

namespace OurSpot.Security;

public static class IdentitySeeder
{
    public static async Task CrearRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        {
            "Administrador",
            "Empleado",
            "Cliente"
        };

        foreach(var rol in roles)
        {
            if(!await roleManager.RoleExistsAsync(rol))
            {
                var resultado = await roleManager.CreateAsync(
                    new IdentityRole(rol));

                if (!resultado.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", resultado.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    public static async Task CrearAdministradorAsync(IServiceProvider services, string correo, string contrasena)
    {
        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var administrador = await userManager.FindByEmailAsync(correo);

        if(administrador == null)
        {
            administrador = new ApplicationUser
            {
                UserName = correo,
                Email = correo,
                NombreCompleto = "Administrador General",
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(administrador, contrasena);

            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ",
                    resultado.Errors.Select(e => e.Description)));
            }
        }

        if(!await userManager.IsInRoleAsync(administrador, "Administrador"))
        {
            var resultado = await userManager.AddToRoleAsync(administrador, "Administrador");

            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", 
                    resultado.Errors.Select(e => e.Description)));
            }
        }
    }
}
