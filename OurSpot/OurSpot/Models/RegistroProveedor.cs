
using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models;

public class RegistroProveedor
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo inválido")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña")]
    [Compare(nameof(Contrasena),
        ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarContrasena { get; set; }
        = string.Empty;
}
