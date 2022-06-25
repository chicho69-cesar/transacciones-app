using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Services {
    public class RepositoryTipoCuenta : IRepositoryTipoCuenta {
        private readonly string connectionString;

        public RepositoryTipoCuenta(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta) {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int> (
                "TipoCuenta_Insertar",
                new { 
                    usuarioId = tipoCuenta.UsuarioId,
                    nombre = tipoCuenta.Nombre
                },
                commandType: System.Data.CommandType.StoredProcedure
            );

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0) {
            using var connection = new SqlConnection(connectionString);

            var existe = await connection.QueryFirstOrDefaultAsync<int> (
                @"SELECT 1 FROM TipoCuenta
                WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId
                AND Id != @id;",
                new { nombre, usuarioId, id }
            );

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId) {
            using var connection = new SqlConnection(connectionString);
            
            return await connection.QueryAsync<TipoCuenta> (
                @"SELECT Id, Nombre, Orden FROM TipoCuenta
                WHERE UsuarioId = @UsuarioId
                ORDER BY Orden ASC;",
                new { usuarioId }
            );
        }

        public async Task Actualizar(TipoCuenta tipoCuenta) {
            using var connection = new SqlConnection(connectionString);
            
            await connection.ExecuteAsync (
                @"UPDATE TipoCuenta
                SET Nombre = @Nombre
                WHERE Id = @Id;",
                tipoCuenta
            );
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId) {
            using var connection = new SqlConnection(connectionString);
            
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
                @"SELECT Id, Nombre, Orden
                FROM TipoCuenta
                WHERE Id = @Id AND UsuarioId = @UsuarioId;",
                new { id, usuarioId }
            );
        }

        public async Task Borrar(int id) {
            using var connection = new SqlConnection(connectionString);
            
            await connection.ExecuteAsync(
                @"DELETE FROM TipoCuenta WHERE Id = @Id;",
                new { id }
            );
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados) {
            using var connection = new SqlConnection(connectionString);
            
            await connection.ExecuteAsync(
                @"UPDATE TipoCuenta SET Orden = @Orden WHERE Id = @Id;", 
                tipoCuentasOrdenados
            );
        }
    }
}