namespace OurSpot.Components.Models
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

    }
}
