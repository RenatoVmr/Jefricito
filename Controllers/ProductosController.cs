using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;

namespace software.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        public static List<Producto> _productos = new()
        {
            new Producto
            {
                Codigo = "PROD001",
                Nombre = "Leche Entera",
                Categoria = "Lácteos",
                CantidadNumerica = 45,
                UnidadMedida = "Litros",
                PrecioUnitario = 2.50M,
                Ubicacion = "Almacén A - Estante 1",
                Vencimiento = DateTime.Parse("2025-11-07"),
                EstadoStock = "Disponible"
            },
            new Producto
            {
                Codigo = "PROD002",
                Nombre = "Yogurt Natural",
                Categoria = "Lácteos",
                CantidadNumerica = 15,
                UnidadMedida = "Unidades",
                PrecioUnitario = 1.80M,
                Ubicacion = "Almacén A - Estante 2",
                Vencimiento = DateTime.Parse("2025-11-04"),
                EstadoStock = "Bajo Stock"
            },
            new Producto
            {
                Codigo = "PROD003",
                Nombre = "Pan Integral",
                Categoria = "Panadería",
                CantidadNumerica = 80,
                UnidadMedida = "Unidades",
                PrecioUnitario = 1.20M,
                Ubicacion = "Almacén B - Estante 1",
                Vencimiento = DateTime.Parse("2025-12-02"),
                EstadoStock = "Disponible"
            },
            new Producto
            {
                Codigo = "PROD004",
                Nombre = "Aceite de Oliva",
                Categoria = "Aceites",
                CantidadNumerica = 25,
                UnidadMedida = "Botellas",
                PrecioUnitario = 8.50M,
                Ubicacion = "Almacén C - Estante 3",
                Vencimiento = DateTime.Parse("2026-05-01"),
                EstadoStock = "Disponible"
            },
            new Producto
            {
                Codigo = "PROD005",
                Nombre = "Arroz Blanco",
                Categoria = "Granos",
                CantidadNumerica = 120,
                UnidadMedida = "Kg",
                PrecioUnitario = 1.50M,
                Ubicacion = "Almacén C - Estante 1",
                Vencimiento = DateTime.Parse("2026-11-02"),
                EstadoStock = "Disponible"
            },
            new Producto
            {
                Codigo = "PROD006",
                Nombre = "Queso Fresco",
                Categoria = "Lácteos",
                CantidadNumerica = 8,
                UnidadMedida = "Kg",
                PrecioUnitario = 12.00M,
                Ubicacion = "Almacén A - Refrigerador 1",
                Vencimiento = DateTime.Parse("2025-11-01"),
                EstadoStock = "Bajo Stock"
            }
        };

        public IActionResult Index()
        {
            return View(_productos);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.Codigo = $"PROD{(_productos.Count + 1):000}";
                _productos.Add(producto);
                TempData["SuccessMessage"] = "Producto agregado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            var producto = _productos.FirstOrDefault(p => p.Codigo == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id, Producto producto)
        {
            if (id != producto.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var index = _productos.FindIndex(p => p.Codigo == id);
                if (index != -1)
                {
                    _productos[index] = producto;
                    TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var producto = _productos.FirstOrDefault(p => p.Codigo == id);
            if (producto != null)
            {
                _productos.Remove(producto);
                TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        public static List<Producto> ObtenerProductos()
        {
            return _productos;
        }

        public IActionResult ExportarPDF()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0);
                var writer = new PdfWriter(ms, writerProperties);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Listado de Productos")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));

                Table table = new Table(8).UseAllAvailableWidth();

                // Encabezados
                table.AddHeaderCell("Código");
                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Categoría");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Precio Unit.");
                table.AddHeaderCell("Ubicación");
                table.AddHeaderCell("Vencimiento");
                table.AddHeaderCell("Valor Total");

                // Agregar datos
                foreach (var producto in _productos)
                {
                    table.AddCell(producto.Codigo);
                    table.AddCell(producto.Nombre);
                    table.AddCell(producto.Categoria);
                    table.AddCell(producto.Cantidad);
                    table.AddCell($"S/{producto.PrecioUnitario:N2}");
                    table.AddCell(producto.Ubicacion);
                    table.AddCell(producto.Vencimiento.ToString("dd/MM/yyyy"));
                    table.AddCell($"S/{producto.ValorTotal:N2}");
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Productos.pdf");
            }
        }
    }
}