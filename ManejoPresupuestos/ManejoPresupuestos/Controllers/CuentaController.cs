using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers {
    public class CuentaController : Controller {
        private readonly IRepositoryTipoCuenta repositoryTipoCuenta;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IServicioReportes servicioReportes;
        private readonly IRepositoryCuenta repositoryCuenta;
        private readonly IRepositoryTransaccion repositoryTransaccion;
        private readonly IMapper mapper;

        public CuentaController(
            IRepositoryTipoCuenta repositoryTipoCuenta,
            IRepositoryCuenta repositoryCuenta,
            IRepositoryTransaccion repositoryTransaccion,
            IServicioUsuario servicioUsuario,
            IServicioReportes servicioReportes,
            IMapper mapper
        ) {
            this.repositoryTipoCuenta = repositoryTipoCuenta;
            this.servicioUsuario = servicioUsuario;
            this.servicioReportes = servicioReportes;
            this.repositoryCuenta = repositoryCuenta;
            this.repositoryTransaccion = repositoryTransaccion;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositoryCuenta.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(c => c.TipoCuenta)
                .Select(group => new IndiceCuentaViewModel {
                    TipoCuenta = group.Key,
                    Cuentas = group.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id, int mes, int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositoryCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, año, ViewBag);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear() {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositoryTipoCuenta.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid) {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await repositoryCuenta.Crear(cuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositoryCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositoryCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await repositoryCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryCuenta.Actualizar(cuentaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositoryCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var cuenta = await repositoryCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryCuenta.Borrar(id);

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId) {
            var tiposCuentas = await repositoryTipoCuenta.Obtener(usuarioId);
            return tiposCuentas
                .Select(t => new SelectListItem(t.Nombre, t.Id.ToString()));
        }
    }
}