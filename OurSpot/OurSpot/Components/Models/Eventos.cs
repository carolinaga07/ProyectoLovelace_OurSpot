using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace OurSpot.Components.Models
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


        //1, "Boda de los Perez", "Social", new DateOnly(2026, 12, 2), "Salon Palma Real", "Privado", "Pendiente"
        public Evento()
        {

        }
        public Evento(int id, string nombre, string categoria, DateTime fecha, string lugar, string tipo, string estado)
        {
            EventoId = id;
            Nombre = nombre;
            Categoria = categoria;
            Fecha = fecha;
            Lugar = lugar;
            Tipo = tipo;
            Estado = estado;
        }

    }
}
