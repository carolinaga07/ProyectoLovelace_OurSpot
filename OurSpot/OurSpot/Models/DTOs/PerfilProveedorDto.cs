
using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models.DTOs;

public class PerfilProveedorDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Rnc { get; set; } = string.Empty;

    public string Servicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contacto es obligatorio")]
    public string Contacto { get; set; } = string.Empty;
}
