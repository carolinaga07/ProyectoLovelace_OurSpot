namespace OurSpot.Models.DTOs;

public class EditarEventoClienteDto
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

    public List<int> ContratacionesIds { get; set; } = new();

    public List<ServicioEditableClienteDto> Servicios { get; set; } = new();
}

public class ServicioEditableClienteDto
{
    public int ContratacionId { get; set; }
    public string Servicio { get; set; } = string.Empty;
    public double PrecioFinal { get; set; }
}
