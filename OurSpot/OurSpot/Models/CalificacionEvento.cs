using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class CalificacionEvento
{
    [Key]
    public int CalificacionId { get; set; }
    public int EventoId { get; set; }
    [ForeignKey(nameof(EventoId))]
    public Evento? Evento { get; set; }
    public int ClienteId { get; set; }
    [ForeignKey(nameof(ClienteId))]
    public Clientes Clientes { get; set; } = null!;
    [Range(1, 5)]
    public int Estrellas { get; set; }

    public DateTime FechaCalificacion { get; set; } = DateTime.UtcNow;

}
