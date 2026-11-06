using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class NotificacionEvento
{
    [Key]
    public int NotificacionId { get; set; }

    [ForeignKey(nameof(Evento))]
    public int EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public string NombreEvento { get; set; } = string.Empty;
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool Confirmada { get; set; } = false;
    public DateTime? FechaConfirmacion { get; set; }
}
