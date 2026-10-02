namespace OurSpot.Components.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public string Estado { get; set; }
        public string Proveedor { get; set; }

        public string Tipo { get; set; }

        public Servicio(int id, string nombre, string descripcion, decimal costo, string estado, string proveedor, string tipo)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Costo = costo;
            Estado = estado;
            Proveedor = proveedor;
            Tipo = tipo;
        }

        public Servicio()
        {

        }
    }
}
