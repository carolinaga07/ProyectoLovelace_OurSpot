using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class EventoDetalles
{
    [Key]
    public int DetalleId { get; set; }
    public int EventoId { get; set; }
    public int Contratacion { get; set; }
    public int Cantidad { get; set; } = 1;
    public decimal PrecioBase { get; set; }
    public decimal PorcentajeGanacias { get; set; } = 25;
    public decimal PrecioFinal { get; set; }

    [ForeignKey("ContratacionId")]
    public Contrataciones? Contrataciones { get; set; }

}
