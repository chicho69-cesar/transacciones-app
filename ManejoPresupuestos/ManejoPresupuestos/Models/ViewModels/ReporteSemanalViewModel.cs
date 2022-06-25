namespace ManejoPresupuestos.Models.ViewModels {
    public class ReporteSemanalViewModel {
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana> TransaccionesPorSemana { get; set; }

        public decimal Ingresos => TransaccionesPorSemana
            .Sum(t => t.Ingresos);
        
        public decimal Gastos => TransaccionesPorSemana
            .Sum(t => t.Gastos);

        public decimal Total => Ingresos - Gastos;
    }
}