using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class EventoDetalles
{
    [Key]
    public int DetalleId { get; set; }
    public int EventoId { get; set; }
    public int ContratacionId { get; set; }
    public string Servicio { get; set; } = string.Empty;
    public double PrecioBase { get; set; }
    public double PorcentajeGanacia { get; set; } = 25;
    public double PrecioFinal { get; set; }

    [ForeignKey("ContratacionId")]
    public Contrataciones? Contrataciones { get; set; }
    public double PorcentajeGanancia { get; internal set; }
}
