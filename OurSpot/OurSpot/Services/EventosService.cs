
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Models.DTOs;
using OurSpot.Services;
using System.Linq.Expressions;

namespace OurSpot.Components.Pages.AdministradorPages.EventosPages.Services;

public class EventosService(
    IDbContextFactory<Contexto> contextFactory,
    NotificacionesApiService notificacionesApi, AuditoriaService auditoriaService)
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

        bool esNuevo = eventos.EventoId == 0;

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
        {
            await RevisarEstadosPendientes();
            string accion = esNuevo ? "Insertar" : "Modificar";
            await auditoriaService.RegistrarAsync("Eventos", accion, $"Se guardó el evento: {eventos.Nombre}");
        }

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

            // Invalidar notificaciones que ya no corresponden.
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

            foreach (var invalida in notificacionesInvalidas)
            {
                invalida.Confirmada = true;
                invalida.FechaConfirmacion = ahora;
            }

            if (notificacionesInvalidas.Count > 0)
                await contexto.SaveChangesAsync();

            if (evento.Estado == estadoNuevo)
                continue;

            // Buscar una notificación pendiente existente.
            var notificacion =
                await contexto.NotificacionesEventos
                    .FirstOrDefaultAsync(n =>
                        n.EventoId == evento.EventoId &&
                        !n.Confirmada);

            // Crear solamente si todavía no existe.
            if (notificacion == null)
            {
                notificacion = new NotificacionEvento
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
            }

            // Enviar también las notificaciones ya existentes.
            try
            {
                await notificacionesApi.NotificarCambioEstado(
                    notificacion.EventoId,
                    notificacion.NombreEvento,
                    notificacion.EstadoAnterior,
                    notificacion.EstadoNuevo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"ERROR AL CONECTAR CON OURSPOT.API: {ex}");
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

        var guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
            await auditoriaService.RegistrarAsync("Eventos", "CambioEstado", $"El evento '{evento.Nombre}' cambió su estado a {evento.Estado}");

        return guardado;
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

    public async Task<bool> CancelarEvento(int idEvento)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == idEvento &&
                !e.Eliminado &&
                (e.Estado == "Pendiente" || e.Estado == "Activo"));

        if (evento == null)
            return false;

        evento.Estado = "Cancelado";

        // Invalidar las notificaciones pendientes de cambio de estado.
        var notificaciones = await contexto.NotificacionesEventos
            .Where(n => n.EventoId == idEvento && !n.Confirmada)
            .ToListAsync();

        foreach (var notificacion in notificaciones)
        {
            notificacion.Confirmada = true;
            notificacion.FechaConfirmacion = DateTime.Now;
        }

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> ReactivarEvento(int idEvento)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .FirstOrDefaultAsync(e =>
                e.EventoId == idEvento &&
                !e.Eliminado &&
                e.Estado == "Cancelado");

        if (evento == null)
            return false;

        var ahora = DateTime.Now;
        var inicio = evento.FechaInicio.ToDateTime(evento.HoraInicio);
        var cierre = evento.FechaCierre.ToDateTime(evento.HoraCierre);

        if (ahora >= cierre)
            return false;

        evento.Estado = ahora >= inicio ? "Activo" : "Pendiente";

        return await contexto.SaveChangesAsync() > 0;
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

        var eliminado = await contexto.SaveChangesAsync() > 0;

        if (eliminado)
            await auditoriaService.RegistrarAsync("Eventos", "Eliminar", $"Se eliminó el evento: {evento.Nombre}");

        return eliminado;
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

        var recuperado = await contexto.SaveChangesAsync() > 0;

        if (recuperado)
            await auditoriaService.RegistrarAsync("Eventos", "Recuperar", $"Se recuperó el evento: {evento.Nombre}");

        return recuperado;
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


    public async Task<bool> CrearEventoCliente(
        Evento nuevoEvento,
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var cliente = await contexto.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.ApplicationUserId == applicationUserId);

        if (cliente == null)
            return false;

        var inicio = nuevoEvento.FechaInicio
            .ToDateTime(nuevoEvento.HoraInicio);

        var cierre = nuevoEvento.FechaCierre
            .ToDateTime(nuevoEvento.HoraCierre);

        if (inicio < DateTime.Now || cierre <= inicio)
            return false;

        if (string.IsNullOrWhiteSpace(nuevoEvento.Nombre) ||
            string.IsNullOrWhiteSpace(nuevoEvento.Descripcion) ||
            string.IsNullOrWhiteSpace(nuevoEvento.Tipo) ||
            string.IsNullOrWhiteSpace(nuevoEvento.Lugar))
            return false;

        if (nuevoEvento.Tipo != "Publico" &&
            nuevoEvento.Tipo != "Privado")
            return false;

        bool lugarValido = await contexto.Lugares
            .AnyAsync(l => l.Nombre == nuevoEvento.Lugar);

        if (!lugarValido)
            return false;

        var ids = nuevoEvento.EventosDetalles
            .Select(d => d.ContratacionId)
            .ToList();

        if (ids.Count != ids.Distinct().Count())
            return false;

        var contrataciones = await contexto.Contrataciones
            .AsNoTracking()
            .Where(c =>
                ids.Contains(c.ContratacionId) &&
                !c.Eliminado &&
                c.Estado == "Activa")
            .ToListAsync();

        if (contrataciones.Count != ids.Count)
            return false;

        const double ganancia = 25;

        var evento = new Evento
        {
            ClienteId = cliente.ClienteId,
            Nombre = nuevoEvento.Nombre.Trim(),
            Descripcion = nuevoEvento.Descripcion.Trim(),
            FechaInicio = nuevoEvento.FechaInicio,
            HoraInicio = nuevoEvento.HoraInicio,
            FechaCierre = nuevoEvento.FechaCierre,
            HoraCierre = nuevoEvento.HoraCierre,
            Tipo = nuevoEvento.Tipo,
            Lugar = nuevoEvento.Lugar,
            Estado = "Pendiente",
            Eliminado = false,
            EventosDetalles = contrataciones.Select(c =>
                new EventoDetalles
                {
                    ContratacionId = c.ContratacionId,
                    Servicio = nuevoEvento.EventosDetalles
                        .First(d =>
                            d.ContratacionId == c.ContratacionId)
                        .Servicio,
                    PrecioBase = c.Costo,
                    PorcentajeGanancia = ganancia,
                    PrecioFinal = c.Costo * (1 + ganancia / 100)
                }).ToList()
        };

        evento.Total = evento.EventosDetalles
            .Sum(d => d.PrecioFinal);

        contexto.Eventos.Add(evento);

        bool guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
        {
            await RevisarEstadosPendientes();
            await auditoriaService.RegistrarAsync("Eventos", "CrearCliente", $"El cliente creó el evento: {evento.Nombre}");
        }

        return guardado;
    }


    public async Task<EventoClienteDetalleDto?> ObtenerDetalleCliente(
        int eventoId,
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .AsNoTracking()
            .Where(e =>
                e.EventoId == eventoId &&
                !e.Eliminado &&
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == applicationUserId)
            .Select(e => new EventoClienteDetalleDto
            {
                EventoId = e.EventoId,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                FechaInicio = e.FechaInicio,
                HoraInicio = e.HoraInicio,
                FechaCierre = e.FechaCierre,
                HoraCierre = e.HoraCierre,
                Lugar = e.Lugar,
                Tipo = e.Tipo,
                Estado = e.Estado,
                Total = e.Total,

                Contrataciones = e.EventosDetalles
                    .Select(d => new ContratacionClienteDetalleDto
                    {
                        Servicio = d.Servicio,
                        PrecioFinal = d.PrecioFinal
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }


    public async Task<EditarEventoClienteDto?> ObtenerEventoParaEditarCliente(
        int eventoId,
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Eventos
            .AsNoTracking()
            .Where(e =>
                e.EventoId == eventoId &&
                !e.Eliminado &&
                e.Estado == "Pendiente" &&
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == applicationUserId)
            .Select(e => new EditarEventoClienteDto
            {
                EventoId = e.EventoId,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                FechaInicio = e.FechaInicio,
                HoraInicio = e.HoraInicio,
                FechaCierre = e.FechaCierre,
                HoraCierre = e.HoraCierre,
                Lugar = e.Lugar,
                Tipo = e.Tipo,

                ContratacionesIds = e.EventosDetalles
                    .Select(d => d.ContratacionId)
                    .ToList(),

                Servicios = e.EventosDetalles
                .Select(d => new ServicioEditableClienteDto
                {
                  ContratacionId = d.ContratacionId,
                  Servicio = d.Servicio,
                  PrecioFinal = d.PrecioFinal
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
    }


    public async Task<bool> ActualizarEventoCliente(
        EditarEventoClienteDto datos,
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var evento = await contexto.Eventos
            .Include(e => e.EventosDetalles)
            .FirstOrDefaultAsync(e =>
                e.EventoId == datos.EventoId &&
                !e.Eliminado &&
                e.Estado == "Pendiente" &&
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == applicationUserId);

        if (evento == null)
            return false;

        var inicio = datos.FechaInicio.ToDateTime(datos.HoraInicio);
        var cierre = datos.FechaCierre.ToDateTime(datos.HoraCierre);

        if (inicio < DateTime.Now || cierre <= inicio)
            return false;

        if (string.IsNullOrWhiteSpace(datos.Nombre) ||
            string.IsNullOrWhiteSpace(datos.Descripcion) ||
            string.IsNullOrWhiteSpace(datos.Lugar))
            return false;

        if (datos.Tipo != "Publico" && datos.Tipo != "Privado")
            return false;

        bool lugarValido = await contexto.Lugares
            .AnyAsync(l => l.Nombre == datos.Lugar);

        if (!lugarValido)
            return false;

        var ids = datos.ContratacionesIds;

        if (ids.Count != ids.Distinct().Count())
            return false;

        var existentes = evento.EventosDetalles
            .ToDictionary(d => d.ContratacionId);

        var idsNuevos = ids
            .Where(id => !existentes.ContainsKey(id))
            .ToList();

        var nuevasContrataciones = await contexto.Contrataciones
            .AsNoTracking()
            .Where(c =>
                idsNuevos.Contains(c.ContratacionId) &&
                !c.Eliminado &&
                c.Estado == "Activa")
            .ToListAsync();

        if (nuevasContrataciones.Count != idsNuevos.Count)
            return false;

        foreach (var detalle in evento.EventosDetalles
            .Where(d => !ids.Contains(d.ContratacionId))
            .ToList())
        {
            evento.EventosDetalles.Remove(detalle);
            contexto.Remove(detalle);
        }

        const double ganancia = 25;

        foreach (var contratacion in nuevasContrataciones)
        {
            evento.EventosDetalles.Add(new EventoDetalles
            {
                ContratacionId = contratacion.ContratacionId,
                Servicio = contratacion.Servicio,
                PrecioBase = contratacion.Costo,
                PorcentajeGanancia = ganancia,
                PrecioFinal = contratacion.Costo * (1 + ganancia / 100)
            });
        }

        evento.Nombre = datos.Nombre.Trim();
        evento.Descripcion = datos.Descripcion.Trim();
        evento.FechaInicio = datos.FechaInicio;
        evento.HoraInicio = datos.HoraInicio;
        evento.FechaCierre = datos.FechaCierre;
        evento.HoraCierre = datos.HoraCierre;
        evento.Lugar = datos.Lugar;
        evento.Tipo = datos.Tipo;

        evento.Total = evento.EventosDetalles
            .Sum(d => d.PrecioFinal);

        bool guardado = await contexto.SaveChangesAsync() > 0;

        if (guardado)
        {
            await RevisarEstadosPendientes();
            await auditoriaService.RegistrarAsync("Eventos", "ActualizarCliente", $"El cliente actualizó su evento: {evento.Nombre}");
        }

        return guardado;
    }

  public async Task<List<Evento>> ObtenerEventoDestacados()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eventos = await contexto.Eventos.Where(e => e.Estado == "Finalizado" &&
        !e.Eliminado).Select(e => new
        {
            Evento = e,
            Promedio = e.calificacionEventos.Select(c => (double?)c.Estrellas)
            .Average() ?? 0

        })
        .AsNoTracking()
        .ToListAsync();

        return eventos.OrderByDescending(e => e.Promedio).Take(5)
            .Select(e =>
            {
                e.Evento.Estrella = e.Promedio;
                return e.Evento;
            })
            .ToList();
    }
}
