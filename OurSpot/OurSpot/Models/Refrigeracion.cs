using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models
{
    public class Refrigeracion
    {
        [Key]
        public int IdRefrigeracion { get; set;}

        public string Nombre { get; set; } 
        public string Descripcion { get; set; }

    }
}
