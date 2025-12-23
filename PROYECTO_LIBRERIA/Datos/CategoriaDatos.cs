using LIBRERIA_APP.Datos;
using LIBRERIA_APP.Models;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Data.SqlClient;

namespace PROYECTO_LIBRERIA.Datos
{
    public class CategoriaDatos
    {

        Conexion cn = new Conexion();

        #region Listar Categoria
        public List<CategoriaModel> CategoriaListaDatos()
        {
            List<CategoriaModel> lista = new List<CategoriaModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_CategoriaLista", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new CategoriaModel
                        {
                            idCategoria = Convert.ToInt32(rd["IDCategoria"]),
                            nombre = rd["Nombre"].ToString(),
                            descripcion = rd["Descripcion"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
        #endregion

        #region Obtener Categoria por ID

        public CategoriaModel CategoriaObtenerDatos(int idCategoria)
        {
            CategoriaModel cat = new CategoriaModel();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_CategoriaObtener", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@IDCategoria", idCategoria);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        cat.idCategoria = Convert.ToInt32(rd["IDCategoria"]);
                        cat.nombre = rd["Nombre"].ToString();
                        cat.descripcion = rd["Descripcion"].ToString();
                    }
                }
            }

            return cat;
        }

        #endregion


        #region Crear Categoria
        public CategoriaModel CategoriaCrearDatos(CategoriaModel cat)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_CategoriaCrear", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Nombre", cat.nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", cat.descripcion ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        cat.codMensaje = rd["CodMensaje"].ToString();
                        cat.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return cat;
        }

        #endregion


        #region Actualizar Categoria
        public CategoriaModel CategoriaActualizarDatos(CategoriaModel cat)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_CategoriaActualizar", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@IDCategoria", cat.idCategoria);
                cmd.Parameters.AddWithValue("@Nombre", cat.nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", cat.descripcion ?? (object)DBNull.Value);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        cat.codMensaje = rd["CodMensaje"].ToString();
                        cat.mensaje = rd["Mensaje"].ToString();
                    }
                }
            }

            return cat;
        }
        #endregion

    }
}
