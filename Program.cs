using System;
using System.Collections.Generic;

namespace DulceCaprichoConsola
{
    class Program
    {
        static ClienteService clienteService = new ClienteService();
        static PedidoService pedidoService = new PedidoService();

        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenido al sistema de pedidos de Dulce Capricho");
            int opcion = 0;

            do
            {
                Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
                Console.WriteLine("1. Toma de Pedidos");
                Console.WriteLine("2. Cronograma de Pedidos");
                Console.WriteLine("3. Salir");
                Console.Write("Seleccione una opción: ");
                
                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            ModuloTomaPedidos();
                            break;
                        case 2:
                            ModuloCronogramaPedidos();
                            break;
                        case 3:
                            Console.WriteLine("Saliendo del sistema...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Por favor, ingrese un número válido.");
                }
            } while (opcion != 3);
        }

        static void ModuloTomaPedidos()
        {
            int opcion = 0;
            do
            {
                Console.WriteLine("\n=== MÓDULO 1: TOMA DE PEDIDOS ===");
                Console.WriteLine("1. Cliente Registrado");
                Console.WriteLine("2. Cliente Nuevo");
                Console.WriteLine("3. Volver");
                Console.Write("Seleccione una opción: ");
                
                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            ProcesarClienteRegistrado();
                            break;
                        case 2:
                            ProcesarClienteNuevo();
                            break;
                        case 3:
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            } while (opcion != 3);
        }

        static void ProcesarClienteRegistrado()
        {
            Console.Write("\nPor favor, ingrese el DNI del cliente: ");
            string dni = Console.ReadLine();

            Cliente cliente = clienteService.BuscarClientePorDni(dni);
            if (cliente != null)
            {
                Console.WriteLine($"Cliente encontrado: {cliente.Nombres} {cliente.Apellidos}");
                RegistrarPedido(cliente.DNI);
            }
            else
            {
                Console.WriteLine("Cliente no encontrado.");
            }
        }

        static void ProcesarClienteNuevo()
        {
            Console.WriteLine("\n--- Registro de Cliente Nuevo ---");
            Cliente nuevoCliente = new Cliente();

            Console.Write("DNI (8 dígitos): ");
            nuevoCliente.DNI = Console.ReadLine();

            Console.Write("Nombres: ");
            nuevoCliente.Nombres = Console.ReadLine();

            Console.Write("Apellidos: ");
            nuevoCliente.Apellidos = Console.ReadLine();

            Console.Write("Celular: ");
            nuevoCliente.Celular = Console.ReadLine();

            Console.Write("Dirección: ");
            nuevoCliente.Direccion = Console.ReadLine();

            string mensajeError;
            bool exito = clienteService.RegistrarCliente(nuevoCliente, out mensajeError);

            if (exito)
            {
                Console.WriteLine("Cliente registrado correctamente.");
                RegistrarPedido(nuevoCliente.DNI);
            }
            else
            {
                Console.WriteLine($"Error: {mensajeError}");
            }
        }

        static void RegistrarPedido(string dniCliente)
        {
            Console.WriteLine("\nAhora registraremos el pedido.");
            Pedido nuevoPedido = new Pedido();
            nuevoPedido.DniCliente = dniCliente;

            Console.Write("Producto: ");
            nuevoPedido.Producto = Console.ReadLine();

            Console.Write("Cantidad: ");
            string cantStr = Console.ReadLine();
            int cantidad;
            if (int.TryParse(cantStr, out cantidad))
            {
                nuevoPedido.Cantidad = cantidad;
            }
            else
            {
                nuevoPedido.Cantidad = 1;
            }

            Console.Write("Fecha de Entrega (YYYY-MM-DD): ");
            string fechaStr = Console.ReadLine();
            DateTime fechaEntrega;
            if (DateTime.TryParse(fechaStr, out fechaEntrega))
            {
                nuevoPedido.FechaEntrega = fechaEntrega;
            }
            else
            {
                nuevoPedido.FechaEntrega = DateTime.Now.AddDays(1); 
            }

            Console.Write("Observaciones: ");
            nuevoPedido.Observaciones = Console.ReadLine();
            
            if (string.IsNullOrEmpty(nuevoPedido.Observaciones))
            {
                nuevoPedido.Observaciones = "Ninguna";
            }

            pedidoService.RegistrarPedido(nuevoPedido);
            Console.WriteLine("\nPedido registrado correctamente.");
        }

        static void ModuloCronogramaPedidos()
        {
            int opcion = 0;
            do
            {
                Console.WriteLine("\n=== MÓDULO 2: CRONOGRAMA DE PEDIDOS ===");
                Console.WriteLine("1. Buscar Pedido");
                Console.WriteLine("2. Pedidos Pendientes");
                Console.WriteLine("3. Volver");
                Console.Write("Seleccione una opción: ");
                
                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            BuscarPedido();
                            break;
                        case 2:
                            MostrarPedidosPendientes();
                            break;
                        case 3:
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            } while (opcion != 3);
        }

        static void BuscarPedido()
        {
            Console.WriteLine("\n--- Buscar Pedido ---");
            Console.Write("Ingrese ID del Pedido (dejar en blanco si no lo sabe): ");
            string idPedido = Console.ReadLine();

            Console.Write("Ingrese DNI del Cliente (dejar en blanco si no lo sabe): ");
            string dniCliente = Console.ReadLine();

            Console.Write("Ingrese Fecha de Entrega (YYYY-MM-DD) opcional: ");
            string fechaStr = Console.ReadLine();

            if (string.IsNullOrEmpty(idPedido) && string.IsNullOrEmpty(dniCliente))//
            {
                Console.WriteLine("\nDebe ingresar un ID de pedido o DNI del cliente.");
                return;
            }

            List<Pedido> todosLosPedidos = pedidoService.ObtenerPedidos();
            List<Pedido> resultados = new List<Pedido>();

            foreach (Pedido p in todosLosPedidos)
            {
                bool coincideId = string.IsNullOrEmpty(idPedido) || p.IdPedido == idPedido;
                bool coincideDni = string.IsNullOrEmpty(dniCliente) || p.DniCliente == dniCliente;
                
                bool coincideFecha = true;
                if (!string.IsNullOrEmpty(fechaStr))
                {
                    DateTime fechaFiltro;
                    if (DateTime.TryParse(fechaStr, out fechaFiltro))
                    {
                        if (p.FechaEntrega.Date != fechaFiltro.Date)
                        {
                            coincideFecha = false;
                        }
                    }
                }

                if (coincideId && coincideDni && coincideFecha)
                {
                    resultados.Add(p);
                }
            }

            if (resultados.Count > 0)
            {
                foreach (Pedido res in resultados)
                {
                    Console.WriteLine("\n-------------------------");
                    Console.WriteLine($"Código Pedido: {res.IdPedido}");
                    Console.WriteLine($"Cliente (DNI): {res.DniCliente}");
                    Console.WriteLine($"Producto: {res.Producto}");
                    Console.WriteLine($"Cantidad: {res.Cantidad}");
                    Console.WriteLine($"Fecha Entrega: {res.FechaEntrega:dd/MM/yyyy}");
                    Console.WriteLine($"Estado: {res.Estado}");
                    Console.WriteLine("-------------------------");

                    Console.WriteLine("Detalle del Pedido seleccionado:");
                    Console.WriteLine($"Observaciones: {res.Observaciones}");
                    Console.WriteLine($"Fecha Registro: {res.FechaRegistro}");
                    
                    Console.WriteLine("\n¿Desea cambiar el estado de este pedido?");
                    Console.WriteLine("1. Cambiar Estado");
                    Console.WriteLine("2. Volver");
                    Console.Write("Seleccione opción: ");
                    string opcEstado = Console.ReadLine();

                    if (opcEstado == "1")
                    {
                        if (pedidoService.CambiarEstadoPedido(res.IdPedido))
                        {
                            Console.WriteLine("Estado actualizado correctamente.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\nNo se encontraron pedidos con los criterios indicados.");
            }
        }

        static void MostrarPedidosPendientes()
        {
            Console.WriteLine("\n--- Pedidos Pendientes (Próximos 6 días) ---");
            List<Pedido> todosLosPedidos = pedidoService.ObtenerPedidos();
            List<Pedido> pendientes = new List<Pedido>();

            DateTime hoy = DateTime.Now.Date;
            DateTime limite = hoy.AddDays(6);

            foreach (Pedido p in todosLosPedidos)
            {
                if (p.Estado == "Pendiente" && p.FechaEntrega.Date >= hoy && p.FechaEntrega.Date <= limite)
                {
                    pendientes.Add(p);
                }
            }

           
            for (int i = 0; i < pendientes.Count - 1; i++)
            {
                for (int j = 0; j < pendientes.Count - 1 - i; j++)
                {
                    if (pendientes[j].FechaEntrega > pendientes[j + 1].FechaEntrega)
                    {
                        Pedido temp = pendientes[j];
                        pendientes[j] = pendientes[j + 1];
                        pendientes[j + 1] = temp;
                    }
                }
            }

            if (pendientes.Count > 0)
            {
                foreach (Pedido p in pendientes)
                {
                    Cliente cliente = clienteService.BuscarClientePorDni(p.DniCliente);
                    string nombreCliente = cliente != null ? cliente.Nombres : "Desconocido";

                    Console.WriteLine($"\n{p.FechaEntrega:dd/MM/yyyy}");
                    Console.WriteLine($"{p.IdPedido}");
                    Console.WriteLine($"Cliente: {nombreCliente}");
                    Console.WriteLine($"Producto: {p.Producto}");
                }
            }
            else
            {
                Console.WriteLine("No hay pedidos pendientes en los próximos 6 días.");
            }
        }
    }
}
