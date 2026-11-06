using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models.DTOs;

public class PerfilClienteDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no es válido.")]
    public string? Telefono { get; set; }
}
