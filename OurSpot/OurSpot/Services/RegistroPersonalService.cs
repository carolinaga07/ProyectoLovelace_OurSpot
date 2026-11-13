
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Security;

namespace OurSpot.Services;

public class RegistroPersonalService(
    Contexto contexto,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor, AuditoriaService auditoriaService)
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
            Email = correo,
            LockoutEnabled = true
        };

        var resultado = await userManager.CreateAsync(
            usuario, contrasena);

        if (!resultado.Succeeded)
            return resultado;

        resultado = await userManager.AddToRoleAsync(
            usuario, rol);

        if (!resultado.Succeeded)
            return resultado;

        if (estado == "Inactivo")
        {
            var resultadoBloqueo = await userManager.SetLockoutEndDateAsync(
                usuario, DateTimeOffset.MaxValue);

            if (!resultadoBloqueo.Succeeded)
                return resultadoBloqueo;
        }

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

        await auditoriaService.RegistrarAsync("RegistroPersonal", "Registrar", $"Se registró el personal con rol {rol}: {nombre}");

        return IdentityResult.Success;
    }

    public async Task<bool> ExisteCorreo(string correo)
    {
        return await contexto.Users
            .AnyAsync(u => u.Email == correo);
    }


    public async Task<IdentityResult> Modificar(
        int personalId,
        string nombre,
        string correo,
        string cargo,
        string rol,
        string estado)
    {
        nombre = nombre.Trim();
        correo = correo.Trim();
        cargo = cargo.Trim();

        if (rol != "Administrador" && rol != "Empleado")
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "El rol seleccionado no es válido."
            });
        }

        if (estado != "Activo" && estado != "Inactivo")
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "El estado seleccionado no es válido."
            });
        }

        await using var transaccion =
            await contexto.Database.BeginTransactionAsync();

        var personal = await contexto.Personales
            .FirstOrDefaultAsync(p => p.Id == personalId);

        if (personal == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No se encontró el personal."
            });
        }

        if (string.IsNullOrEmpty(personal.ApplicationUserId))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Este personal no tiene una cuenta de Identity vinculada."
            });
        }

        var correoExistente = await contexto.Personales
            .AnyAsync(p => p.Correo == correo && p.Id != personalId);

        if (correoExistente)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Ya existe personal con ese correo."
            });
        }

        var usuario = await userManager.FindByIdAsync(
            personal.ApplicationUserId);

        if (usuario == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No se encontró la cuenta de Identity."
            });
        }

        // INICIO: Proteger al último administrador activo

        var rolesActuales = await userManager.GetRolesAsync(usuario);

        bool esAdministrador = rolesActuales.Contains("Administrador");

        bool pierdeAccesoAdministrativo =
            rol != "Administrador" || estado == "Inactivo";

        if (esAdministrador && pierdeAccesoAdministrativo)
        {
            var administradores = await userManager
                .GetUsersInRoleAsync("Administrador");

            int administradoresActivos = 0;

            foreach (var admin in administradores)
            {
                if (!await userManager.IsLockedOutAsync(admin))
                {
                    administradoresActivos++;
                }
            }

            if (administradoresActivos <= 1)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description =
                        "No puedes desactivar ni quitar el rol al último administrador activo."
                });
            }
        }

        // FIN: Proteger al último administrador activo

        var usuarioConCorreo = await userManager.FindByEmailAsync(correo);

        if (usuarioConCorreo != null &&
            usuarioConCorreo.Id != usuario.Id)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "El correo ya pertenece a otra cuenta."
            });
        }

        usuario.NombreCompleto = nombre;

        var resultado = await userManager.SetEmailAsync(usuario, correo);

        if (!resultado.Succeeded)
            return resultado;

        resultado = await userManager.SetUserNameAsync(usuario, correo);

        if (!resultado.Succeeded)
            return resultado;


        if (!rolesActuales.Contains(rol))
        {
            var rolesPersonal = rolesActuales
                .Where(r => r == "Administrador" || r == "Empleado")
                .ToArray();

            if (rolesPersonal.Length > 0)
            {
                resultado = await userManager.RemoveFromRolesAsync(
                    usuario, rolesPersonal);

                if (!resultado.Succeeded)
                    return resultado;
            }

            resultado = await userManager.AddToRoleAsync(usuario, rol);

            if (!resultado.Succeeded)
                return resultado;
        }

        bool fueDesactivado =
            personal.Estado ==  "Activo" && estado == "Inactivo";
        // Sincronizar los datos laborales.
        personal.Nombre = nombre;
        personal.Correo = correo;
        personal.Usuario = correo;
        personal.Cargo = cargo;
        personal.Rol = rol;
        personal.Estado = estado;

        // Habilitar el mecanismo de bloqueo de Identity.
        var resultadoBloqueo = await userManager.SetLockoutEnabledAsync(
            usuario, true);

        if (!resultadoBloqueo.Succeeded)
            return resultadoBloqueo;

        if (estado == "Inactivo")
        {
            // Bloquear la cuenta hasta la fecha máxima permitida.
            resultadoBloqueo = await userManager.SetLockoutEndDateAsync(
                usuario, DateTimeOffset.MaxValue);
        }
        else
        {
            // Quitar el bloqueo y permitir nuevamente el acceso.
            resultadoBloqueo = await userManager.SetLockoutEndDateAsync(
                usuario, null);
        }

        if (!resultadoBloqueo.Succeeded)
            return resultadoBloqueo;

        // El campo Contrasena heredado no se modifica.

        // Invalidar sesiones anteriores cuando se desactiva una cuenta.
        if (fueDesactivado)
        {
            var resultadoSeguridad =
                await userManager.UpdateSecurityStampAsync(usuario);

            if (!resultadoSeguridad.Succeeded)
                return resultadoSeguridad;
        }

        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();

        await auditoriaService.RegistrarAsync("RegistroPersonal", "Modificar", $"Se modificó el registro de personal ID: {personalId}");

        return IdentityResult.Success;
    }


    public async Task<IdentityResult> Eliminar(int personalId)
    {
        // Comprobar que la operación la realiza un administrador.
        var usuarioActual = httpContextAccessor.HttpContext?.User;

        if (usuarioActual?.Identity?.IsAuthenticated != true ||
            !usuarioActual.IsInRole("Administrador"))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No tienes permisos para eliminar personal."
            });
        }

        await using var transaccion =
            await contexto.Database.BeginTransactionAsync();

        var personal = await contexto.Personales
            .FirstOrDefaultAsync(p => p.Id == personalId);

        if (personal == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No se encontró el personal."
            });
        }

        if (string.IsNullOrEmpty(personal.ApplicationUserId))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description =
                    "Este personal no tiene una cuenta de Identity vinculada."
            });
        }

        var usuario = await userManager.FindByIdAsync(
            personal.ApplicationUserId);

        if (usuario == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No se encontró la cuenta de Identity."
            });
        }

        // Evitar que el administrador elimine su propia cuenta.
        var usuarioActualId = userManager.GetUserId(usuarioActual);

        if (usuario.Id == usuarioActualId)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "No puedes eliminar tu propia cuenta."
            });
        }

        // Proteger al último administrador activo.
        if (await userManager.IsInRoleAsync(usuario, "Administrador"))
        {
            var administradores = await userManager
                .GetUsersInRoleAsync("Administrador");

            int administradoresActivos = 0;

            foreach (var admin in administradores)
            {
                if (!await userManager.IsLockedOutAsync(admin))
                {
                    administradoresActivos++;
                }
            }

            if (administradoresActivos <= 1)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description =
                        "No puedes eliminar al último administrador activo."
                });
            }
        }

        // Eliminar primero el registro de Personal.
        contexto.Personales.Remove(personal);
        await contexto.SaveChangesAsync();

        // Eliminar la cuenta de Identity.
        var resultado = await userManager.DeleteAsync(usuario);

        if (!resultado.Succeeded)
            return resultado;

        await transaccion.CommitAsync();

        await auditoriaService.RegistrarAsync("RegistroPersonal", "Eliminar", $"Se eliminó el registro de personal ID: {personalId}");

        return IdentityResult.Success;
    }


}

