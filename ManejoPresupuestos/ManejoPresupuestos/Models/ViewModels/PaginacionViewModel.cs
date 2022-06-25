namespace ManejoPresupuestos.Models.ViewModels {
    public class PaginacionViewModel {
        private int recordsPorPagina = 10;
        private int cantidadMaximaRecordsPorPagina = 50;
        
        public int Pagina { get; set; } = 1;
        
        public int RecordsPorPagina { 
            get {
                return recordsPorPagina;
            }

            set {
                recordsPorPagina = value > cantidadMaximaRecordsPorPagina
                ? cantidadMaximaRecordsPorPagina
                : value;
            }
        }
        
        public int RecordsASaltar => recordsPorPagina * (Pagina - 1);
    }
}