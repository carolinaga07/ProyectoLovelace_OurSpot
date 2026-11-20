
namespace OurSpot.API.DTOs;

public class NotificacionProveedorDto
{
    public int NotificacionProveedorId { get; set; }

    public int EventoId { get; set; }

    public int DetalleId { get; set; }

    public string NombreEvento { get; set; } = string.Empty;

    public string Servicio { get; set; } = string.Empty;

    public string EstadoAnterior { get; set; } = string.Empty;

    public string EstadoNuevo { get; set; } = string.Empty;

    public string? Observaciones { get; set; }

    public DateTime Fecha { get; set; }
}
