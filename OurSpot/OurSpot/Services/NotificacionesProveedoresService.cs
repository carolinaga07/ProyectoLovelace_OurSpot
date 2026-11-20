
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;

namespace OurSpot.Services;

public class NotificacionesProveedoresService(
    IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<NotificacionProveedor>> ObtenerNoLeidas()
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.NotificacionesProveedores
            .AsNoTracking()
            .Where(n => !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

    public async Task<bool> MarcarComoLeida(int notificacionId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var notificacion = await contexto.NotificacionesProveedores
            .FirstOrDefaultAsync(n =>
                n.NotificacionProveedorId == notificacionId &&
                !n.Leida);

        if (notificacion is null)
            return false;

        notificacion.Leida = true;
        notificacion.FechaLectura = DateTime.UtcNow;

        return await contexto.SaveChangesAsync() > 0;
    }
}
