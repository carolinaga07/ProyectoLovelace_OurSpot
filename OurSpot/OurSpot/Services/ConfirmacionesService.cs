
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
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
}
