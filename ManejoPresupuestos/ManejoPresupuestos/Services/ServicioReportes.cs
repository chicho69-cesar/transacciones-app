using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;

namespace ManejoPresupuestos.Services {
    public class ServicioReportes : IServicioReportes {
        private readonly IRepositoryTransaccion repositoryTransaccion;
        private readonly HttpContext httpContext;

        public ServicioReportes(
            IRepositoryTransaccion repositoryTransaccion,
            IHttpContextAccessor httpContextAccessor
        ) {
            this.repositoryTransaccion = repositoryTransaccion;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemana(int usuarioId, int mes, int año, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var parametro = new ParametroObtenerTransaccionesPorUsuario() {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            var modelo = await repositoryTransaccion.ObtenerPorSemana(parametro);

            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var parametro = new ParametroObtenerTransaccionesPorUsuario() {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositoryTransaccion.ObtenerPorUsuarioId(parametro);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);

            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var obtenerTransaccionesPorCuenta = new ParametroObtenerTransaccionesPorCuenta() {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositoryTransaccion.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            
            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        private static ReporteTransaccionesDetalladas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones) {
            var modelo = new ReporteTransaccionesDetalladas();

            var transaccionesPorFecha = transacciones
                .OrderByDescending(t => t.FechaTransaccion)
                .GroupBy(t => t.FechaTransaccion)
                .Select(group => new ReporteTransaccionesDetalladas.TransaccionesPorFecha() {
                    FechaTransaccion = group.Key,
                    Transacciones = group.AsEnumerable()
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            
            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic ViewBag, DateTime fechaInicio) {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.AñoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.UrlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int año) {
            DateTime fechaInicio, fechaFin;

            if (mes <= 0 || mes > 12 || año <= 1900) {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            } else {
                fechaInicio = new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}