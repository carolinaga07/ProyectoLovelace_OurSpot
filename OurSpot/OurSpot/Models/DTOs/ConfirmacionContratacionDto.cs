
namespace OurSpot.Models.DTOs;

public class ConfirmacionContratacionDto
{
    public int DetalleId { get; set; }

    public int EventoId { get; set; }

    public int ContratacionId { get; set; }

    public int? ProveedorId { get; set; }

    public string NombreEvento { get; set; } = string.Empty;

    public string NombreProveedor { get; set; } = string.Empty;

    public string Servicio { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Lugar { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public string EstadoConfirmacion { get; set; } = "Pendiente";

    public string EstadoOperativo { get; set; } = "Pendiente";

    public DateTime? FechaRespuesta { get; set; }

    public string? MotivoRechazo { get; set; }
}
