using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Models.ViewModels {
    public class CuentaCreacionViewModel : Cuenta {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}