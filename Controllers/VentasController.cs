using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;

namespace software.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private static readonly List<Venta> _ventas = new List<Venta>();
        private static readonly object _lock = new object();
        private readonly List<Producto> _productos;

        static VentasController()
        {
            // Agregar una venta de prueba
            _ventas.Add(new Venta
            {
                Id = Guid.NewGuid().ToString(),
                Codigo = "PROD001",
                Producto = "Leche Entera",
                Categoria = "Lácteos",
                Cantidad = 2,
                PrecioUnitario = 2.50M,
                Cliente = "Cliente Ejemplo",
                VendidoPor = "Admin",
                Fecha = DateTime.Now.AddMinutes(-5)
            });
        }

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
            List<Venta> ventasOrdenadas;
            lock (_lock)
            {
                ventasOrdenadas = _ventas.OrderByDescending(v => v.Fecha).ToList();
            }
            
            var resumen = new ResumenVentas
            {
                TotalVentas = ventasOrdenadas.Count,
                IngresosTotales = ventasOrdenadas.Sum(v => v.Total),
                UnidadesVendidas = ventasOrdenadas.Sum(v => v.Cantidad)
            };

            // Para depuración
            System.Diagnostics.Debug.WriteLine($"Total de ventas en la lista: {ventasOrdenadas.Count}");
            foreach (var venta in ventasOrdenadas)
            {
                System.Diagnostics.Debug.WriteLine($"Venta ID: {venta.Id}, Producto: {venta.Producto}, Cliente: {venta.Cliente}, Fecha: {venta.Fecha}");
            }

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
            try
            {
                System.Diagnostics.Debug.WriteLine($"Recibiendo venta - Código: {venta.Codigo}, Producto: {venta.Producto}, Cliente: {venta.Cliente}, Cantidad: {venta.Cantidad}");

                // Validar que todos los campos requeridos estén presentes
                if (string.IsNullOrEmpty(venta.Codigo) || 
                    string.IsNullOrEmpty(venta.Producto) || 
                    string.IsNullOrEmpty(venta.Categoria) || 
                    string.IsNullOrEmpty(venta.Cliente))
                {
                    ModelState.AddModelError(string.Empty, "Todos los campos son requeridos");
                    return View(venta);
                }

                // Validar cantidad y precio
                if (venta.Cantidad <= 0 || venta.PrecioUnitario <= 0)
                {
                    ModelState.AddModelError(string.Empty, "La cantidad y el precio deben ser mayores a 0");
                    return View(venta);
                }

                // Validar que el producto existe y tiene stock suficiente
                var producto = _productos.FirstOrDefault(p => p.Codigo == venta.Codigo);
                if (producto == null)
                {
                    ModelState.AddModelError(string.Empty, "El producto no existe");
                    return View(venta);
                }

                if (producto.CantidadNumerica < venta.Cantidad)
                {
                    ModelState.AddModelError(string.Empty, "No hay suficiente stock disponible");
                    return View(venta);
                }

                // Establecer los datos de la venta
                venta.Id = Guid.NewGuid().ToString();
                venta.Fecha = DateTime.Now;
                venta.VendidoPor = User.Identity?.Name ?? "Sistema";

                // Actualizar el stock del producto
                producto.CantidadNumerica -= venta.Cantidad;
                if (producto.CantidadNumerica <= 10)
                {
                    producto.EstadoStock = "Bajo Stock";
                }

                // Agregar la venta de forma thread-safe
                lock (_lock)
                {
                    _ventas.Add(venta);
                    System.Diagnostics.Debug.WriteLine($"Venta agregada exitosamente - ID: {venta.Id}");
                    System.Diagnostics.Debug.WriteLine($"Total de ventas en la lista: {_ventas.Count}");
                }

                // Configurar mensajes de éxito
                TempData["SuccessMessage"] = $"Se registró la venta de {venta.Cantidad} unidades de {venta.Producto}";
                TempData["LastVentaId"] = venta.Id;
                TempData["VentaTotal"] = venta.Total.ToString("C2");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar la venta: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar la venta. Por favor, inténtelo nuevamente.");
                return View(venta);
            }

            // Si llegamos aquí, hay errores de validación
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