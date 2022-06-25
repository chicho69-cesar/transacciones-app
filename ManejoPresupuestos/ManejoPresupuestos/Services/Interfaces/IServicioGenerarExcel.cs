using ManejoPresupuestos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Services.Interfaces {
    public interface IServicioGenerarExcel {
        FileResult GenerarExcel(string nombreArchivo, IEnumerable<Transaccion> transacciones);
    }
}