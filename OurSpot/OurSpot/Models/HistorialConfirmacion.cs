
namespace OurSpot.Models;

public class HistorialConfirmacion
{
    public int HistorialConfirmacionId { get; set; }

    public int DetalleId { get; set; }

    public int ContratacionIdAnterior { get; set; }

    public int ContratacionIdNueva { get; set; }

    public string EstadoAnterior { get; set; } = string.Empty;

    public string? MotivoRechazo { get; set; }

    public DateTime? FechaRespuestaAnterior { get; set; }

    public DateTime FechaReasignacion { get; set; } = DateTime.UtcNow;
}
