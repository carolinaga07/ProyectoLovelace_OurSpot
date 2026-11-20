using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class Contrataciones
{

    [Key]
    public int ContratacionId { get; set; }

    // Se conserva temporalmente por compatibilidad
    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Proveedor { get; set; } = string.Empty;

    // Relación con el proveedor registrado
    public int? ProveedorId { get; set; }

    [ForeignKey(nameof(ProveedorId))]
    public Proveedores? ProveedorEntidad { get; set; }

    [Range(1, double.MaxValue,
        ErrorMessage = "Este campo es obligatorio")]
    public double Costo { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Estado { get; set; } = "Pendiente";

    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es obligatorio")]
    public string Servicio { get; set; } = string.Empty;

}
