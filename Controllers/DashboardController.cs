using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;

namespace software.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var dashboardModel = new DashboardViewModel
            {
                TotalProductos = 6,
                ValorTotal = 724.00M,
                BajoStock = 2,
                AlertasCriticas = 3,
                InventarioPorCategoria = new List<CategoriaInventario>
                {
                    new() { Nombre = "Lácteos", Unidades = 68 },
                    new() { Nombre = "Panadería", Unidades = 80 },
                    new() { Nombre = "Aceites", Unidades = 25 },
                    new() { Nombre = "Granos", Unidades = 120 }
                },
                ValorizacionPorCategoria = new List<CategoriaValorizacion>
                {
                    new() { Nombre = "Lácteos", Valor = 235.50M },
                    new() { Nombre = "Aceites", Valor = 212.50M },
                    new() { Nombre = "Granos", Valor = 180.00M },
                    new() { Nombre = "Panadería", Valor = 96.00M }
                },
                EstadoDelInventario = new EstadoInventario
                {
                    ProductosOptimos = 3,
                    BajoStock = 2,
                    VencidosoPorVencer = 3
                }
            };

            return View(dashboardModel);
        }

        public IActionResult ExportarPDF()
        {
            var dashboardModel = new DashboardViewModel
            {
                TotalProductos = 6,
                ValorTotal = 724.00M,
                BajoStock = 2,
                AlertasCriticas = 3,
                InventarioPorCategoria = new List<CategoriaInventario>
                {
                    new() { Nombre = "Lácteos", Unidades = 68 },
                    new() { Nombre = "Panadería", Unidades = 80 },
                    new() { Nombre = "Aceites", Unidades = 25 },
                    new() { Nombre = "Granos", Unidades = 120 }
                },
                ValorizacionPorCategoria = new List<CategoriaValorizacion>
                {
                    new() { Nombre = "Lácteos", Valor = 235.50M },
                    new() { Nombre = "Aceites", Valor = 212.50M },
                    new() { Nombre = "Granos", Valor = 180.00M },
                    new() { Nombre = "Panadería", Valor = 96.00M }
                },
                EstadoDelInventario = new EstadoInventario
                {
                    ProductosOptimos = 3,
                    BajoStock = 2,
                    VencidosoPorVencer = 3
                }
            };

            using (MemoryStream ms = new MemoryStream())
            {
                var writerProperties = new iText.Kernel.Pdf.WriterProperties().SetPdfVersion(iText.Kernel.Pdf.PdfVersion.PDF_2_0);
                var writer = new iText.Kernel.Pdf.PdfWriter(ms, writerProperties);
                var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                // Título y fecha
                document.Add(new iText.Layout.Element.Paragraph("Reporte de Dashboard")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20));

                document.Add(new iText.Layout.Element.Paragraph($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(10));

                // Resumen General
                document.Add(new iText.Layout.Element.Paragraph("\nResumen General")
                    .SetFontSize(14));

                var resumenTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                resumenTable.AddCell("Total de Productos");
                resumenTable.AddCell(dashboardModel.TotalProductos.ToString());
                resumenTable.AddCell("Valor Total del Inventario");
                resumenTable.AddCell($"S/ {dashboardModel.ValorTotal:N2}");
                resumenTable.AddCell("Productos en Bajo Stock");
                resumenTable.AddCell(dashboardModel.BajoStock.ToString());
                resumenTable.AddCell("Alertas Críticas");
                resumenTable.AddCell(dashboardModel.AlertasCriticas.ToString());

                document.Add(resumenTable);

                // Estado del Inventario
                document.Add(new iText.Layout.Element.Paragraph("\nEstado del Inventario")
                    .SetFontSize(14));

                var estadoTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                estadoTable.AddCell("Productos Óptimos");
                estadoTable.AddCell(dashboardModel.EstadoDelInventario.ProductosOptimos.ToString());
                estadoTable.AddCell("Bajo Stock");
                estadoTable.AddCell(dashboardModel.EstadoDelInventario.BajoStock.ToString());
                estadoTable.AddCell("Vencidos/Por Vencer");
                estadoTable.AddCell(dashboardModel.EstadoDelInventario.VencidosoPorVencer.ToString());

                document.Add(estadoTable);

                // Inventario por Categoría
                document.Add(new iText.Layout.Element.Paragraph("\nInventario por Categoría")
                    .SetFontSize(14));

                var categoriaTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                categoriaTable.AddHeaderCell("Categoría");
                categoriaTable.AddHeaderCell("Unidades");

                foreach (var categoria in dashboardModel.InventarioPorCategoria)
                {
                    categoriaTable.AddCell(categoria.Nombre);
                    categoriaTable.AddCell($"{categoria.Unidades} unidades");
                }

                document.Add(categoriaTable);

                // Valorización por Categoría
                document.Add(new iText.Layout.Element.Paragraph("\nValorización por Categoría")
                    .SetFontSize(14));

                var valorizacionTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth();
                valorizacionTable.AddHeaderCell("Categoría");
                valorizacionTable.AddHeaderCell("Valor");

                foreach (var categoria in dashboardModel.ValorizacionPorCategoria)
                {
                    valorizacionTable.AddCell(categoria.Nombre);
                    valorizacionTable.AddCell($"S/ {categoria.Valor:N2}");
                }

                document.Add(valorizacionTable);

                document.Close();
                return File(ms.ToArray(), "application/pdf", $"Dashboard_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
    }
}