using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Services {
    public class RepositoryCuenta : IRepositoryCuenta {
        private readonly string connectionString;

        public RepositoryCuenta(IConfiguration configuration) {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta) {
            using var connection = new SqlConnection(connectionString);
            
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Cuenta (Nombre, TipoCuentaId, Descripcion, Balance)
                VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                SELECT SCOPE_IDENTITY();",
                cuenta
            );

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(
                @"SELECT c.Id, c.Nombre, c.Balance, tc.Nombre as TipoCuenta 
                FROM Cuenta c INNER JOIN TipoCuenta tc
                ON tc.Id = c.TipoCuentaId
                WHERE tc.UsuarioId = @UsuarioId
                ORDER BY tc.Orden;",
                new { usuarioId }
            );
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"SELECT c.Id, c.Nombre, c.Balance, c.Descripcion, c.TipoCuentaId
                FROM Cuenta c INNER JOIN TipoCuenta tc
                ON tc.Id = c.TipoCuentaId
                WHERE tc.UsuarioId = @UsuarioId AND c.Id = @Id",
                new { id, usuarioId }
            );
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"UPDATE Cuenta
                SET Nombre = @Nombre, Balance = @Balance, 
                Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                WHERE Id = @Id;",
                cuenta
            );
        }

        public async Task Borrar(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"DELETE FROM Cuenta WHERE Id = @Id",
                new { id }
            );
        }
    }
}