using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models;

public class Auditoria
{
    [Key]
    public int AuditoriaId { get; set; }
    public string UsuarioEmail { get; set; } = string.Empty;
    public string Seccion { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
}