using System;
using System.Collections.Generic;

namespace DulceCaprichoConsola
{
    class Program
    {
        static ClienteService clienteService = new ClienteService();
        static PedidoService pedidoService = new PedidoService();
        static ProductoService productoService = new ProductoService();
        static RecetaService recetaService = new RecetaService();
        static StockService stockService = new StockService();

        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenido al sistema de pedidos de Dulce Capricho");
            int opcion = 0;

            do
            {
                Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
                Console.WriteLine("1. Toma de Pedidos");
                Console.WriteLine("2. Cronograma de Pedidos");
                Console.WriteLine("3. Gestión de Productos y Recetas");
                Console.WriteLine("4. Reporte de Insumos Actuales (Almacén)");
                Console.WriteLine("5. Salir");
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
                            ModuloGestionProductos();
                            break;
                        case 4:
                            ReporteInsumosActuales();
                            break;
                        case 5:
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
            } while (opcion != 5);
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

            Console.WriteLine("\n--- Productos Disponibles ---");
            List<Producto> productos = productoService.ObtenerProductos();
            foreach (var p in productos)
            {
                Console.WriteLine($"[{p.IdProducto}] {p.Nombre} - Venta: S/. {p.PrecioPromedio}");
            }

            Console.Write("\nIngrese el ID del Producto: ");
            string idProd = Console.ReadLine().Trim();
            Producto prodSeleccionado = productoService.BuscarProductoPorId(idProd);

            if (prodSeleccionado == null)
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            nuevoPedido.Producto = prodSeleccionado.Nombre;

            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) cantidad = 1;
            nuevoPedido.Cantidad = cantidad;

            List<Receta> formula = recetaService.ObtenerRecetaPorProducto(prodSeleccionado.IdProducto);

            if (formula == null || formula.Count == 0)
            {
                Console.WriteLine($"\n[ERROR] El producto '{prodSeleccionado.Nombre}' no tiene insumos en su receta.");
                return;
            }

            string mensajeStock;
            if (!stockService.ValidarYDescontarStock(formula, nuevoPedido.Cantidad, out mensajeStock))
            {
                Console.WriteLine($"\n[ERROR DE INVENTARIO] {mensajeStock}");
                return;
            }

            nuevoPedido.CostoProduccion = prodSeleccionado.Costo;
            nuevoPedido.PrecioVenta = prodSeleccionado.PrecioPromedio;

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
            Console.WriteLine("\nPedido registrado correctamente en el sistema.");
        }

        static void ModuloCronogramaPedidos()
        {
            int opcion = 0;
            do
            {
                Console.WriteLine("\n=== MÓDULO 2: CRONOGRAMA DE PEDIDOS ===");
                Console.WriteLine("1. Buscar Pedido");
                Console.WriteLine("2. Pedidos Pendientes");
                Console.WriteLine("3. Reporte Financiero de Ventas");
                Console.WriteLine("4. Volver");
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
                            Console.Write("\n¿De cuántos días atrás deseas el reporte financiero? (ej: 7): ");
                            if (int.TryParse(Console.ReadLine(), out int dias))
                            {
                                pedidoService.GenerarReporteUltimosDias(dias);
                            }
                            else
                            {
                                pedidoService.GenerarReporteUltimosDias(7);
                            }
                            break;
                        case 4:
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            } while (opcion != 4);
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

            if (string.IsNullOrEmpty(idPedido) && string.IsNullOrEmpty(dniCliente))
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

        static void ModuloGestionProductos()
        {
            int opcion = 0;
            do
            {
                Console.WriteLine("\n=== MÓDULO 3: GESTIÓN DE PRODUCTOS ===");
                Console.WriteLine("1. Registrar Nuevo Producto");
                Console.WriteLine("2. Ver Catálogo de Productos");
                Console.WriteLine("3. Editar Producto");
                Console.WriteLine("4. Eliminar Producto");
                Console.WriteLine("5. Gestionar Receta de Producto");
                Console.WriteLine("6. Registrar Insumo / Stock Inicial");
                Console.WriteLine("7. Volver");
                Console.Write("Seleccione una opción: ");

                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            RegistrarProductoNuevo();
                            break;
                        case 2:
                            MostrarCatalogoProductos();
                            break;
                        case 3:
                            EditarProducto();
                            break;
                        case 4:
                            EliminarProducto();
                            break;
                        case 5:
                            SubModuloRecetas();
                            break;
                        case 6:
                            RegistrarStockInicial();
                            break;
                        case 7:
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            } while (opcion != 7);
        }

        static void RegistrarProductoNuevo()
        {
            Console.WriteLine("\n--- Registro de Producto Nuevo ---");
            Producto nuevoProd = new Producto();

            string nombre;
            do
            {
                Console.Write("Nombre del Producto: ");
                nombre = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    Console.WriteLine("El nombre no puede estar vacío.");
                }
            } while (string.IsNullOrEmpty(nombre));
            nuevoProd.Nombre = nombre;

            decimal precioVenta;
            while (true)
            {
                Console.Write("Precio Promedio de Venta (ej: 45.50): ");
                if (decimal.TryParse(Console.ReadLine(), out precioVenta) && precioVenta >= 0)
                {
                    break;
                }
                Console.WriteLine("Por favor, ingrese un monto numérico válido y mayor o igual a cero.");
            }
            nuevoProd.PrecioPromedio = precioVenta;

            decimal costo;
            while (true)
            {
                Console.Write("Costo de Producción (Insumos): ");
                if (decimal.TryParse(Console.ReadLine(), out costo) && costo >= 0)
                {
                    break;
                }
                Console.WriteLine("Por favor, ingrese un monto numérico válido y mayor o igual a cero.");
            }
            nuevoProd.Costo = costo;

            double tiempo;
            while (true)
            {
                Console.Write("Tiempo Estimado (en horas, ej: 1.5): ");
                if (double.TryParse(Console.ReadLine(), out tiempo) && tiempo > 0)
                {
                    break;
                }
                Console.WriteLine("Por favor, ingrese un tiempo válido en horas (mayor a cero).");
            }
            nuevoProd.TiempoEstimado = tiempo;

            productoService.RegistrarProducto(nuevoProd);
            Console.WriteLine("\nProducto guardado exitosamente sin errores.");
        }

        static void MostrarCatalogoProductos()
        {
            Console.WriteLine("\n--- Catálogo de Productos Registrados ---");
            List<Producto> lista = productoService.ObtenerProductos();

            if (lista.Count > 0)
            {
                foreach (Producto p in lista)
                {
                    Console.WriteLine("-------------------------------------");
                    Console.WriteLine($"ID Producto : {p.IdProducto}");
                    Console.WriteLine($"Nombre      : {p.Nombre}");
                    Console.WriteLine($"Precio Venta: S/. {p.PrecioPromedio}");
                    Console.WriteLine($"Costo       : S/. {p.Costo}");
                    Console.WriteLine($"Tiempo Prep.: {p.TiempoEstimado} hrs");
                    Console.WriteLine("Receta Asociada:");
                    if (p.Ingredientes.Count > 0)
                    {
                        foreach (var ing in p.Ingredientes)
                        {
                            Console.WriteLine($"  * {ing.SubProducto}: {ing.Cantidad} {ing.UnidadMedida}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  (Sin ingredientes registrados)");
                    }
                }
                Console.WriteLine("-------------------------------------");
            }
            else
            {
                Console.WriteLine("No hay productos registrados todavía.");
            }
        }

        static void EditarProducto()
        {
            Console.WriteLine("\n--- Editar Producto ---");
            Console.Write("Ingrese el ID del producto a editar: ");
            string id = Console.ReadLine();

            List<Producto> productos = productoService.ObtenerProductos();
            Producto prodAEditar = null;

            foreach (Producto p in productos)
            {
                if (p.IdProducto == id)
                {
                    prodAEditar = p;
                    break;
                }
            }

            if (prodAEditar != null)
            {
                Console.WriteLine($"\nProducto encontrado: {prodAEditar.Nombre}");

                Console.Write("Nuevo Nombre (dejar en blanco para no cambiar): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) prodAEditar.Nombre = nuevoNombre;

                Console.Write("Nuevo Precio Venta (dejar en blanco para no cambiar): ");
                string nuevoPrecio = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoPrecio)) prodAEditar.PrecioPromedio = decimal.Parse(nuevoPrecio);

                Console.Write("Nuevo Costo (dejar en blanco para no cambiar): ");
                string nuevoCosto = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoCosto)) prodAEditar.Costo = decimal.Parse(nuevoCosto);

                Console.Write("Nuevo Tiempo Estimado en horas (ej: 0.5 / dejar en blanco para no cambiar): ");
                string nuevoTiempo = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTiempo)) prodAEditar.TiempoEstimado = double.Parse(nuevoTiempo);

                productoService.ActualizarArchivo(productos);
                Console.WriteLine("\nProducto actualizado correctamente.");
            }
            else
            {
                Console.WriteLine("\nProducto no encontrado.");
            }
        }

        static void EliminarProducto()
        {
            Console.WriteLine("\n--- Eliminar Producto ---");
            Console.Write("Ingrese el ID del producto a eliminar: ");
            string id = Console.ReadLine();

            if (productoService.EliminarProducto(id))
            {
                Console.WriteLine("\nProducto eliminado exitosamente del archivo.");
            }
            else
            {
                Console.WriteLine("\nNo se encontró ningún producto con ese ID.");
            }
        }

        static void SubModuloRecetas()
        {
            int opcion = 0;
            do
            {
                Console.WriteLine("\n=== SUB-MÓDULO: RECETAS DE PRODUCTOS ===");
                Console.WriteLine("1. Agregar Insumo a Receta");
                Console.WriteLine("2. Consultar Receta de un Producto");
                Console.WriteLine("3. Eliminar Receta Completa");
                Console.WriteLine("4. Volver al Menú de Productos");
                Console.Write("Seleccione una opción: ");

                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            Console.Write("Ingrese el ID del Producto para su receta: ");
                            string idProd = Console.ReadLine().Trim();
                            Producto p = productoService.BuscarProductoPorId(idProd);
                            if (p == null)
                            {
                                Console.WriteLine("\nEl producto no existe. Regístrelo primero en el catálogo.");
                            }
                            else
                            {
                                string continuar = "s";
                                do
                                {
                                    AgregarInsumoAReceta(idProd);
                                    Console.Write("\n¿Desea agregar otro insumo a este producto? (s/n): ");
                                    continuar = Console.ReadLine().ToLower();
                                } while (continuar == "s");
                            }
                            break;
                        case 2:
                            ConsultarRecetaProducto();
                            break;
                        case 3:
                            EliminarRecetaProducto();
                            break;
                        case 4:
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            } while (opcion != 4);
        }

        static void AgregarInsumoAReceta(string idProducto)
        {
            List<InsumoStock> listaInsumos = stockService.CargarInventario();
            InsumoStock insumoSeleccionado = null;

            while (insumoSeleccionado == null)
            {
                Console.WriteLine("\nInsumos Disponibles en Almacén:");
                for (int i = 0; i < listaInsumos.Count; i++)
                {
                    Console.WriteLine($"[{i + 1}] {listaInsumos[i].Nombre} ({listaInsumos[i].Unidad})");
                }
                Console.WriteLine($"[{listaInsumos.Count + 1}] -> REGISTRAR NUEVO INSUMO / STOCK INICIAL <-");

                Console.Write("\nSeleccione una opción: ");
                if (int.TryParse(Console.ReadLine(), out int opcion) && opcion > 0 && opcion <= listaInsumos.Count + 1)
                {
                    if (opcion == listaInsumos.Count + 1)
                    {
                        RegistrarStockInicial();
                        listaInsumos = stockService.CargarInventario();
                        continue;
                    }
                    else
                    {
                        insumoSeleccionado = listaInsumos[opcion - 1];
                    }
                }
                else
                {
                    Console.WriteLine("Por favor, seleccione una opción correcta de la lista.");
                }
            }

            decimal cantidadInsumo;
            while (true)
            {
                Console.Write($"Cantidad de '{insumoSeleccionado.Nombre}' necesaria para 1 unidad del producto (en {insumoSeleccionado.Unidad}): ");
                if (decimal.TryParse(Console.ReadLine(), out cantidadInsumo) && cantidadInsumo > 0)
                {
                    break;
                }
                Console.WriteLine("Ingrese una cantidad numérica válida y mayor a cero.");
            }

            Receta nuevoInsumoReceta = new Receta
            {
                IdProducto = idProducto,
                SubProducto = insumoSeleccionado.Nombre,
                Cantidad = cantidadInsumo,
                UnidadMedida = insumoSeleccionado.Unidad
            };

            recetaService.RegistrarIngrediente(nuevoInsumoReceta);
            Console.WriteLine($"\n'{insumoSeleccionado.Nombre}' agregado con éxito a la receta del producto.");
        }

        static void ConsultarRecetaProducto()
        {
            Console.WriteLine("\n--- Consultar Receta ---");
            Console.Write("Ingrese el ID del Producto: ");
            string idProd = Console.ReadLine();

            Producto p = productoService.BuscarProductoPorId(idProd);
            string nombreProd = p != null ? p.Nombre : "Producto No Encontrado o Eliminado";

            List<Receta> ingredientes = recetaService.ObtenerRecetaPorProducto(idProd);

            Console.WriteLine($"\nReceta para: {nombreProd} (ID: {idProd})");
            if (ingredientes.Count > 0)
            {
                Console.WriteLine("-------------------------------------");
                foreach (Receta r in ingredientes)
                {
                    Console.WriteLine($"- Insumo: {r.SubProducto} | Cantidad: {r.Cantidad} {r.UnidadMedida}");
                }
                Console.WriteLine("-------------------------------------");
            }
            else
            {
                Console.WriteLine("Este producto aún no tiene insumos registrados en su receta.");
            }
        }

        static void EmptyRecetaProducto() { }

        static void EliminarRecetaProducto()
        {
            Console.WriteLine("\n--- Eliminar Receta de Producto ---");
            Console.Write("Ingrese el ID del Producto para borrar su receta: ");
            string idProd = Console.ReadLine();

            if (recetaService.EliminarRecetaDeProducto(idProd))
            {
                Console.WriteLine("\nReceta eliminada exitosamente del archivo recetas.txt.");
            }
            else
            {
                Console.WriteLine("\nNo se puede eliminar la receta porque el producto todavía existe en el catálogo.");
            }
        }

        static void ReporteInsumosActuales()
        {
            Console.WriteLine("\n=============================================");
            Console.WriteLine("        REPORTE DE INSUMOS ACTUALES (ALMACÉN) ");
            Console.WriteLine("=============================================");
            Console.WriteLine($"{"Insumo",-20} | {"Stock Disponible",-18} | {"Unidad",-10}");
            Console.WriteLine("---------------------------------------------");

            List<InsumoStock> inventario = stockService.CargarInventario();

            foreach (var insumo in inventario)
            {
                string alerta = insumo.Cantidad < 5 ? " -> ¡STOCK BAJO!" : "";
                Console.WriteLine($"{insumo.Nombre,-20} | {insumo.Cantidad,-18:N2} | {insumo.Unidad,-10}{alerta}");
            }
            Console.WriteLine("=============================================");
        }

        static void RegistrarStockInicial()
        {
            Console.WriteLine("\n--- Registro de Stock Inicial ---");

            Console.Write("Nombre del Insumo (ej: Harina): ");
            string nombre = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("El nombre del insumo no puede estar vacío.");
                return;
            }

            decimal cantidad;
            while (true)
            {
                Console.Write("Cantidad Inicial: ");
                if (decimal.TryParse(Console.ReadLine(), out cantidad) && cantidad >= 0)
                {
                    break;
                }
                Console.WriteLine("Por favor, ingrese una cantidad numérica válida y mayor o igual a cero.");
            }

            Console.Write("Unidad de Medida (ej: Kg, Litros, Unidades): ");
            string unidad = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(unidad)) unidad = "Und";

            stockService.RegistrarOActualizarInsumo(nombre, cantidad, unidad);
            Console.WriteLine($"\nStock inicial de {nombre} registrado correctamente.");
        }
    }
}