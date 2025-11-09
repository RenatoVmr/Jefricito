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
            // Obtener productos del controlador de productos
            _productos = ProductosController.ObtenerProductos();
        }

        public IActionResult Index()
        {
            var hoy = DateTime.Now;
            
            // Obtener productos vencidos
            var productosVencidos = _productos
                .Where(p => p.Vencimiento < hoy)
                .ToList();

            // Obtener productos próximos a vencer (30 días)
            var productosProximosVencer = _productos
                .Where(p => p.Vencimiento >= hoy && p.Vencimiento <= hoy.AddDays(30))
                .OrderBy(p => p.Vencimiento)
                .ToList();

            // Obtener productos con stock bajo
            var productosBajoStock = _productos
                .Where(p => p.CantidadNumerica <= p.StockMinimo)
                .OrderBy(p => p.CantidadNumerica)
                .ToList();

            ViewBag.TotalVencidos = productosVencidos.Count;
            ViewBag.TotalProximosVencer = productosProximosVencer.Count;
            ViewBag.TotalBajoStock = productosBajoStock.Count;

            ViewBag.ProductosVencidos = productosVencidos;
            ViewBag.ProductosProximosVencer = productosProximosVencer;
            ViewBag.ProductosBajoStock = productosBajoStock;

            return View();
        }
    }
}