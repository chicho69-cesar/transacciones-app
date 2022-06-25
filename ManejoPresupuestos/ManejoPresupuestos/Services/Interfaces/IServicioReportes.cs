using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Services.Interfaces {
    public interface IServicioReportes {
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemana(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetalladas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag);
    }
}