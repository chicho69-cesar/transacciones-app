using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Controllers {
    public class TipoCuentaController : Controller {
        private readonly IRepositoryTipoCuenta repositoryTipoCuenta;
        private readonly IServicioUsuario servicioUsuario;

        public TipoCuentaController (
            IRepositoryTipoCuenta repositoryTipoCuenta, 
            IServicioUsuario servicioUsuario
        ) {
            this.repositoryTipoCuenta = repositoryTipoCuenta;
            this.servicioUsuario = servicioUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tiposCuentas = await repositoryTipoCuenta.Obtener(usuarioId);
            return View(tiposCuentas);
        }

        [HttpGet]
        public IActionResult Crear() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta) {
            if (!ModelState.IsValid) {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuario.ObtenerUsuarioId();

            var existe = await repositoryTipoCuenta.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            
            if (existe) {
                ModelState.AddModelError(
                    nameof(TipoCuenta.Nombre),
                    $"El nombre {tipoCuenta.Nombre} ya existe en la base de datos"
                );

                return View(tipoCuenta);
            }

            await repositoryTipoCuenta.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositoryTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositoryTipoCuenta.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryTipoCuenta.Actualizar(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositoryTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositoryTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryTipoCuenta.Borrar(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre, int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var existeTipoCuenta = await repositoryTipoCuenta.Existe(nombre, usuarioId, id);

            if (existeTipoCuenta) {
                return Json($"El nombre {nombre} ya existe en la base de datos");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tiposCuentas = await repositoryTipoCuenta.Obtener(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(t => t.Id).ToList();

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0) {
                return Forbid(); // Significa prohibido
            }

            var tiposCuentasOrdenados = ids.Select(
                (valor, indice) => new TipoCuenta() {
                    Id = valor,
                    Orden = indice + 1
                }
            ).AsEnumerable();

            await repositoryTipoCuenta.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}