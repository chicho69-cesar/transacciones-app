using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers {
    public class TransaccionController : Controller {
        private readonly IServicioUsuario servicioUsuario;
        private readonly IServicioReportes servicioReportes;
        private readonly IServicioGenerarExcel servicioGenerarExcel;
        private readonly IRepositoryTransaccion repositoryTransaccion;
        private readonly IRepositoryCuenta repositoryCuenta;
        private readonly IRepositoryCategoria repositoryCategoria;
        private readonly IMapper mapper;

        public TransaccionController(
            IRepositoryTransaccion repositoryTransaccion,
            IRepositoryCuenta repositoryCuenta,
            IRepositoryCategoria repositoryCategoria,
            IServicioUsuario servicioUsuario,
            IServicioReportes servicioReportes,
            IServicioGenerarExcel servicioGenerarExcel,
            IMapper mapper
        ) {
            this.servicioUsuario = servicioUsuario;
            this.servicioReportes = servicioReportes;
            this.servicioGenerarExcel = servicioGenerarExcel;
            this.repositoryTransaccion = repositoryTransaccion;
            this.repositoryCuenta = repositoryCuenta;
            this.repositoryCategoria = repositoryCategoria;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int mes, int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, mes, año, ViewBag);
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Semanal(int mes, int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = await servicioReportes.ObtenerReporteSemana(usuarioId, mes, año, ViewBag);

            var agrupado = transaccionesPorSemana
                .GroupBy(t => t.Semana)
                .Select(t => new ResultadoObtenerPorSemana() {
                    Semana = t.Key,
                    Ingresos = t
                        .Where(i => i.TipoOperacionId == TipoOperacion.Ingreso)
                        .Select(i => i.Monto)
                        .FirstOrDefault(),
                    Gastos = t
                        .Where(i => i.TipoOperacionId == TipoOperacion.Gasto)
                        .Select(i => i.Monto)
                        .FirstOrDefault()
                }).ToList();

            if (año == 0 || mes == 0) {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for (int i = 0; i < diasSegmentados.Count; i++) {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado
                    .FirstOrDefault(a => a.Semana == semana);

                if (grupoSemana is null) {
                    agrupado.Add(new ResultadoObtenerPorSemana() {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                } else {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            agrupado = agrupado
                .OrderByDescending(a => a.Semana)
                .ToList();

            var modelo = new ReporteSemanalViewModel {
                TransaccionesPorSemana = agrupado,
                FechaReferencia = fechaReferencia
            };

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Mensual(int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            if (año == 0) {
                año = DateTime.Today.Year;
            }

            var transaccionesPorMes = await repositoryTransaccion.ObtenerPorMes(usuarioId, año);

            var transaccionesAgrupadas = transaccionesPorMes
                .GroupBy(t => t.Mes)
                .Select(t => new ResultadoObtenerPorMes() {
                    Mes = t.Key,
                    Ingresos = t
                        .Where(i => i.TipoOperacionId == TipoOperacion.Ingreso)
                        .Select(i => i.Monto)
                        .FirstOrDefault(),
                    Gastos = t
                        .Where(i => i.TipoOperacionId == TipoOperacion.Gasto)
                        .Select(i => i.Monto)
                        .FirstOrDefault()
                }).ToList();

            for (int mes = 1; mes <= 12; mes++) {
                var transaccion = transaccionesAgrupadas
                    .FirstOrDefault(t => t.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1);

                if (transaccion is null) {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes() {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });
                } else {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas
                .OrderByDescending(t => t.Mes)
                .ToList();

            var modelo = new ReporteMensualViewModel {
                Año = año,
                TransaccionesPorMes = transaccionesAgrupadas
            };

            return View(modelo);
        }

        [HttpGet]
        public IActionResult ExcelReporte() {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorMes(int mes, int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var fechaInicio = new DateTime(año, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var transacciones = await ObtenerTransacciones(usuarioId, fechaInicio, fechaFin);
            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("MMMM yyyy")}.xlsx";

            return servicioGenerarExcel.GenerarExcel(nombreArchivo, transacciones);
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorAño(int año) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var fechaInicio = new DateTime(año, 1, 1);
            var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
            var transacciones = await ObtenerTransacciones(usuarioId, fechaInicio, fechaFin);
            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("yyyy")}.xlsx";

            return servicioGenerarExcel.GenerarExcel(nombreArchivo, transacciones);
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelTodo() {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(100);
            var transacciones = await ObtenerTransacciones(usuarioId, fechaInicio, fechaFin);
            var nombreArchivo = $"Manejo Presupuesto - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";

            return servicioGenerarExcel.GenerarExcel(nombreArchivo, transacciones);
        }

        [HttpGet]
        public IActionResult Calendario() {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerTransaccionesCalendario(DateTime start, DateTime end) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transacciones = await ObtenerTransacciones(usuarioId, start, end);

            var eventosCalendario = transacciones
                .Select(t => new EventoCalendario {
                    Title = t.Monto.ToString("C2"),
                    Start = t.FechaTransaccion.ToString("yyyy-MM-dd"),
                    End = t.FechaTransaccion.ToString("yyyy-MM-dd"),
                    Color = t.TipoOperacionId == TipoOperacion.Gasto 
                        ? "Red"
                        : "Blue"
                });
            
            return Json(eventosCalendario);
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerTransaccionesPorFecha(DateTime fecha) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transacciones = await ObtenerTransacciones(usuarioId, fecha, fecha);
            return Json(transacciones);
        }

        [HttpGet]
        public async Task<IActionResult> Crear() {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid) {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositoryCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositoryCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto) {
                modelo.Monto *= -1;
            }

            await repositoryTransaccion.Crear(modelo);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transaccion = await repositoryTransaccion.ObtenerPorId(id, usuarioId);

            if (transaccion is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto) {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = urlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid) {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositoryCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositoryCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);

            if (modelo.TipoOperacionId == TipoOperacion.Gasto) {
                transaccion.Monto *= -1;
            }

            await repositoryTransaccion.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);
            
            if (string.IsNullOrEmpty(modelo.UrlRetorno)) {
                return RedirectToAction("Index");
            } else {
                return LocalRedirect(modelo.UrlRetorno);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transaccion = await repositoryTransaccion.ObtenerPorId(id, usuarioId);

            if (transaccion is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(transaccion);
        }

        [HttpPost]
        public async Task<ActionResult> BorrarTransaccion(int id) {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var transaccion = await repositoryTransaccion.ObtenerPorId(id, usuarioId);

            if (transaccion is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryTransaccion.Borrar(id);

            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId) {
            var cuentas = await repositoryCuenta.Buscar(usuarioId);
            return cuentas
                .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion) {
            var categorias = await repositoryCategoria.Obtener(usuarioId, tipoOperacion);
            var resultado = categorias
                .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
                .ToList();

            var opcionPorDefecto = new SelectListItem("-- Seleccione una categoría --", "0", true);

            resultado.Insert(0, opcionPorDefecto);

            return resultado;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerTransacciones(int usuarioId, DateTime fechaInicio, DateTime fechaFin) {
            return await repositoryTransaccion.ObtenerPorUsuarioId(new ParametroObtenerTransaccionesPorUsuario() {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }
    }
}