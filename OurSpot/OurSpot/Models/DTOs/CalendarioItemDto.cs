
namespace OurSpot.Models.DTOs;

public class CalendarioItemDto
{
    public int Id { get; set; }

    public int EventoId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string NombreEvento { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string Lugar { get; set; } = string.Empty;

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public string? Observaciones { get; set; }

    public string UrlDetalle { get; set; } = string.Empty;
}
