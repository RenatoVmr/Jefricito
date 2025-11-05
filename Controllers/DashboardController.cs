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
    }
}