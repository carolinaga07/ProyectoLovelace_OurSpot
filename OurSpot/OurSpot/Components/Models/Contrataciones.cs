using System.ComponentModel.DataAnnotations;

namespace OurSpot.Components.Models;

public class Contrataciones
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage ="Este campo es obligatorio")]
    public string Evento { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Proveedor { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio")]
    public DateOnly? Fecha { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Servicio { get; set; }
    [Range(1, double.MaxValue, ErrorMessage ="Este campo es obligatorio")]
    public double Costo { get; set; }
    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Estado { get; set; }
    public bool Eliminado { get; set; } = false;
}
