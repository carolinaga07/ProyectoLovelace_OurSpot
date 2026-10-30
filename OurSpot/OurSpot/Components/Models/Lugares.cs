using System.ComponentModel.DataAnnotations;

namespace OurSpot.Components.Models;

public class Lugares
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "La ubicación es obligatoria")]
    public string Ubicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este  es obligatorio")]
    public string Categoria { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor que 0")]
    public int Capacidad { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio")]
    public string Estado { get; set; } = "Disponible";

    public bool Eliminado { get; set; } = false;
}
