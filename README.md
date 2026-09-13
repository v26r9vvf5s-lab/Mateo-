# Mini-POS: Sistema de Ventas e Inventario

## Información del Estudiante

| Campo               | Detalle                               |
| ------------------- | -------------------------------------- |
| **Nombre completo** | Jhon Mateo Guette Vera                 |
| **Módulo**          | Unidad 1 — Fundamentos de C# (.NET 8)  |
| **Fecha**           | Septiembre 2026                        |

---

## Descripción del Proyecto

Aplicación de consola en **C# (.NET 8)** que simula un pequeño **Punto de Venta (POS)** con control de inventario, pensada para un negocio pequeño.

Funcionalidades principales:

- **Registrar productos** con nombre, precio y existencias iniciales.
- **Consultar el inventario**, con aviso de bajo stock (5 unidades o menos).
- **Registrar ventas**, con IVA del 19% y descuento del 8% para clientes frecuentes.
- **Ver el reporte de caja del día**: total recaudado, unidades vendidas, promedio por venta y producto más vendido.

> **Restricción de diseño:** el proyecto usa únicamente los conceptos de la Unidad 1 (variables, `List<T>`, tuplas, `switch`, ciclos, métodos estáticos, `try/catch` mediante `TryParse`). No se usan clases personalizadas ni programación orientada a objetos.

---

## Estructura del Proyecto

```
MiniPOS/
├── Program.cs        ← Código fuente principal
├── MiniPOS.csproj     ← Configuración del proyecto
├── .gitignore         ← Exclusión de bin/ y obj/
└── README.md          ← Este archivo
```

---

## Métodos Estáticos Implementados

| Método                                   | Propósito                                            |
| ----------------------------------------- | ----------------------------------------------------- |
| `LeerEntero(mensaje, minimo, maximo)`     | Lectura segura de números enteros dentro de un rango  |
| `LeerDecimal(mensaje, minimo)`            | Lectura segura de números decimales                   |
| `AgregarProducto()`                       | Registra un nuevo producto en el inventario           |
| `MostrarInventario()`                     | Imprime la tabla de inventario con alertas de stock   |
| `RealizarVenta()`                         | Calcula subtotal, descuento, IVA y total de una venta |
| `MostrarReporteDiario()`                  | Calcula las estadísticas de ventas del día            |
| `MostrarEncabezado(titulo)`               | Da formato visual a los títulos de cada pantalla      |

---

## Lógica de Cálculo de una Venta

```
Subtotal   = Precio unitario x Cantidad
Descuento  = Subtotal x 8%   (solo si es cliente frecuente)
Base       = Subtotal - Descuento
IVA        = Base x 19%
Total      = Base + IVA
```

---

## Cómo Ejecutarlo

```bash
dotnet run
```

---

## Licencia

Proyecto académico — uso educativo.
