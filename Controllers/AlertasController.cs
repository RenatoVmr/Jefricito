using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;
using System.Linq;

namespace software.Controllers
{
    [Authorize]
    public class AlertasController : Controller
    {
        private readonly List<Producto> _productos;

        public AlertasController()
        {
            // Datos de ejemplo
            _productos = new List<Producto>
            {
                new Producto
                {
                    Codigo = "PROD001",
                    Nombre = "Leche Entera",
                    Categoria = "Lácteos",
                    CantidadNumerica = 45,
                    UnidadMedida = "Litros",
                    PrecioUnitario = 12.00M,
                    Ubicacion = "Almacén A - Estante 1",
                    Vencimiento = DateTime.Now.AddDays(-2),
                    StockMinimo = 50
                },
                new Producto
                {
                    Codigo = "PROD002",
                    Nombre = "Yogurt Natural",
                    Categoria = "Lácteos",
                    CantidadNumerica = 3,
                    UnidadMedida = "Unidades",
                    PrecioUnitario = 5.00M,
                    Ubicacion = "Almacén A - Estante 2",
                    Vencimiento = DateTime.Now.AddDays(-4),
                    StockMinimo = 30
                },
                new Producto
                {
                    Codigo = "PROD003",
                    Nombre = "Pan Integral",
                    Categoria = "Panadería",
                    CantidadNumerica = 80,
                    UnidadMedida = "Unidades",
                    PrecioUnitario = 3.50M,
                    Ubicacion = "Almacén B - Estante 1",
                    Vencimiento = DateTime.Now.AddDays(24),
                    StockMinimo = 100
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
                    Vencimiento = DateTime.Now.AddDays(-7),
                    StockMinimo = 10
                }
            };
        }

        public IActionResult Detalles(string id)
        {
            var producto = _productos.FirstOrDefault(p => p.Codigo == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        public IActionResult Index()
        {
            // Filtrar productos según su estado
            var productosVencidos = _productos.Where(p => p.Vencimiento < DateTime.Now).ToList();
            var productosProximosVencer = _productos.Where(p => p.Vencimiento >= DateTime.Now && p.Vencimiento <= DateTime.Now.AddDays(30)).ToList();
            var productosBajoStock = _productos.Where(p => p.CantidadNumerica < p.StockMinimo).ToList();

            ViewBag.ProductosVencidos = productosVencidos;
            ViewBag.ProductosProximosVencer = productosProximosVencer;
            ViewBag.ProductosBajoStock = productosBajoStock;

            return View();
        }

        public IActionResult ExportarPDF()
        {
            var productosVencidos = _productos.Where(p => p.Vencimiento < DateTime.Now).ToList();
            var productosProximosVencer = _productos.Where(p => p.Vencimiento >= DateTime.Now && p.Vencimiento <= DateTime.Now.AddDays(30)).ToList();
            var productosBajoStock = _productos.Where(p => p.CantidadNumerica < p.StockMinimo).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                var writerProperties = new iText.Kernel.Pdf.WriterProperties().SetPdfVersion(iText.Kernel.Pdf.PdfVersion.PDF_2_0);
                var writer = new iText.Kernel.Pdf.PdfWriter(ms, writerProperties);
                var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                // Título del reporte
                document.Add(new iText.Layout.Element.Paragraph("Reporte de Alertas de Inventario")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20));

                document.Add(new iText.Layout.Element.Paragraph($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(10));

                // Sección de Productos Vencidos
                if (productosVencidos.Any())
                {
                    document.Add(new iText.Layout.Element.Paragraph("\nProductos Vencidos")
                        .SetFontSize(14));

                    var table = new iText.Layout.Element.Table(7).UseAllAvailableWidth();
                    table.AddHeaderCell("Código");
                    table.AddHeaderCell("Producto");
                    table.AddHeaderCell("Lote");
                    table.AddHeaderCell("Cantidad");
                    table.AddHeaderCell("F. Vencimiento");
                    table.AddHeaderCell("Días Vencido");
                    table.AddHeaderCell("Ubicación");

                    foreach (var producto in productosVencidos)
                    {
                        var diasVencido = (DateTime.Now - producto.Vencimiento).Days;
                        table.AddCell(producto.Codigo);
                        table.AddCell(producto.Nombre);
                        table.AddCell($"LT{producto.Vencimiento:yyyy-MM}-{producto.Codigo.Substring(4)}");
                        table.AddCell(producto.Cantidad);
                        table.AddCell(producto.Vencimiento.ToString("dd/MM/yyyy"));
                        table.AddCell($"{diasVencido} días");
                        table.AddCell(producto.Ubicacion);
                    }

                    document.Add(table);
                }

                // Sección de Productos Próximos a Vencer
                if (productosProximosVencer.Any())
                {
                    document.Add(new iText.Layout.Element.Paragraph("\nProductos Próximos a Vencer")
                        .SetFontSize(14));

                    var table = new iText.Layout.Element.Table(7).UseAllAvailableWidth();
                    table.AddHeaderCell("Código");
                    table.AddHeaderCell("Producto");
                    table.AddHeaderCell("Lote");
                    table.AddHeaderCell("Cantidad");
                    table.AddHeaderCell("F. Vencimiento");
                    table.AddHeaderCell("Días Restantes");
                    table.AddHeaderCell("Ubicación");

                    foreach (var producto in productosProximosVencer)
                    {
                        var diasRestantes = (producto.Vencimiento - DateTime.Now).Days;
                        table.AddCell(producto.Codigo);
                        table.AddCell(producto.Nombre);
                        table.AddCell($"LT{producto.Vencimiento:yyyy-MM}-{producto.Codigo.Substring(4)}");
                        table.AddCell(producto.Cantidad);
                        table.AddCell(producto.Vencimiento.ToString("dd/MM/yyyy"));
                        table.AddCell($"{diasRestantes} días");
                        table.AddCell(producto.Ubicacion);
                    }

                    document.Add(table);
                }

                // Sección de Productos con Stock Bajo
                if (productosBajoStock.Any())
                {
                    document.Add(new iText.Layout.Element.Paragraph("\nProductos con Stock Bajo")
                        .SetFontSize(14));

                    var table = new iText.Layout.Element.Table(7).UseAllAvailableWidth();
                    table.AddHeaderCell("Código");
                    table.AddHeaderCell("Producto");
                    table.AddHeaderCell("Categoría");
                    table.AddHeaderCell("Stock Actual");
                    table.AddHeaderCell("Stock Mínimo");
                    table.AddHeaderCell("Déficit");
                    table.AddHeaderCell("Ubicación");

                    foreach (var producto in productosBajoStock)
                    {
                        var deficit = producto.CantidadNumerica - producto.StockMinimo;
                        table.AddCell(producto.Codigo);
                        table.AddCell(producto.Nombre);
                        table.AddCell(producto.Categoria);
                        table.AddCell(producto.Cantidad);
                        table.AddCell($"{producto.StockMinimo} {producto.UnidadMedida}");
                        table.AddCell($"{deficit:F2} {producto.UnidadMedida}");
                        table.AddCell(producto.Ubicacion);
                    }

                    document.Add(table);
                }

                document.Close();
                return File(ms.ToArray(), "application/pdf", $"AlertasInventario_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
    }
}