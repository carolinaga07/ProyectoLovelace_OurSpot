
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Security;

namespace OurSpot.Services
{
    public class RegistroPersonalService(
        Contexto contexto,
        UserManager<ApplicationUser> userManager)
    {
        public async Task<IdentityResult> Registrar(
            string nombre,
            string correo,
            string contrasena,
            string cargo,
            string rol,
            string estado)
        {
            nombre = nombre.Trim();
            correo = correo.Trim();

            if (rol != "Administrador" && rol != "Empleado")
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RolInvalido",
                    Description = "El rol seleccionado no es válido."
                });
            }

            if (await ExisteCorreo(correo))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CorreoDuplicado",
                    Description = "El correo ya está registrado."
                });
            }

            await using var transaccion =
                await contexto.Database.BeginTransactionAsync();

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
                usuario, rol);

            if (!resultado.Succeeded)
                return resultado;

            var personal = new Personal
            {
                Nombre = nombre,
                Correo = correo,
                Cargo = cargo,
                Rol = rol,
                Estado = estado,
                Usuario = correo,
                Contrasena = string.Empty,
                ApplicationUserId = usuario.Id
            };

            contexto.Personales.Add(personal);

            await contexto.SaveChangesAsync();
            await transaccion.CommitAsync();

            return IdentityResult.Success;
        }

        public async Task<bool> ExisteCorreo(string correo)
        {
            return await contexto.Users
                .AnyAsync(u => u.Email == correo);
        }
    }
}

