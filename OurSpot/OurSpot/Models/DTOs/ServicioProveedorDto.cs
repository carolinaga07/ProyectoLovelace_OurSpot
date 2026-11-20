namespace OurSpot.Models.DTOs;

public class ServicioProveedorDto
{
    public int DetalleId { get; set; }

    public int EventoId { get; set; }

    public string NombreEvento { get; set; } = string.Empty;

    public string Servicio { get; set; } = string.Empty;

    public string Lugar { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public string EstadoOperativo { get; set; } = string.Empty;

    public string? ObservacionesProveedor { get; set; }
}
