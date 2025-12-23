using LIBRERIA_APP.Datos;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Data.SqlClient; // Necesario para SQL Server

namespace PROYECTO_LIBRERIA.Datos
{
    public class ReporteDatos
    {
        // 1. Instanciamos tu clase Conexion
        private readonly Conexion _conexion = new Conexion();

        // 2. Método auxiliar: Obtiene el string de tu clase y crea el objeto SqlConnection
        private SqlConnection GetConnection()
        {
            string cadenaConexion = _conexion.GetCadenaSQL();
            return new SqlConnection(cadenaConexion);
        }

        public async Task<List<Dictionary<string, object>>> EjecutarReporteAsync(ReporteFiltro filtro)
        {
            var lista = new List<Dictionary<string, object>>();

            // Usamos el método GetConnection() que acabamos de definir
            using var cn = GetConnection();
            using var cmd = new SqlCommand("sp_ReportesMaster", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Modo", filtro.Modo);
            cmd.Parameters.AddWithValue("@FechaInicio", (object?)filtro.FechaInicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaFin", (object?)filtro.FechaFin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdUsuario", (object?)filtro.IdUsuario ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdCliente", (object?)filtro.IdCliente ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdCategoria", (object?)filtro.IdCategoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SKU", (object?)filtro.SKU ?? DBNull.Value);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var dic = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    var val = dr.GetValue(i);
                    dic[dr.GetName(i)] = val == DBNull.Value ? null! : val;
                }
                lista.Add(dic);
            }

            return lista;
        }

        public async Task<List<(int Id, string Nombre)>> BuscarUsuariosAsync(string filtro)
        {
            var lista = new List<(int, string)>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT IdUsuario, Usuario
                FROM tbUsuarios 
                WHERE Usuario LIKE @filtro + '%'
                ORDER BY Usuario;", cn);

            cmd.Parameters.AddWithValue("@filtro", filtro ?? string.Empty);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add((dr.GetInt32(0), dr.GetString(1)));
            }
            return lista;
        }

        public async Task<List<(int Id, string Nombre)>> BuscarClientesAsync(string filtro)
        {
            var lista = new List<(int, string)>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT EntidadId, NombreRazon
                FROM tbEntidad
                WHERE NombreRazon LIKE @filtro + '%'
                ORDER BY NombreRazon;", cn);

            cmd.Parameters.AddWithValue("@filtro", filtro ?? string.Empty);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add((dr.GetInt32(0), dr.GetString(1)));
            }
            return lista;
        }

        public async Task<List<(int Id, string Nombre)>> BuscarCategoriasAsync(string filtro)
        {
            var lista = new List<(int, string)>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT IDCategoria, Nombre
                FROM tbCategoria
                WHERE Nombre LIKE @filtro + '%'
                ORDER BY Nombre;", cn);

            cmd.Parameters.AddWithValue("@filtro", filtro ?? string.Empty);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add((dr.GetInt32(0), dr.GetString(1)));
            }
            return lista;
        }

        public async Task<List<(string Id, string Nombre)>> BuscarProductosAsync(string filtro, int? idCategoria)
        {
            var lista = new List<(string, string)>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT SKU, ProductoNombre
                FROM tbProducto
                WHERE (@idCat IS NULL OR IDCategoria = @idCat)
                  AND (ProductoNombre LIKE @filtro + '%' OR SKU LIKE @filtro + '%')
                ORDER BY ProductoNombre;", cn);

            cmd.Parameters.AddWithValue("@idCat", (object?)idCategoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@filtro", filtro ?? string.Empty);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                lista.Add((dr.GetString(0), dr.GetString(1)));
            }
            return lista;
        }
    }
}