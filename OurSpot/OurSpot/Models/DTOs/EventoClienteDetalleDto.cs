namespace OurSpot.Models.DTOs;

public class EventoClienteDetalleDto
{
    public int EventoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public DateOnly FechaCierre { get; set; }
    public TimeOnly HoraCierre { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public double Total { get; set; }

    public List<ContratacionClienteDetalleDto> Contrataciones
    { get; set; } = new();
}
public class ContratacionClienteDetalleDto
{
    public string Servicio { get; set; } = string.Empty;
    public double PrecioFinal { get; set; }
}
