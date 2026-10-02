using System.ComponentModel.DataAnnotations;

namespace OurSpot.Models
{
    public class Evento
    {
        public int EventoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoria es obligatoria")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El lugar es obligatorio")]
        public string Lugar { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Estado { get; set; } = "Pendiente";
    }

}
