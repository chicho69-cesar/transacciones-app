using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Services {
    public class RepositoryUsuarios : IRepositoryUsuarios {
        private readonly string connectionString;

        public RepositoryUsuarios(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario) {
            using var connection = new SqlConnection(connectionString);

            var usuarioId = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Usuario (Email, EmailNormalizado, PasswordHash)
                VALUES (@Email, @EmailNormalizado, @PasswordHash);
                SELECT SCOPE_IDENTITY();",
                usuario
            );

            await connection.ExecuteAsync(
                "CrearDatosUsuarioNuevo",
                new { usuarioId },
                commandType: System.Data.CommandType.StoredProcedure
            );

            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                @"SELECT * FROM Usuario 
                WHERE EmailNormalizado = @emailNormalizado;",
                new { emailNormalizado }
            );
        }

        public async Task<Usuario> BuscarUsuarioPorId(int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                @"SELECT * FROM Usuario 
                WHERE Id = @Id;",
                new { usuarioId }
            );
        }
    }
}