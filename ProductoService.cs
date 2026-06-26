using System;
using System.Collections.Generic;
using System.IO;

namespace DulceCaprichoConsola
{
    public class ProductoService
    {
        private string archivoProductos = "producto.txt";

        public ProductoService()
        {
            if (!File.Exists(archivoProductos))
            {
                File.Create(archivoProductos).Close();
            }
        }

        public List<Producto> ObtenerProductos()
        {
            List<Producto> productos = new List<Producto>();
            string[] lineas = File.ReadAllLines(archivoProductos);
            RecetaService recetaService = new RecetaService();

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');
                // Cambiado a 5 columnas
                if (datos.Length == 5)
                {
                    Producto prod = new Producto();
                    prod.IdProducto = datos[0];
                    prod.Nombre = datos[1];
                    prod.PrecioPromedio = decimal.Parse(datos[2]);
                    prod.Costo = decimal.Parse(datos[3]);
                    prod.TiempoEstimado = double.Parse(datos[4]);

                    prod.Ingredientes = recetaService.ObtenerRecetaPorProducto(prod.IdProducto);

                    productos.Add(prod);
                }
            }
            return productos;
        }

        private string GenerarNuevoId()
        {
            List<Producto> productos = ObtenerProductos();
            if (productos.Count == 0)
            {
                return "0001";
            }

            string ultimoId = productos[productos.Count - 1].IdProducto;
            int numero = int.Parse(ultimoId);
            numero++;

            string nuevoNumeroStr = numero.ToString();
            while (nuevoNumeroStr.Length < 4)
            {
                nuevoNumeroStr = "0" + nuevoNumeroStr;
            }

            return nuevoNumeroStr;
        }

        public void RegistrarProducto(Producto nuevoProducto)
        {
            nuevoProducto.IdProducto = GenerarNuevoId();

            // Armamos la línea con solo 5 datos
            string linea = $"{nuevoProducto.IdProducto}|{nuevoProducto.Nombre}|{nuevoProducto.PrecioPromedio}|{nuevoProducto.Costo}|{nuevoProducto.TiempoEstimado}";

            using (StreamWriter sw = File.AppendText(archivoProductos))
            {
                sw.WriteLine(linea);
            }
        }

        public Producto BuscarProductoPorId(string idProducto)
        {
            List<Producto> productos = ObtenerProductos();
            foreach (Producto p in productos)
            {
                if (p.IdProducto == idProducto)
                {
                    return p;
                }
            }
            return null;
        }

        public void ActualizarArchivo(List<Producto> productos)
        {
            using (StreamWriter sw = new StreamWriter(archivoProductos, false))
            {
                foreach (Producto p in productos)
                {
                    // Armamos la línea con solo 5 datos para la actualización
                    string linea = $"{p.IdProducto}|{p.Nombre}|{p.PrecioPromedio}|{p.Costo}|{p.TiempoEstimado}";
                    sw.WriteLine(linea);
                }
            }
        }

        public bool EliminarProducto(string idProducto)
        {
            List<Producto> productos = ObtenerProductos();
            bool eliminado = false;

            for (int i = 0; i < productos.Count; i++)
            {
                if (productos[i].IdProducto == idProducto)
                {
                    productos.RemoveAt(i);
                    eliminado = true;
                    break;
                }
            }

            if (eliminado)
            {
                ActualizarArchivo(productos);
                return true;
            }
            return false;
        }
    }
}