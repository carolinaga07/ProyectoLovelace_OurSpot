using OurSpot.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models;

public class Clientes
{
    [Key]
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    
    [ForeignKey(nameof(ApplicationUser))]
    public string? ApplicationUserId { get; set; } 
    public ApplicationUser? ApplicationUser { get; set; }

    public List<Evento> Eventos { get; set; } = new List<Evento>();
}
