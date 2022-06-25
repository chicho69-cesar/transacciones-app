using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuestos.Services {
    public class RepositoryTransaccion : IRepositoryTransaccion {
        private readonly string connectionString;

        public RepositoryTransaccion(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion) {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(
                "Transacciones_insertar",
                new {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: CommandType.StoredProcedure
            );

            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"Transacciones_actualizar",
                new {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT t.*, c.TipoOperacionId FROM Transaccion t
                INNER JOIN Categoria c
                ON t.CategoriaId = c.Id
                WHERE t.Id = @Id AND t.UsuarioId = @UsuarioId;",
                new { id, usuarioId }
            );
        }

        public async Task Borrar(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(
                @"Transacciones_borrar",
                new { id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ParametroObtenerTransaccionesPorCuenta modelo) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, 
                c.Nombre as Categoria, cu.Nombre as Cuenta, 
                c.TipoOperacionId
                FROM Transaccion t INNER JOIN Categoria c
                ON t.CategoriaId = c.Id INNER JOIN Cuenta cu
                ON t.CuentaId = cu.Id
                WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin;",
                modelo
            );
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, t.Nota, 
                c.Nombre as Categoria, cu.Nombre as Cuenta, 
                c.TipoOperacionId
                FROM Transaccion t INNER JOIN Categoria c
                ON t.CategoriaId = c.Id INNER JOIN Cuenta cu
                ON t.CuentaId = cu.Id
                WHERE t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY t.FechaTransaccion DESC;",
                modelo
            );
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorSemana>(
                @"SELECT datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 AS Semana,
                SUM(Monto) AS Monto, c.TipoOperacionId
                FROM Transaccion t INNER JOIN Categoria c ON t.CategoriaId = c.Id
                WHERE t.UsuarioId = @UsuarioId and
                FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                GROUP BY datediff(d, @fechaInicio, FechaTransaccion) / 7, c.TipoOperacionId;",
                modelo
            );
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoObtenerPorMes>(
                @"SELECT month(t.FechaTransaccion) AS Mes,
                sum(t.Monto) AS Monto, c.TipoOperacionId
                FROM Transaccion t INNER JOIN Categoria c 
                ON c.Id = t.CategoriaId 
                WHERE T.UsuarioId = @UsuarioId AND year(t.FechaTransaccion) = @Año 
                GROUP BY month(t.FechaTransaccion), c.TipoOperacionId;",
                new { usuarioId, año }
            );
        }
    }
}