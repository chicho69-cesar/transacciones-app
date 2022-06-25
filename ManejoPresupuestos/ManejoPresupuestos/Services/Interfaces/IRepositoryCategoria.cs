using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;

namespace ManejoPresupuestos.Services.Interfaces {
    public interface IRepositoryCategoria {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task<int> Contar(int usuarioId);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacion);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }
}