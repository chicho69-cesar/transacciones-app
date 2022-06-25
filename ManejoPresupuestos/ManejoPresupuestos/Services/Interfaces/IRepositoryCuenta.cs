using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;

namespace ManejoPresupuestos.Services.Interfaces {
    public interface IRepositoryCuenta {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
}