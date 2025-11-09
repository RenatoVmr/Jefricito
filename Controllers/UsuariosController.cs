using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software.Models;
using software.Services;

namespace software.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly List<Usuario> _usuarios;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public UsuariosController(List<Usuario> usuarios, IUserService userService, IPasswordHasher passwordHasher)
        {
            _usuarios = usuarios;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsuarios = _usuarios.Count;
            ViewBag.Administradores = _usuarios.Count(u => u.Role == "Admin");
            ViewBag.UsuariosRegulares = _usuarios.Count(u => u.Role == "User");
            
            return View(_usuarios);
        }

        public IActionResult Agregar()
        {
            return View(new RegisterViewModel 
            { 
                Username = "",
                Password = "",
                ConfirmPassword = "",
                Email = ""
            });
        }

        [HttpPost]
        public IActionResult Agregar(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string error;
                if (_userService.RegisterUser(model, out error))
                {
                    TempData["SuccessMessage"] = "Usuario registrado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }
    }
}