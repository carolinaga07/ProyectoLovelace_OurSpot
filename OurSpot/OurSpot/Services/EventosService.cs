
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Services;
using System.Linq.Expressions;

namespace OurSpot.Components.Pages.AdministradorPages.EventosPages.Services;

public class EventosService(
    IDbContextFactory<Contexto> contextFactory,
    NotificacionesApiService notificacionesApi)
    : IService<Evento, int>
{
    private static string ObtenerEstadoCorrespondiente(
        Evento evento,
        DateTime ahora)
    {
        var inicio = evento.FechaInicio.ToDateTime(evento.HoraInicio);
        var cierre = evento.FechaCierre.ToDateTime(evento.HoraCierre);

        if (evento.Estado == "Pendiente" && ahora >= inicio)
            return "Activo";

        if (evento.Estado == "Activo" && ahora >= cierre)
            return "Finalizado";

        return evento.Estado;
    }

    public async Task<bool> Guardar(Evento eventos)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var inicio = eventos.FechaInicio.ToDateTime(eventos.HoraInicio);
        var cierre = eventos.FechaCierre.ToDateTime(eventos.HoraCierre);

        if (cierre < inicio)
            return false;

        if (eventos.EventoId == 0)
        {
            // Los eventos nuevos comienzan pendientes.
            eventos.Estado = "Pendiente";
            contexto.Eventos.Add(eventos);
        }
        else
        {
            // Conservar el estado hasta su confirmación.
            var estadoActual = await contexto.Eventos
                .AsNoTracking()
                .Where(e => e.EventoId == eventos.EventoId)
                .Select(e => e.Estado)
                .FirstOrDefaultAsync();

            if (estadoActual == null)
                return false;

            eventos.Estado = estadoActual;
            contexto.Update(eventos);
        }

        bool guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
            await RevisarEstadosPendientes();

        return guardado;
    }

    public async Task<Evento?> Buscar(int eventoId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .Include(e => e.EventosDetalles)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventoId == eventoId);
    }

    public async Task<List<Evento>> GetList(
        Expression<Func<Evento, bool>> criterio)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RevisarEstadosPendientes()
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var eventos = await contexto.Eventos
            .Where(e => !e.Eliminado && e.Estado != "Cancelado")
            .ToListAsync();

        var ahora = DateTime.Now;

        foreach (var evento in eventos)
        {
            string estadoNuevo =
                ObtenerEstadoCorrespondiente(evento, ahora);

            // Invalidar notificaciones que ya no corresponden
            // al estado actual del evento.
            var notificacionesInvalidas =
                await contexto.NotificacionesEventos
                    .Where(n =>
                        n.EventoId == evento.EventoId &&
                        !n.Confirmada &&
                        (
                            n.EstadoAnterior != evento.Estado ||
                            n.EstadoNuevo != estadoNuevo
                        ))
                    .ToListAsync();

            foreach (var notificacionInvalida
                     in notificacionesInvalidas)
            {
                notificacionInvalida.Confirmada = true;
                notificacionInvalida.FechaConfirmacion = ahora;
            }

            if (notificacionesInvalidas.Count > 0)
                await contexto.SaveChangesAsync();

            // Si el estado no necesita cambiar, continuar.
            if (evento.Estado == estadoNuevo)
                continue;

            // Evitar notificaciones duplicadas.
            bool existePendiente =
                await contexto.NotificacionesEventos
                    .AnyAsync(n =>
                        n.EventoId == evento.EventoId &&
                        !n.Confirmada);

            if (existePendiente)
                continue;

            // Crear una notificación pendiente.
            var notificacion = new NotificacionEvento
            {
                EventoId = evento.EventoId,
                NombreEvento = evento.Nombre,
                EstadoAnterior = evento.Estado,
                EstadoNuevo = estadoNuevo,
                Fecha = ahora,
                Confirmada = false
            };

            contexto.NotificacionesEventos.Add(notificacion);

            await contexto.SaveChangesAsync();

            // Avisar mediante SignalR sin cambiar el estado.
            try
            {
                await notificacionesApi.NotificarCambioEstado(
                    evento.EventoId,
                    evento.Nombre,
                    evento.Estado,
                    estadoNuevo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR AL CONECTAR CON OURSPOT.API: {ex}");
            }
        }
    }

    public async Task<bool> ConfirmarCambioEstado(int notificacionId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var notificacion = await contexto.NotificacionesEventos
            .FirstOrDefaultAsync(n =>
                n.NotificacionId == notificacionId &&
                !n.Confirmada);

        if (notificacion == null)
            return false;

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == notificacion.EventoId &&
                !e.Eliminado);

        if (evento == null ||
            evento.Estado != notificacion.EstadoAnterior)
            return false;

        // Validar que la transición sea correcta.
        bool transicionValida =
            (evento.Estado == "Pendiente" &&
             notificacion.EstadoNuevo == "Activo") ||
            (evento.Estado == "Activo" &&
             notificacion.EstadoNuevo == "Finalizado");

        if (!transicionValida)
            return false;

        // Confirmar el cambio.
        evento.Estado = notificacion.EstadoNuevo;

        notificacion.Confirmada = true;
        notificacion.FechaConfirmacion = DateTime.Now;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<List<NotificacionEvento>>
        ObtenerNotificacionesPendientes()
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.NotificacionesEventos
            .Where(n =>
                !n.Confirmada &&
                !n.Evento.Eliminado &&
                n.Evento.Estado == n.EstadoAnterior &&
                (
                    (n.EstadoAnterior == "Pendiente" &&
                     n.EstadoNuevo == "Activo") ||
                    (n.EstadoAnterior == "Activo" &&
                     n.EstadoNuevo == "Finalizado")
                ))
            .AsNoTracking()
            .OrderBy(n => n.Fecha)
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int idEvento)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == idEvento &&
                !e.Eliminado);

        if (evento == null)
            return false;

        evento.Eliminado = true;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExisteNombre(
        string nombre,
        int eventoId = 0)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .AnyAsync(e =>
                e.Nombre == nombre &&
                e.EventoId != eventoId);
    }

    public async Task<bool> Recuperar(int idEvento)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == idEvento &&
                e.Eliminado);

        if (evento == null)
            return false;

        evento.Eliminado = false;

        return await contexto.SaveChangesAsync() > 0;
    }


    public async Task<List<Evento>> ObtenerEventosCliente(
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .Where(e =>
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == applicationUserId &&
                !e.Eliminado)
            .AsNoTracking()
            .OrderBy(e => e.FechaInicio)
            .ThenBy(e => e.HoraInicio)
            .ToListAsync();
    }

}
