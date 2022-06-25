using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Models.ViewModels;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Services {
    public class RepositoryCategoria : IRepositoryCategoria {
        private readonly string connectionString;

        public RepositoryCategoria(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria) {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Categoria (Nombre, TipoOperacionId, UsuarioId)
                VALUES(@Nombre, @TipoOperacionId, @UsuarioId); 
                SELECT SCOPE_IDENTITY();",
                categoria
            );

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>(
                @$"SELECT * FROM Categoria 
                WHERE UsuarioId = @UsuarioId
                ORDER BY Nombre
                OFFSET {paginacion.RecordsASaltar} 
                ROWS FETCH NEXT {paginacion.RecordsPorPagina}
                ROWS ONLY;",
                new { usuarioId }
            );
        }

        public async Task<int> Contar(int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.ExecuteScalarAsync<int>(
                @"SELECT count(*) FROM Categoria 
                WHERE UsuarioId = @usuarioId;",
                new { usuarioId }
            );
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>(
                @"SELECT * FROM Categoria 
                WHERE UsuarioId = @UsuarioId AND TipoOperacionId = @TipoOperacionId;",
                new { usuarioId, tipoOperacionId }
            );
        }

        public async Task<Categoria> ObtenerPorId(int id, int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                @"SELECT * FROM Categoria 
                WHERE Id = @Id AND UsuarioId = @UsuarioId;",
                new { id, usuarioId }
            );
        }

        public async Task Actualizar(Categoria categoria) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"UPDATE Categoria
                SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                WHERE Id = @Id;",
                categoria
            );
        }

        public async Task Borrar(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"DELETE FROM Categoria 
                WHERE Id = @Id;",
                new { id }
            );
        }
    }
}