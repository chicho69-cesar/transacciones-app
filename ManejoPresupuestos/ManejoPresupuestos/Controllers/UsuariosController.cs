using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Controllers {
    public class UsuariosController : Controller {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        public UsuariosController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager
        ) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registro() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo) {
            if (!ModelState.IsValid) {
                return View(modelo);
            }

            var usuario = new Usuario() {
                Email = modelo.Email
            };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded) {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transaccion");
            } else {
                foreach (var error in resultado.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo) {
            if (!ModelState.IsValid) {
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded) {
                return RedirectToAction("Index", "Transaccion");
            } else {
                ModelState.AddModelError(string.Empty, "Correo electronico o password incorrecto.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transaccion");
        }
    }
}