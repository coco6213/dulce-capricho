using System;
using System.Collections.Generic;
using System.IO;

namespace DulceCaprichoConsola
{
    public class ClienteService
    {
        private string archivoClientes = "clientes.txt";

        public ClienteService()
        {
            if (!File.Exists(archivoClientes))
            {
                File.Create(archivoClientes).Close();
            }
        }

        public List<Cliente> ObtenerClientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            string[] lineas = File.ReadAllLines(archivoClientes);

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');
                if (datos.Length == 5)
                {
                    Cliente cliente = new Cliente();
                    cliente.DNI = datos[0];
                    cliente.Nombres = datos[1];
                    cliente.Apellidos = datos[2];
                    cliente.Celular = datos[3];
                    cliente.Direccion = datos[4];
                    clientes.Add(cliente);
                }
            }

            return clientes;
        }

        public Cliente BuscarClientePorDni(string dni)
        {
            List<Cliente> clientes = ObtenerClientes();
            foreach (Cliente c in clientes)
            {
                if (c.DNI == dni)
                {
                    return c;
                }
            }
            return null;
        }

        public bool RegistrarCliente(Cliente nuevoCliente, out string mensajeError)
        {
            mensajeError = "";

            if (string.IsNullOrEmpty(nuevoCliente.DNI) || nuevoCliente.DNI.Length != 8)
            {
                mensajeError = "El DNI es obligatorio y debe tener exactamente 8 dígitos.";
                return false;
            }

            if (string.IsNullOrEmpty(nuevoCliente.Nombres) || string.IsNullOrEmpty(nuevoCliente.Apellidos))
            {
                mensajeError = "Los nombres y apellidos son obligatorios.";
                return false;
            }

            if (string.IsNullOrEmpty(nuevoCliente.Celular))
            {
                mensajeError = "El celular es obligatorio.";
                return false;
            }

            Cliente clienteExistente = BuscarClientePorDni(nuevoCliente.DNI);
            if (clienteExistente != null)
            {
                mensajeError = "El DNI ingresado ya se encuentra registrado.";
                return false;
            }

            string linea = $"{nuevoCliente.DNI}|{nuevoCliente.Nombres}|{nuevoCliente.Apellidos}|{nuevoCliente.Celular}|{nuevoCliente.Direccion}";
            using (StreamWriter sw = File.AppendText(archivoClientes))
            {
                sw.WriteLine(linea);
            }

            return true;
        }
    }
}
