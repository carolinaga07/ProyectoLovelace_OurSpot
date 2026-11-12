namespace OurSpot.API.DTOs;

public class NotificacionEventoDto
{
    public int EventoId { get; set; }
    public string NombreEvento { get; set; } = string.Empty;
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
}
