using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;

namespace software.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private static readonly List<Venta> _ventas = new();

        public IActionResult Index()
        {
            var resumen = new ResumenVentas
            {
                TotalVentas = _ventas.Count,
                IngresosTotales = _ventas.Sum(v => v.Total),
                UnidadesVendidas = _ventas.Sum(v => v.Cantidad)
            };

            ViewBag.Resumen = resumen;
            return View(_ventas);
        }

        public IActionResult Nueva()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nueva(Venta venta)
        {
            if (ModelState.IsValid)
            {
                venta.VendidoPor = User.Identity?.Name ?? "Sistema";
                _ventas.Add(venta);
                return RedirectToAction(nameof(Index));
            }
            return View(venta);
        }

        [HttpPost]
        public IActionResult Eliminar(string id)
        {
            var venta = _ventas.FirstOrDefault(v => v.Id == id);
            if (venta != null)
            {
                _ventas.Remove(venta);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}