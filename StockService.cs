using System;
using System.Collections.Generic;
using System.IO;

namespace DulceCaprichoConsola
{

    public class StockService
    {
        private string archivoInventario = "inventario.txt";

        public StockService()
        {
            if (!File.Exists(archivoInventario))
            {
                File.Create(archivoInventario).Close();
            }
        }

        public List<InsumoStock> CargarInventario()
        {
            List<InsumoStock> lista = new List<InsumoStock>();
            string[] lineas = File.ReadAllLines(archivoInventario);

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');
                if (datos.Length == 3)
                {
                    InsumoStock insumo = new InsumoStock();
                    insumo.Nombre = datos[0];
                    insumo.Cantidad = decimal.Parse(datos[1]);
                    insumo.Unidad = datos[2];
                    lista.Add(insumo);
                }
            }
            return lista;
        }

        public void GuardarInventario(List<InsumoStock> lista)
        {
            using (StreamWriter sw = new StreamWriter(archivoInventario, false))
            {
                foreach (var item in lista)
                {
                    sw.WriteLine($"{item.Nombre}|{item.Cantidad}|{item.Unidad}");
                }
            }
        }

        public bool ValidarYDescontarStock(List<Receta> formula, int cantidadPedido, out string mensaje)
        {
            mensaje = "";
            List<InsumoStock> inventario = CargarInventario();

            foreach (Receta ing in formula)
            {
                decimal necesario = ing.Cantidad * cantidadPedido;
                InsumoStock almacenado = inventario.Find(i => i.Nombre.Equals(ing.SubProducto, StringComparison.OrdinalIgnoreCase));

                if (almacenado == null || almacenado.Cantidad < necesario)
                {
                    decimal disponible = almacenado != null ? almacenado.Cantidad : 0;
                    mensaje = $"Falta stock de '{ing.SubProducto}'. Requiere {necesario} y hay {disponible} {ing.UnidadMedida}.";
                    return false;
                }
            }

            foreach (Receta ing in formula)
            {
                decimal necesario = ing.Cantidad * cantidadPedido;
                InsumoStock almacenado = inventario.Find(i => i.Nombre.Equals(ing.SubProducto, StringComparison.OrdinalIgnoreCase));
                almacenado.Cantidad -= necesario;
            }

            GuardarInventario(inventario);
            return true;
        }

        public void RegistrarOActualizarInsumo(string nombre, decimal cantidad, string unidad)
        {
            List<InsumoStock> inventario = CargarInventario();

            
            InsumoStock existente = inventario.Find(i => i.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

            if (existente != null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                InsumoStock nuevo = new InsumoStock
                {
                    Nombre = nombre,
                    Cantidad = cantidad,
                    Unidad = unidad
                };
                inventario.Add(nuevo);
            }

            GuardarInventario(inventario);
        }



    }
}