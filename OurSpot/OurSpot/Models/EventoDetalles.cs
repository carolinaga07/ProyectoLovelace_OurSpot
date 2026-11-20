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

    // Estado que actualiza el proveedor
    public string EstadoOperativo { get; set; } = "Pendiente";

    // Observaciones del proveedor
    public string? ObservacionesProveedor { get; set; }

    // Última actualización
    public DateTime? FechaActualizacionEstado { get; set; }
}
