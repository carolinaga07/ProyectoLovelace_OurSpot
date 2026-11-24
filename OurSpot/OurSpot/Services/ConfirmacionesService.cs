
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using OurSpot.Models.DTOs;

namespace OurSpot.Services;

public class ConfirmacionesService(
    IDbContextFactory<Contexto> contextFactory)
{
    public async Task<List<ConfirmacionContratacionDto>>
        ObtenerConfirmaciones()
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        return await contexto.EventosDetalles
            .AsNoTracking()
            .Where(d =>
                d.Evento != null &&
                !d.Evento.Eliminado &&
                d.Contrataciones != null &&
                !d.Contrataciones.Eliminado)
            .Select(d => new ConfirmacionContratacionDto
            {
                DetalleId = d.DetalleId,

                EventoId = d.EventoId,

                ContratacionId = d.ContratacionId,

                ProveedorId = d.Contrataciones!.ProveedorId,

                NombreEvento = d.Evento!.Nombre,

                NombreProveedor =
                    d.Contrataciones.ProveedorEntidad != null
                        ? d.Contrataciones.ProveedorEntidad.Nombre
                        : d.Contrataciones.Proveedor,

                Servicio = d.Servicio,

                Categoria = d.Contrataciones.Categoria,

                Lugar = d.Evento.Lugar,

                FechaInicio = d.Evento.FechaInicio,

                EstadoConfirmacion = d.EstadoConfirmacion,

                EstadoOperativo = d.EstadoOperativo,

                FechaRespuesta = d.FechaRespuesta,

                MotivoRechazo = d.MotivoRechazo
            })
            .OrderBy(d => d.FechaInicio)
            .ThenBy(d => d.NombreEvento)
            .ToListAsync();
    }


    public async Task<bool> ReasignarContratacion(
        int detalleId,
        int nuevaContratacionId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        await using var transaccion =
            await contexto.Database.BeginTransactionAsync();

        var detalle = await contexto.EventosDetalles
            .Include(d => d.Evento)
            .Include(d => d.Contrataciones)
            .FirstOrDefaultAsync(d =>
                d.DetalleId == detalleId &&
                d.EstadoConfirmacion == "Rechazada" &&
                d.Evento != null &&
                !d.Evento.Eliminado);

        if (detalle is null || detalle.Contrataciones is null)
            return false;

        var nuevaContratacion = await contexto.Contrataciones
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.ContratacionId == nuevaContratacionId &&
                !c.Eliminado &&
                c.Estado == "Activo" &&
                c.ProveedorId != null &&
                c.ProveedorEntidad != null &&
                !c.ProveedorEntidad.Eliminado &&
                c.ProveedorEntidad.ApplicationUserId != null &&
                c.ProveedorEntidad.ApplicationUserId != "" &&
                c.Categoria == detalle.Contrataciones.Categoria);

        if (nuevaContratacion is null)
            return false;

        if (detalle.ContratacionId == nuevaContratacionId)
            return false;

        var historial = new HistorialConfirmacion
        {
            DetalleId = detalle.DetalleId,

            ContratacionIdAnterior = detalle.ContratacionId,

            ContratacionIdNueva = nuevaContratacionId,

            EstadoAnterior = detalle.EstadoConfirmacion,

            MotivoRechazo = detalle.MotivoRechazo,

            FechaRespuestaAnterior = detalle.FechaRespuesta,

            FechaReasignacion = DateTime.UtcNow
        };

        contexto.HistorialConfirmaciones.Add(historial);

        detalle.ContratacionId = nuevaContratacionId;

        detalle.Servicio = nuevaContratacion.Servicio;

        detalle.EstadoConfirmacion = "Pendiente";

        detalle.FechaRespuesta = null;

        detalle.MotivoRechazo = null;

        detalle.EstadoOperativo = "Pendiente";

        detalle.ObservacionesProveedor = null;

        detalle.FechaActualizacionEstado = null;

        await contexto.SaveChangesAsync();

        await transaccion.CommitAsync();

        return true;
    }


    public async Task<List<Contrataciones>> ObtenerContratacionesDisponibles(
        int detalleId)
    {
        await using var contexto =
            await contextFactory.CreateDbContextAsync();

        var detalle = await contexto.EventosDetalles
            .AsNoTracking()
            .Where(d =>
                d.DetalleId == detalleId &&
                d.EstadoConfirmacion == "Rechazada" &&
                d.Evento != null &&
                !d.Evento.Eliminado)
            .Select(d => new
            {
                d.ContratacionId,
                Categoria = d.Contrataciones!.Categoria
            })
            .FirstOrDefaultAsync();

        if (detalle is null)
            return new List<Contrataciones>();

        return await contexto.Contrataciones
            .AsNoTracking()
            .Where(c =>
                !c.Eliminado &&
                c.Estado == "Activo" &&
                c.ContratacionId != detalle.ContratacionId &&
                c.Categoria == detalle.Categoria &&
                c.ProveedorId != null &&
                c.ProveedorEntidad != null &&
                !c.ProveedorEntidad.Eliminado &&
                c.ProveedorEntidad.ApplicationUserId != null &&
                c.ProveedorEntidad.ApplicationUserId != "")
            .OrderBy(c => c.Proveedor)
            .ThenBy(c => c.Servicio)
            .ToListAsync();
    }


}
