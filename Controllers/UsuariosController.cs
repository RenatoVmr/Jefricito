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

        public IActionResult Edit(string id)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel
            {
                Username = usuario.Username,
                Email = usuario.Email,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };

            ViewBag.IsEdit = true;
            ViewBag.UserId = usuario.Id;
            return View("Agregar", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                ViewBag.UserId = id;
                return View("Agregar", model);
            }

            var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Actualizar campos (no sobreescribimos la contraseña si la dejan en blanco)
            usuario.Username = model.Username;
            usuario.Email = model.Email;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                usuario.Password = _passwordHasher.HashPassword(model.Password);
            }

            TempData["SuccessMessage"] = "Usuario actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario != null)
            {
                _usuarios.Remove(usuario);
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}