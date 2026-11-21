
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Models.DTOs;

namespace OurSpot.Services;

public class PortalProveedoresService(
    IDbContextFactory<Contexto> contextFactory, NotificacionesApiService notificacionesApi)
{
    public async Task<List<ServicioProveedorDto>>
        ObtenerServiciosProveedor(string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.EventosDetalles
            .AsNoTracking()
            .Where(d =>
                d.Evento != null &&
                !d.Evento.Eliminado &&
                d.Contrataciones != null &&
                !d.Contrataciones.Eliminado &&
                d.Contrataciones.ProveedorEntidad != null &&
                !d.Contrataciones.ProveedorEntidad.Eliminado &&
                d.Contrataciones.ProveedorEntidad.ApplicationUserId
                    == applicationUserId)
            .Select(d => new ServicioProveedorDto
            {
                DetalleId = d.DetalleId,
                EventoId = d.EventoId,
                NombreEvento = d.Evento!.Nombre,
                Servicio = d.Servicio,
                Lugar = d.Evento.Lugar,
                FechaInicio = d.Evento.FechaInicio,
                HoraInicio = d.Evento.HoraInicio,
                EstadoConfirmacion = d.EstadoConfirmacion,
                FechaRespuesta = d.FechaRespuesta,
                MotivoRechazo = d.MotivoRechazo,
                EstadoOperativo = d.EstadoOperativo,
                ObservacionesProveedor = d.ObservacionesProveedor
               
            })
            .OrderBy(d => d.FechaInicio)
            .ThenBy(d => d.HoraInicio)
            .ToListAsync();
    }


    public async Task<ServicioProveedorDto?> ObtenerDetalleServicio(
        int detalleId,
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.EventosDetalles
            .AsNoTracking()
            .Where(d =>
                d.DetalleId == detalleId &&
                d.Evento != null &&
                !d.Evento.Eliminado &&
                d.Contrataciones != null &&
                !d.Contrataciones.Eliminado &&
                d.Contrataciones.ProveedorEntidad != null &&
                !d.Contrataciones.ProveedorEntidad.Eliminado &&
                d.Contrataciones.ProveedorEntidad.ApplicationUserId
                    == applicationUserId)
            .Select(d => new ServicioProveedorDto
            {
                DetalleId = d.DetalleId,
                EventoId = d.EventoId,
                NombreEvento = d.Evento!.Nombre,
                Servicio = d.Servicio,
                Lugar = d.Evento.Lugar,
                FechaInicio = d.Evento.FechaInicio,
                HoraInicio = d.Evento.HoraInicio,
                EstadoConfirmacion = d.EstadoConfirmacion,
                FechaRespuesta = d.FechaRespuesta,
                MotivoRechazo = d.MotivoRechazo,
                EstadoOperativo = d.EstadoOperativo,
                ObservacionesProveedor = d.ObservacionesProveedor
            })
            .FirstOrDefaultAsync();
    }


    public async Task<bool> ActualizarEstadoServicio(
        int detalleId,
        string applicationUserId,
        string estado,
        string? observaciones)
    {
        var estadosPermitidos = new[]
        {
        "Pendiente",
        "En proceso",
        "Completado"
    };
       

        if (string.IsNullOrWhiteSpace(applicationUserId) ||
            !estadosPermitidos.Contains(estado))
        {
            return false;
        }

        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var detalle = await contexto.EventosDetalles
        .FirstOrDefaultAsync(d =>
        d.DetalleId == detalleId &&
        d.EstadoConfirmacion == "Aceptada" &&
        d.Evento != null &&
        !d.Evento.Eliminado &&
        d.Contrataciones != null &&
        !d.Contrataciones.Eliminado &&
        d.Contrataciones.ProveedorEntidad != null &&
        !d.Contrataciones.ProveedorEntidad.Eliminado &&
        d.Contrataciones.ProveedorEntidad.ApplicationUserId
            == applicationUserId);

        if (detalle is null)
            return false;

        // Guardamos el estado anterior
        string estadoAnterior = detalle.EstadoOperativo;

        // Comprobamos si cambió el estado
        bool cambioEstado = estadoAnterior != estado;

        // Actualizamos el servicio
        detalle.EstadoOperativo = estado;
        detalle.ObservacionesProveedor = observaciones?.Trim();
        detalle.FechaActualizacionEstado = DateTime.UtcNow;

        NotificacionProveedor? notificacion = null;
        // Registramos la notificación si cambió el estado
        if (cambioEstado)
        {
            var nombreEvento = await contexto.Eventos
                .Where(e => e.EventoId == detalle.EventoId)
                .Select(e => e.Nombre)
                .FirstAsync();

            notificacion = new NotificacionProveedor
            {
                EventoId = detalle.EventoId,
                DetalleId = detalle.DetalleId,
                NombreEvento = nombreEvento,
                Servicio = detalle.Servicio,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estado,
                Observaciones = detalle.ObservacionesProveedor,
                Fecha = DateTime.UtcNow,
                Leida = false
            };

            contexto.NotificacionesProveedores.Add(notificacion);
        }


        bool guardado = await contexto.SaveChangesAsync() > 0;

        if (!guardado)
            return false;

        if (notificacion is not null)
        {
            try
            {
                await notificacionesApi.NotificarActualizacionProveedor(
                    notificacion.NotificacionProveedorId,
                    notificacion.EventoId,
                    notificacion.DetalleId,
                    notificacion.NombreEvento,
                    notificacion.Servicio,
                    notificacion.EstadoAnterior,
                    notificacion.EstadoNuevo,
                    notificacion.Observaciones,
                    notificacion.Fecha);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error al notificar actualización del proveedor: {ex}");
            }
        }

        return true;


    }


    public async Task<PerfilProveedorDto?> ObtenerPerfil(
        string applicationUserId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.Proveedores
            .AsNoTracking()
            .Where(p =>
                p.ApplicationUserId == applicationUserId &&
                !p.Eliminado)
            .Select(p => new PerfilProveedorDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Rnc = p.Rnc,
                Servicio = p.Servicio,
                Direccion = p.Direccion,
                Contacto = p.Contacto
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ActualizarPerfil(
        string applicationUserId,
        PerfilProveedorDto perfil)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId) ||
            string.IsNullOrWhiteSpace(perfil.Direccion) ||
            string.IsNullOrWhiteSpace(perfil.Contacto))
            return false;

        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var proveedor = await contexto.Proveedores
            .FirstOrDefaultAsync(p =>
                p.ApplicationUserId == applicationUserId &&
                !p.Eliminado);

        if (proveedor is null)
            return false;

        proveedor.Direccion = perfil.Direccion.Trim();
        proveedor.Contacto = perfil.Contacto.Trim();

        await contexto.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AceptarSolicitud(
    int detalleId,
    string applicationUserId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return false;

        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var ahora = DateTime.UtcNow;

        var filasActualizadas = await contexto.EventosDetalles
            .Where(d =>
                d.DetalleId == detalleId &&
                d.EstadoConfirmacion == "Pendiente" &&
                d.Evento != null &&
                !d.Evento.Eliminado &&
                d.Contrataciones != null &&
                !d.Contrataciones.Eliminado &&
                d.Contrataciones.ProveedorEntidad != null &&
                !d.Contrataciones.ProveedorEntidad.Eliminado &&
                d.Contrataciones.ProveedorEntidad.ApplicationUserId
                    == applicationUserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(d => d.EstadoConfirmacion, "Aceptada")
                .SetProperty(d => d.FechaRespuesta, ahora)
                .SetProperty(d => d.MotivoRechazo, (string?)null));

        return filasActualizadas == 1;
    }
   
    public async Task<bool> RechazarSolicitud(
    int detalleId,
    string applicationUserId,
    string motivo)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId) ||
            string.IsNullOrWhiteSpace(motivo))
            return false;

        motivo = motivo.Trim();

        if (motivo.Length > 500)
            return false;

        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var ahora = DateTime.UtcNow;

        var filasActualizadas = await contexto.EventosDetalles
            .Where(d =>
                d.DetalleId == detalleId &&
                d.EstadoConfirmacion == "Pendiente" &&
                d.Evento != null &&
                !d.Evento.Eliminado &&
                d.Contrataciones != null &&
                !d.Contrataciones.Eliminado &&
                d.Contrataciones.ProveedorEntidad != null &&
                !d.Contrataciones.ProveedorEntidad.Eliminado &&
                d.Contrataciones.ProveedorEntidad.ApplicationUserId
                    == applicationUserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(d => d.EstadoConfirmacion, "Rechazada")
                .SetProperty(d => d.FechaRespuesta, ahora)
                .SetProperty(d => d.MotivoRechazo, motivo));

        return filasActualizadas == 1;
    }
}
