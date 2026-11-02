
using Microsoft.AspNetCore.Identity;
using OurSpot.Security;

namespace OurSpot.Services
{
    public class AdministradoresService(
        UserManager<ApplicationUser> userManager)
    {
        public async Task<IdentityResult> Guardar(
            string nombre,
            string correo,
            string contrasena)
        {
            return await Insertar(nombre, correo, contrasena);
        }

        public async Task<IdentityResult> Insertar(
            string nombre,
            string correo,
            string contrasena)
        {
            nombre = nombre.Trim();
            correo = correo.Trim();

            var usuario = new ApplicationUser
            {
                NombreCompleto = nombre,
                UserName = correo,
                Email = correo
            };

            var resultado = await userManager.CreateAsync(
                usuario, contrasena);

            if (!resultado.Succeeded)
                return resultado;

            resultado = await userManager.AddToRoleAsync(
                usuario, "Administrador");

            if (!resultado.Succeeded)
            {
                var eliminacion = await userManager.DeleteAsync(usuario);

                if (!eliminacion.Succeeded)
                {
                    throw new InvalidOperationException(
                        "No se pudo asignar el rol ni revertir la creación de la cuenta.");
                }

                return resultado;
            }

            return IdentityResult.Success;
        }
    }
}
