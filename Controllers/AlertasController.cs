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
    }
}