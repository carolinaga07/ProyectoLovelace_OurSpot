
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OurSpot.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OurSpot.Security;

public class JwtTokenService(
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager)
{
    public async Task<string> GenerarTokenAdministrador(
        ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true ||
            !principal.IsInRole("Administrador"))
        {
            throw new UnauthorizedAccessException(
                "El usuario no es administrador.");
        }

        var usuario = await userManager.GetUserAsync(principal);

        if (usuario is null ||
            !await userManager.IsInRoleAsync(usuario, "Administrador"))
        {
            throw new UnauthorizedAccessException(
                "No se encontró un administrador válido.");
        }

        var clave = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(clave) ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "La configuración JWT está incompleta.");
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(clave));

        var credenciales = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Name, usuario.UserName ?? usuario.Id),
            new(ClaimTypes.Role, "Administrador")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
