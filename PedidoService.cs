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
                if (datos.Length == 8)
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
            string ultimoId = pedidos[pedidos.Count - 1].IdPedido; // Ejemplo: PED-0001
            string numeroStr = ultimoId.Substring(4); // Obtiene "0001"
            int numero = int.Parse(numeroStr);
            numero++; // Incrementa a 2

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

            string linea = $"{nuevoPedido.IdPedido}|{nuevoPedido.DniCliente}|{nuevoPedido.Producto}|{nuevoPedido.Cantidad}|{nuevoPedido.FechaRegistro:yyyy-MM-dd HH:mm:ss}|{nuevoPedido.FechaEntrega:yyyy-MM-dd}|{nuevoPedido.Observaciones}|{nuevoPedido.Estado}";
            using (StreamWriter sw = File.AppendText(archivoPedidos))
            {
                sw.WriteLine(linea);
            }
        }

        public void ActualizarArchivo(List<Pedido> pedidos)
        {
            // Sobrescribir todo el archivo con la lista actual
            using (StreamWriter sw = new StreamWriter(archivoPedidos, false))
            {
                foreach (Pedido p in pedidos)
                {
                    string linea = $"{p.IdPedido}|{p.DniCliente}|{p.Producto}|{p.Cantidad}|{p.FechaRegistro:yyyy-MM-dd HH:mm:ss}|{p.FechaEntrega:yyyy-MM-dd}|{p.Observaciones}|{p.Estado}";
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
    }
}
