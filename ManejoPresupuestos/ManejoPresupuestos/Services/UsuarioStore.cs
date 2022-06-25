using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuestos.Services {
    public class UsuarioStore : IUserStore<Usuario>, IUserEmailStore<Usuario>, IUserPasswordStore<Usuario> {
        private readonly IRepositoryUsuarios repositoryUsuarios;

        public UsuarioStore(IRepositoryUsuarios repositoryUsuarios) {
            this.repositoryUsuarios = repositoryUsuarios;
        }
        
        public async Task<IdentityResult> CreateAsync(Usuario user, CancellationToken cancellationToken) {
            user.Id = await repositoryUsuarios.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(Usuario user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(Usuario user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<Usuario> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            return await repositoryUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public async Task<Usuario> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            return await repositoryUsuarios.BuscarUsuarioPorId(int.Parse(userId));
        }

        public async Task<Usuario> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            return await repositoryUsuarios.BuscarUsuarioPorEmail(normalizedUserName);
        }

        public Task<string> GetEmailAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }

        public Task<string> GetNormalizedEmailAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.EmailNormalizado);
        }

        public Task<string> GetNormalizedUserNameAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<string> GetPasswordHashAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(Usuario user, CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }

        public Task SetEmailAsync(Usuario user, string email, CancellationToken cancellationToken) {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(Usuario user, bool confirmed, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(Usuario user, string normalizedEmail, CancellationToken cancellationToken) {
            user.EmailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(Usuario user, string normalizedName, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuario user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuario user, string userName, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}