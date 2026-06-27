using System;

namespace DulceCaprichoConsola
{
    public class Pedido
    {
        public string IdPedido { get; set; }
        public string DniCliente { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaEntrega { get; set; } 
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public decimal CostoProduccion { get; set; }
        public decimal PrecioVenta { get; set; }

    }
}