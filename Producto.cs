using System;

namespace DulceCaprichoConsola
{
    public class Producto
    {
        public string IdProducto { get; set; } 
        public string Nombre { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal Costo { get; set; }
        public double TiempoEstimado { get; set; }
        public List<Receta> Ingredientes { get; set; } = new List<Receta>();
    }
}