namespace ManejoPresupuestos.Models.ViewModels {
    public class ReporteMensualViewModel {
        public IEnumerable<ResultadoObtenerPorMes> TransaccionesPorMes { get; set; }
        public int Año { get; set; }

        public decimal Ingresos => 
            TransaccionesPorMes.Sum(x => x.Ingresos);

        public decimal Gastos =>
            TransaccionesPorMes.Sum(x => x.Gastos);

        public decimal Total => Ingresos - Gastos;
    }
}