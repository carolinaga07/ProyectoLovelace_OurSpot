using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;

namespace OurSpot.Services;

public class AuditoriaService(IDbContextFactory<Contexto> contextFactory, AuthenticationStateProvider authStateProvider)
{
    public async Task RegistrarAsync(string seccion, string accion, string detalle)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var emailUsuario = user.Identity?.Name ?? "Usuario Anónimo";

        var auditoria = new Auditoria
        {
            UsuarioEmail = emailUsuario,
            Seccion = seccion,
            Accion = accion,
            Detalle = detalle,
            Fecha = DateTime.Now
        };

        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Auditorias.Add(auditoria);
        await contexto.SaveChangesAsync();
    }

    public async Task<List<Auditoria>> GetListAsync()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Auditorias
            .OrderByDescending(a => a.Fecha)
            .AsNoTracking()
            .ToListAsync();
    }
}