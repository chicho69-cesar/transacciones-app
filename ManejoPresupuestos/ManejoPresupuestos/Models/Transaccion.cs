using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models {
    public class Transaccion {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Display(Name = "Fecha de la transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));

        public decimal Monto { get; set; }

        [Display(Name = "Categoría de la transacción")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaId { get; set; }

        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede pasar de {1} caracteres")]
        public string Nota { get; set; }

        [Display(Name = "Cuenta de la transacción")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int CuentaId { get; set; }

        [Display(Name = "Tipo de operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Categoria { get; set; }

        public string Cuenta { get; set; }
    }
}