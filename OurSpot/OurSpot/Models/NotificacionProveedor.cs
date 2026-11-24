
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class NotificacionProveedor
{
    [Key]
    public int NotificacionProveedorId { get; set; }

    public int EventoId { get; set; }

    [ForeignKey(nameof(EventoId))]
    public Evento Evento { get; set; } = null!;

    public int DetalleId { get; set; }

    public string NombreEvento { get; set; } = string.Empty;

    public string Servicio { get; set; } = string.Empty;

    public string EstadoAnterior { get; set; } = string.Empty;

    public string EstadoNuevo { get; set; } = string.Empty;

    public string? Observaciones { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public bool Leida { get; set; } = false;

    public DateTime? FechaLectura { get; set; }

    public string TipoNotificacion { get; set; } = "EstadoOperativo";

    public string? DestinatarioId { get; set; }

    public string? RolDestinatario { get; set; }
}
