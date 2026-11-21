using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class EventoDetalles
{
    [Key]
    public int DetalleId { get; set; }

    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }

    public int ContratacionId { get; set; }

    [ForeignKey(nameof(ContratacionId))]
    public Contrataciones? Contrataciones { get; set; }

    public string Servicio { get; set; } = string.Empty;

    public double PrecioBase { get; set; }

    public double PorcentajeGanancia { get; set; } = 25;

    public double PrecioFinal { get; set; }

    // Confirmación del proveedor
    public string EstadoConfirmacion { get; set; } = "Pendiente";

    public DateTime? FechaRespuesta { get; set; }

    public string? MotivoRechazo { get; set; }

    // Ejecución del servicio
    public string EstadoOperativo { get; set; } = "Pendiente";

    public string? ObservacionesProveedor { get; set; }

    public DateTime? FechaActualizacionEstado { get; set; }
}