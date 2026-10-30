using OurSpot.Components.Pages.AdministradorPages.EventosPagesNavMenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace OurSpot.Models
{
    public class Evento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripcion es necesaria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateOnly FechaInicio { get; set; }
        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeOnly HoraInicio { get; set;  }
        [Required(ErrorMessage = "La fecha de cierre es obligatoria")]
        public DateOnly FechaCierre { get; set; }
        [Required(ErrorMessage = "La hora de cierre es obligatoria")]
        public TimeOnly HoraCierre { get; set;  }

        [Required(ErrorMessage = "El lugar es obligatorio")]
        public string Lugar { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; } = string.Empty;

        public string Estado { get; set; } = "Pendiente";

        public bool Eliminado { get; set; } = false;
        public double Total { get; set; }

        public int Estrella { get; set; }
        public List<EventoDetalles> EventosDetalles { get; set; } = new();

        //[InverseProperty("Evento")]
        //public virtual ICollection<EventosDetalles> EventosDetalles { get; set; } = new List<EventosDetalles>();

        public Evento()
        {

        }

    }
}
