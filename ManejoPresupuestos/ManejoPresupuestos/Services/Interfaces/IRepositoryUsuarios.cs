using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Services.Interfaces {
    public interface IRepositoryUsuarios {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<Usuario> BuscarUsuarioPorId(int usuarioId);
        Task<int> CrearUsuario(Usuario usuario);
    }
}