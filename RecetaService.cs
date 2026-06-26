using System;
using System.Collections.Generic;
using System.IO;

namespace DulceCaprichoConsola
{
    public class RecetaService
    {
        private string archivoRecetas = "recetas.txt";
        private ProductoService productoService = new ProductoService();

        public RecetaService()
        {
            if (!File.Exists(archivoRecetas))
            {
                File.Create(archivoRecetas).Close();
            }
        }

        public List<Receta> ObtenerTodasLasRecetas()
        {
            List<Receta> lista = new List<Receta>();
            string[] lineas = File.ReadAllLines(archivoRecetas);

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');
                if (datos.Length == 4)
                {
                    Receta rec = new Receta();
                    rec.IdProducto = datos[0];
                    rec.SubProducto = datos[1];
                    rec.Cantidad = decimal.Parse(datos[2]);
                    rec.UnidadMedida = datos[3];

                    lista.Add(rec);
                }
            }
            return lista;
        }

        public List<Receta> ObtenerRecetaPorProducto(string idProducto)
        {
            List<Receta> todas = ObtenerTodasLasRecetas();
            List<Receta> filtrada = new List<Receta>();

            foreach (Receta r in todas)
            {
                if (r.IdProducto == idProducto)
                {
                    filtrada.Add(r);
                }
            }
            return filtrada;
        }

        public void RegistrarIngrediente(Receta nuevoIngrediente)
        {
            string linea = $"{nuevoIngrediente.IdProducto}|{nuevoIngrediente.SubProducto}|{nuevoIngrediente.Cantidad}|{nuevoIngrediente.UnidadMedida}";

            using (StreamWriter sw = File.AppendText(archivoRecetas))
            {
                sw.WriteLine(linea);
            }
        }

        public bool EliminarRecetaDeProducto(string idProducto)
        {
            // VALIDACIÓN: Verificamos si el producto todavía existe en el archivo producto.txt
            Producto productoExistente = productoService.BuscarProductoPorId(idProducto);

            // Si el producto existe, la regla dice que NO se puede eliminar la receta
            if (productoExistente != null)
            {
                return false;
            }

            // Si el producto ya no existe, procedemos a limpiar sus ingredientes
            List<Receta> todas = ObtenerTodasLasRecetas();

            using (StreamWriter sw = new StreamWriter(archivoRecetas, false))
            {
                foreach (Receta r in todas)
                {
                    if (r.IdProducto != idProducto)
                    {
                        string linea = $"{r.IdProducto}|{r.SubProducto}|{r.Cantidad}|{r.UnidadMedida}";
                        sw.WriteLine(linea);
                    }
                }
            }
            return true;
        }
    }
}