using System;
using System.Collections.Generic;
using System.IO;

namespace DulceCaprichoConsola
{
    public class PedidoService
    {
        private string archivoPedidos = "pedidos.txt";

        public PedidoService()
        {
            if (!File.Exists(archivoPedidos))
            {
                File.Create(archivoPedidos).Close();
            }
        }

        public List<Pedido> ObtenerPedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();
            string[] lineas = File.ReadAllLines(archivoPedidos);

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');

                if (datos.Length == 10)
                {
                    Pedido pedido = new Pedido();
                    pedido.IdPedido = datos[0];
                    pedido.DniCliente = datos[1];
                    pedido.Producto = datos[2];
                    pedido.Cantidad = int.Parse(datos[3]);
                    pedido.FechaRegistro = DateTime.Parse(datos[4]);
                    pedido.FechaEntrega = DateTime.Parse(datos[5]);
                    pedido.Observaciones = datos[6];
                    pedido.Estado = datos[7];
                    pedido.CostoProduccion = decimal.Parse(datos[8]);
                    pedido.PrecioVenta = decimal.Parse(datos[9]);

                    pedidos.Add(pedido);
                }
            }
            return pedidos; 
        }




        private string GenerarNuevoId()
        {
            List<Pedido> pedidos = ObtenerPedidos();
            if (pedidos.Count == 0)
            {
                return "PED-0001";
            }

            // Obtener el último id
            string ultimoId = pedidos[pedidos.Count - 1].IdPedido; 
            string numeroStr = ultimoId.Substring(4); 
            int numero = int.Parse(numeroStr);
            numero++; 

            // Formatea con ceros a la izquierda
            string nuevoNumeroStr = numero.ToString();
            while (nuevoNumeroStr.Length < 4)
            {
                nuevoNumeroStr = "0" + nuevoNumeroStr;
            }

            return "PED-" + nuevoNumeroStr;
        }

        public void RegistrarPedido(Pedido nuevoPedido)
        {
            nuevoPedido.IdPedido = GenerarNuevoId();
            nuevoPedido.Estado = "Pendiente";
            nuevoPedido.FechaRegistro = DateTime.Now;

            string linea = $"{nuevoPedido.IdPedido}|{nuevoPedido.DniCliente}|{nuevoPedido.Producto}|{nuevoPedido.Cantidad}|{nuevoPedido.FechaRegistro:yyyy-MM-dd HH:mm:ss}|{nuevoPedido.FechaEntrega:yyyy-MM-dd}|{nuevoPedido.Observaciones}|{nuevoPedido.Estado}|{nuevoPedido.CostoProduccion}|{nuevoPedido.PrecioVenta}";

            using (StreamWriter sw = File.AppendText(archivoPedidos))
            {
                sw.WriteLine(linea);
            }
        }

        public void ActualizarArchivo(List<Pedido> pedidos)
        {
            using (StreamWriter sw = new StreamWriter(archivoPedidos, false))
            {
                foreach (Pedido p in pedidos)
                {
                    string linea = $"{p.IdPedido}|{p.DniCliente}|{p.Producto}|{p.Cantidad}|{p.FechaRegistro:yyyy-MM-dd HH:mm:ss}|{p.FechaEntrega:yyyy-MM-dd}|{p.Observaciones}|{p.Estado}|{p.CostoProduccion}|{p.PrecioVenta}";
                    sw.WriteLine(linea);
                }
            }
        }

        public bool CambiarEstadoPedido(string idPedido)
        {
            List<Pedido> pedidos = ObtenerPedidos();
            bool encontrado = false;

            foreach (Pedido p in pedidos)
            {
                if (p.IdPedido == idPedido)
                {
                    encontrado = true;
                    if (p.Estado == "Pendiente")
                    {
                        p.Estado = "Entregado";
                    }
                    else
                    {
                        p.Estado = "Pendiente";
                    }
                    break;
                }
            }

            if (encontrado)
            {
                ActualizarArchivo(pedidos);
                return true;
            }

            return false;
        }

        public void GenerarReporteUltimosDias(int diasAtras)
        {
            List<Pedido> todosLosPedidos = ObtenerPedidos();
            DateTime fechaLimite = DateTime.Now.Date.AddDays(-diasAtras);

            decimal ingresosTotales = 0;
            decimal costosTotales = 0;
            int totalEntregados = 0;

            Console.WriteLine($"\n=================================================================");
            Console.WriteLine($"   REPORTE DE RENDIMIENTO FINANCIERO (ÚLTIMOS {diasAtras} DÍAS)");
            Console.WriteLine($"=================================================================");
            Console.WriteLine($"{"ID", -9} {"Fecha Reg.", -12} {"Producto", -18} {"Cant.", -6} {"Venta T.", -10} {"Ganancia", -10}");
            Console.WriteLine($"-----------------------------------------------------------------");

            foreach (Pedido p in todosLosPedidos)
            {
                // Filtramos que la fecha de registro esté dentro del rango de los últimos X días
                if (p.FechaRegistro.Date >= fechaLimite)
                {
                    decimal ventaTotal = p.PrecioVenta * p.Cantidad;
                    decimal costoTotal = p.CostoProduccion * p.Cantidad;
                    decimal gananciaNeto = ventaTotal - costoTotal;

                    if (p.Estado != "Cancelado")
                    {
                        ingresosTotales += ventaTotal;
                        costosTotales += costoTotal;
                        if (p.Estado == "Entregado") totalEntregados++;
                    }

                    Console.WriteLine($"{p.IdPedido,-9} {p.FechaRegistro:dd/MM/yyyy,-12} {p.Producto,-18} {p.Cantidad,-6} S/. {ventaTotal,-8:N2} S/. {gananciaNeto,-8:N2}");
                }
            }

            Console.WriteLine($"-----------------------------------------------------------------");
            Console.WriteLine($" Rendimiento Neto del Período:");
            Console.WriteLine($"   * Pedidos Completados (Entregados): {totalEntregados}");
            Console.WriteLine($"   * Total Facturado (Ventas)       : S/. {ingresosTotales:N2}");
            Console.WriteLine($"   * Inversión en Insumos (Costos)  : S/. {costosTotales:N2}");
            Console.WriteLine($"   * GANANCIA NETA TOTAL            : S/. {(ingresosTotales - costosTotales):N2}");
            Console.WriteLine($"=================================================================");
        }





    }
}
