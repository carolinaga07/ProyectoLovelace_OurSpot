namespace OurSpot.Models.DTOs;

public class EventoImagenDto
{
    public int EventoImagenId { get; set; }
    public string RutaImagen { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
}
