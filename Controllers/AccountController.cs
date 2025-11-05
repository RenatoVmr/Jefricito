using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using software.Models;
using software.Services;
using System.Security.Claims;

namespace software.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string error;
                if (_userService.RegisterUser(model, out error))
                {
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicie sesión.";
                    return RedirectToAction(nameof(Login));
                }
                
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                Usuario? usuario;
                if (_userService.ValidateCredentials(model.Username, model.Password, out usuario))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, usuario.Username),
                        new(ClaimTypes.Role, usuario.Role),
                        new(ClaimTypes.Email, usuario.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        RedirectUri = returnUrl
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (string.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Dashboard");
                    return LocalRedirect(returnUrl);
                }

                if (usuario != null && _userService.IsUserLockedOut(usuario))
                {
                    ModelState.AddModelError(string.Empty, 
                        "Su cuenta está bloqueada temporalmente por demasiados intentos fallidos. Por favor, intente más tarde.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}