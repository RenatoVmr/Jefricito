using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;

namespace software.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private static readonly List<Venta> _ventas = new();
        private readonly List<Producto> _productos;

        public VentasController()
        {
            // Obtenemos la lista de productos del controlador de productos
            _productos = ProductosController.ObtenerProductos();
        }

        [HttpGet]
        public IActionResult BuscarProducto(string codigo)
        {
            var producto = _productos.FirstOrDefault(p => p.Codigo == codigo);
            if (producto == null)
                return NotFound();

            return Json(new
            {
                producto = producto.Nombre,
                categoria = producto.Categoria,
                precioUnitario = producto.PrecioUnitario
            });
        }

        public IActionResult Index()
        {
            // Ordenar las ventas por fecha descendente (más recientes primero)
            var ventasOrdenadas = _ventas.OrderByDescending(v => v.Fecha).ToList();
            
            var resumen = new ResumenVentas
            {
                TotalVentas = ventasOrdenadas.Count,
                IngresosTotales = ventasOrdenadas.Sum(v => v.Total),
                UnidadesVendidas = ventasOrdenadas.Sum(v => v.Cantidad)
            };

            ViewBag.Resumen = resumen;
            return View(ventasOrdenadas);
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
                // Establecer la fecha actual
                venta.Fecha = DateTime.Now;
                
                // Establecer el vendedor
                venta.VendidoPor = User.Identity?.Name ?? "Sistema";
                
                // Validar que el producto existe
                var producto = _productos.FirstOrDefault(p => p.Codigo == venta.Codigo);
                if (producto == null)
                {
                    ModelState.AddModelError(string.Empty, "El producto no existe");
                    return View(venta);
                }

                // Validar el stock
                if (producto.CantidadNumerica < venta.Cantidad)
                {
                    ModelState.AddModelError(string.Empty, "No hay suficiente stock disponible");
                    return View(venta);
                }

                // Actualizar el stock del producto
                producto.CantidadNumerica -= venta.Cantidad;
                if (producto.CantidadNumerica <= 10)
                {
                    producto.EstadoStock = "Bajo Stock";
                }

                // Agregar la venta
                _ventas.Add(venta);

                // Agregar mensaje de éxito
                TempData["SuccessMessage"] = "Venta registrada exitosamente";
                
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