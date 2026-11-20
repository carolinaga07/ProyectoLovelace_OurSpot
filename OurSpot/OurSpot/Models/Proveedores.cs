using OurSpot.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurSpot.Models
{
    public class Proveedores
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string Rnc { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Contacto { get; set; } = null!;
        public string Servicio { get; set; } = null!;

        public bool Eliminado { get; set; } = false;

        // Cuenta que permite acceder al portal del proveedor
        [ForeignKey(nameof(ApplicationUser))]
        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        // Contrataciones de esta empresa
        public ICollection<Contrataciones> Contrataciones { get; set; }
            = new List<Contrataciones>();

    }
}
