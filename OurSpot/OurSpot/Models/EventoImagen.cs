using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class EventoImagen
{
    [Key]
    public int EventoImagenId { get; set; }

    [Required]
    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento Evento { get; set; } = null!;

    [Required]
    public string RutaImagen { get; set; } = string.Empty;

    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}
