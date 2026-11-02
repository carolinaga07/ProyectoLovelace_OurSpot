using OurSpot.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Personal
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es valido")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio")]
    public string Cargo { get; set; } = string.Empty;

    public string Usuario { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public string Estado { get; set; } = "Activo";

    [Required(ErrorMessage = "Seleccione un rol")]
    public string Rol { get; set; } = "Empleado";

    [ForeignKey(nameof(ApplicationUser))]
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

}