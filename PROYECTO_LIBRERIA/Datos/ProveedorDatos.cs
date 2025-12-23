using LIBRERIA_APP.Datos;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Data.SqlClient;

namespace PROYECTO_LIBRERIA.Datos
{
    public class ProveedorDatos
    {
        Conexion cn = new Conexion();

        public List<ProveedorModel> ProveedorListaDatos()
        {
            var lista = new List<ProveedorModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorLista", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ProveedorModel
                        {
                            entidadId = Convert.ToInt32(rd["EntidadId"]),
                            idTipoDocumento = rd["IDTipoDocumento"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["IDTipoDocumento"]),
                            nroDocumento = rd["NroDocumento"]?.ToString(),
                            nombreRazon = rd["NombreRazon"]?.ToString(),
                            direccion = rd["Direccion"]?.ToString(),
                            email = rd["Email"]?.ToString(),
                            telefono = rd["Telefono"]?.ToString(),
                            activo = rd["Activo"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(rd["Activo"])
                        });
                    }
                }
            }

            return lista;
        }

        public List<ProveedorModel> ProveedorListaSimpleDatos()
        {
            var lista = new List<ProveedorModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorListaSimple", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ProveedorModel
                        {
                            entidadId = Convert.ToInt32(rd["EntidadId"]),
                            nombreRazon = rd["NombreRazon"]?.ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public ProveedorModel ProveedorObtenerDatos(int entidadId)
        {
            var prov = new ProveedorModel();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorObtener", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EntidadId", entidadId);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prov.entidadId = Convert.ToInt32(rd["EntidadId"]);
                        prov.idTipoDocumento = rd["IDTipoDocumento"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["IDTipoDocumento"]);
                        prov.nroDocumento = rd["NroDocumento"]?.ToString();
                        prov.nombreRazon = rd["NombreRazon"]?.ToString();
                        prov.direccion = rd["Direccion"]?.ToString();
                        prov.email = rd["Email"]?.ToString();
                        prov.telefono = rd["Telefono"]?.ToString();
                        prov.activo = rd["Activo"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(rd["Activo"]);
                    }
                }
            }

            return prov;
        }

        public ProveedorModel ProveedorCrearDatos(ProveedorModel prov)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorCrear", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@IDTipoDocumento", (object?)prov.idTipoDocumento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NroDocumento", prov.nroDocumento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreRazon", prov.nombreRazon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", prov.direccion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", prov.email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", prov.telefono ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prov.codMensaje = rd["CodMensaje"].ToString();
                        prov.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return prov;
        }

        public ProveedorModel ProveedorActualizarDatos(ProveedorModel prov)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorActualizar", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@EntidadId", prov.entidadId);
                cmd.Parameters.AddWithValue("@IDTipoDocumento", (object?)prov.idTipoDocumento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NroDocumento", prov.nroDocumento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreRazon", prov.nombreRazon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", prov.direccion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", prov.email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", prov.telefono ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        prov.codMensaje = rd["CodMensaje"].ToString();
                        prov.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return prov;
        }

        public ProveedorModel ProveedorCambiarEstadoDatos(int entidadId, bool activo)
        {
            var resp = new ProveedorModel { entidadId = entidadId, activo = activo };

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_ProveedorCambiarEstado", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@EntidadId", entidadId);
                cmd.Parameters.AddWithValue("@Activo", activo);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        resp.codMensaje = rd["CodMensaje"].ToString();
                        resp.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return resp;
        }
    }
}
