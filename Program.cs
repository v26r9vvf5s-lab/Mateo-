// =========================================================
// MINI-POS - Sistema de Punto de Venta e Inventario
// Estudiante: Jhon Mateo Guette Vera
// Unidad 1 - Fundamentos de C# (.NET 8)
// =========================================================
// Restricción de diseño: solo se usan los conceptos vistos
// en la Unidad 1 (variables, List<T>, tuplas, control de
// flujo, métodos estáticos, try/catch y TryParse).
// No se usan clases personalizadas (POO).
// =========================================================

using System.Globalization;

class Program
{
    // Parámetros generales del negocio
    const double PORCENTAJE_IVA = 0.19;
    const double PORCENTAJE_DESCUENTO = 0.08;
    static readonly CultureInfo Moneda = new CultureInfo("es-CO");

    // "Base de datos" en memoria: cada producto es una tupla
    // (Nombre, Precio, Existencias)
    static List<(string Nombre, double Precio, int Existencias)> inventario = new();

    // Historial de ventas del día: (NombreProducto, Cantidad, TotalCobrado)
    static List<(string Producto, int Cantidad, double Total)> ventasDelDia = new();

    static void Main()
    {
        bool continuar = true;

        while (continuar)
        {
            MostrarEncabezado("MINI-POS - GESTIÓN DE VENTAS E INVENTARIO");
            Console.WriteLine(" 1. Agregar producto al inventario");
            Console.WriteLine(" 2. Ver inventario disponible");
            Console.WriteLine(" 3. Realizar una venta");
            Console.WriteLine(" 4. Reporte de ventas del día");
            Console.WriteLine(" 5. Salir del sistema");
            Console.WriteLine(new string('=', 55));

            int opcion = LeerEntero("Elija una opción (1-5): ", 1, 5);

            switch (opcion)
            {
                case 1:
                    AgregarProducto();
                    break;
                case 2:
                    MostrarInventario();
                    break;
                case 3:
                    RealizarVenta();
                    break;
                case 4:
                    MostrarReporteDiario();
                    break;
                case 5:
                    continuar = false;
                    Console.WriteLine("\nGracias por usar Mini-POS. ¡Hasta pronto!");
                    break;
            }

            if (continuar)
            {
                Console.WriteLine("\nPresione ENTER para continuar...");
                Console.ReadLine();
            }
        }
    }

    // ---------------------------------------------------
    // OPCIÓN 1: Registrar un producto nuevo en el inventario
    // ---------------------------------------------------
    static void AgregarProducto()
    {
        MostrarEncabezado("NUEVO PRODUCTO");

        Console.Write("Nombre del producto: ");
        string nombre = Console.ReadLine() ?? "";

        while (string.IsNullOrWhiteSpace(nombre))
        {
            Console.Write("El nombre no puede estar vacío. Intente de nuevo: ");
            nombre = Console.ReadLine() ?? "";
        }

        double precio = LeerDecimal("Precio unitario: $", 0);
        int existencias = LeerEntero("Cantidad inicial en stock: ", 0, 100000);

        inventario.Add((nombre, precio, existencias));

        Console.WriteLine($"\n[OK] '{nombre}' fue agregado al inventario con {existencias} unidades.");
    }

    // ---------------------------------------------------
    // OPCIÓN 2: Mostrar el inventario actual
    // ---------------------------------------------------
    static void MostrarInventario()
    {
        MostrarEncabezado("INVENTARIO ACTUAL");

        if (inventario.Count == 0)
        {
            Console.WriteLine("No hay productos registrados todavía.");
            return;
        }

        Console.WriteLine($"{"#",-3} {"Producto",-20} {"Precio",12} {"Stock",8}  Estado");
        Console.WriteLine(new string('-', 55));

        for (int i = 0; i < inventario.Count; i++)
        {
            var p = inventario[i];
            string alerta = p.Existencias <= 5 ? "¡BAJO STOCK!" : "OK";
            Console.WriteLine($"{i + 1,-3} {p.Nombre,-20} {p.Precio,12:C0} {p.Existencias,8}  {alerta}");
        }
    }

    // ---------------------------------------------------
    // OPCIÓN 3: Vender un producto
    // ---------------------------------------------------
    static void RealizarVenta()
    {
        MostrarEncabezado("REGISTRAR VENTA");

        if (inventario.Count == 0)
        {
            Console.WriteLine("No hay productos en el inventario para vender.");
            return;
        }

        MostrarInventario();
        Console.WriteLine();

        int indice = LeerEntero("Número del producto a vender: ", 1, inventario.Count) - 1;
        var producto = inventario[indice];

        if (producto.Existencias == 0)
        {
            Console.WriteLine($"\n[ERROR] '{producto.Nombre}' no tiene unidades disponibles.");
            return;
        }

        int cantidad = LeerEntero($"Cantidad a vender (disponible: {producto.Existencias}): ", 1, producto.Existencias);

        Console.Write("¿Es cliente frecuente? (S/N): ");
        string respuesta = (Console.ReadLine() ?? "").Trim().ToUpper();
        bool esClienteFrecuente = respuesta == "S";

        // Cálculo de la factura
        double subtotal = producto.Precio * cantidad;
        double descuento = esClienteFrecuente ? subtotal * PORCENTAJE_DESCUENTO : 0;
        double baseGravable = subtotal - descuento;
        double iva = baseGravable * PORCENTAJE_IVA;
        double total = baseGravable + iva;

        // Actualizar inventario
        inventario[indice] = (producto.Nombre, producto.Precio, producto.Existencias - cantidad);

        // Guardar la venta para el reporte del día
        ventasDelDia.Add((producto.Nombre, cantidad, total));

        // Imprimir el ticket
        Console.WriteLine();
        Console.WriteLine(new string('=', 55));
        Console.WriteLine("                   TICKET DE VENTA");
        Console.WriteLine(new string('=', 55));
        Console.WriteLine($" Producto:        {producto.Nombre} (x{cantidad})");
        Console.WriteLine($" Subtotal:        {subtotal,15:C0}");
        if (esClienteFrecuente)
            Console.WriteLine($" Descuento (8%): -{descuento,15:C0}");
        Console.WriteLine($" IVA (19%):       {iva,15:C0}");
        Console.WriteLine(new string('-', 55));
        Console.WriteLine($" TOTAL A PAGAR:   {total,15:C0}");
        Console.WriteLine(new string('=', 55));
        Console.WriteLine($"[OK] Venta registrada. Stock restante: {producto.Existencias - cantidad} unidades.");
    }

    // ---------------------------------------------------
    // OPCIÓN 4: Reporte de caja del día
    // ---------------------------------------------------
    static void MostrarReporteDiario()
    {
        MostrarEncabezado("REPORTE DE CAJA DEL DÍA");

        if (ventasDelDia.Count == 0)
        {
            Console.WriteLine("Todavía no se ha registrado ninguna venta hoy.");
            return;
        }

        double totalRecaudado = 0;
        int totalUnidadesVendidas = 0;

        // Para saber el producto más vendido, contamos unidades por nombre
        Dictionary<string, int> unidadesPorProducto = new();

        foreach (var venta in ventasDelDia)
        {
            totalRecaudado += venta.Total;
            totalUnidadesVendidas += venta.Cantidad;

            if (unidadesPorProducto.ContainsKey(venta.Producto))
                unidadesPorProducto[venta.Producto] += venta.Cantidad;
            else
                unidadesPorProducto[venta.Producto] = venta.Cantidad;
        }

        string productoEstrella = "";
        int maxUnidades = -1;
        foreach (var par in unidadesPorProducto)
        {
            if (par.Value > maxUnidades)
            {
                maxUnidades = par.Value;
                productoEstrella = par.Key;
            }
        }

        double promedioPorVenta = totalRecaudado / ventasDelDia.Count;

        Console.WriteLine($" Número de ventas realizadas: {ventasDelDia.Count}");
        Console.WriteLine($" Unidades totales vendidas:   {totalUnidadesVendidas}");
        Console.WriteLine($" Total recaudado:             {totalRecaudado:C0}");
        Console.WriteLine($" Promedio por venta:          {promedioPorVenta:C0}");
        Console.WriteLine($" Producto más vendido:        {productoEstrella} ({maxUnidades} unidades)");
    }

    // ---------------------------------------------------
    // MÉTODOS DE UTILIDAD (lectura segura y presentación)
    // ---------------------------------------------------

    static void MostrarEncabezado(string titulo)
    {
        Console.Clear();
        Console.WriteLine(new string('=', 55));
        Console.WriteLine(titulo.PadLeft((55 + titulo.Length) / 2));
        Console.WriteLine(new string('=', 55));
    }

    static int LeerEntero(string mensaje, int minimo, int maximo)
    {
        int valor;
        bool esValido;

        do
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine() ?? "";
            esValido = int.TryParse(entrada, out valor);

            if (!esValido)
            {
                Console.WriteLine("[ERROR] Debe ingresar un número entero válido.");
            }
            else if (valor < minimo || valor > maximo)
            {
                Console.WriteLine($"[ERROR] El valor debe estar entre {minimo} y {maximo}.");
                esValido = false;
            }

        } while (!esValido);

        return valor;
    }

    static double LeerDecimal(string mensaje, double minimo)
    {
        double valor;
        bool esValido;

        do
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine() ?? "";
            esValido = double.TryParse(entrada, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);

            if (!esValido)
            {
                Console.WriteLine("[ERROR] Debe ingresar un valor numérico válido.");
            }
            else if (valor < minimo)
            {
                Console.WriteLine($"[ERROR] El valor no puede ser menor a {minimo}.");
                esValido = false;
            }

        } while (!esValido);

        return valor;
    }
}
