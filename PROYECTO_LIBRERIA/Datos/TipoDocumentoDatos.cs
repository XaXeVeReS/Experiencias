using LIBRERIA_APP.Datos;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Data.SqlClient;

namespace PROYECTO_LIBRERIA.Datos
{
    public class TipoDocumentoDatos
    {
        Conexion cn = new Conexion();

        public List<TipoDocumentoModel> TipoDocumentoListaDatos()
        {
            var lista = new List<TipoDocumentoModel>();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_TipoDocumentoLista", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new TipoDocumentoModel
                        {
                            idTipoDocumento = Convert.ToInt32(rd["IDTipoDocumento"]),
                            documento = rd["Documento"]?.ToString(),
                            siglas = rd["Siglas"]?.ToString(),
                            longMin = rd["LongMin"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["LongMin"]),
                            longMax = rd["LongMax"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["LongMax"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
