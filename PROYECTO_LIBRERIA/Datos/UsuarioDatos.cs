using LIBRERIA_APP.Models;

using System.Data;
using System.Data.SqlClient;

namespace LIBRERIA_APP.Datos
{
    public class UsuarioDatos
    {

        UsuariosModel Usuaro = new UsuariosModel();
        IndicadoresModel indicadores = new IndicadoresModel();
        Conexion cn = new Conexion();
        public UsuariosModel IniciarSesion(string xUsuario, string xContrasenia)

        {
     
            var _Login = new UsuariosModel();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_UsuarioLogeo", conexion);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("user", xUsuario);
                cmd.Parameters.AddWithValue("pass", xContrasenia);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        _Login.codMensaje = rd["codMensaje"].ToString();
                        _Login.mensaje = rd["Mensaje"].ToString();
                        _Login.usuario = rd["Usuario"].ToString();
                        _Login.nombreRazon = rd["NombreRazon"].ToString();
                        _Login.rolDescripcion= rd["Codigo"].ToString();
                        _Login.idUsuario =Convert.ToInt32( rd["IDusuario"].ToString());


                        // _Login.SesionActivo =Convert.ToBoolean( (rd["SesionActivo"].ToString()));
                    }
                }
            }
            return _Login;

        }

        public List<UsuariosModel> UsuariosListaGeneral_Datos()
        {
            List<UsuariosModel> listaUsuarios = new List<UsuariosModel>();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_UsuarioListaGeneral", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        listaUsuarios.Add(new UsuariosModel
                        {
                            entidadId = Convert.ToInt32(rd["EntidadId"]),
                            usuario = rd["Usuario"].ToString(),
                            nroDocumento = rd["NroDocumento"].ToString(),
                            nombreRazon = rd["NombreRazon"].ToString(),
                            codigo = rd["Codigo"].ToString(),
                            email = rd["Email"].ToString(),
                            activo = Convert.ToBoolean( rd["Activo"].ToString()),
                            fechaRegistro = rd["FechaRegistro"].ToString()
                        });
                    }
                }

                conexion.Close();
            }
            return listaUsuarios;
        }



        public List<TipoDocumentoModel> TipoDocumentoListaDatos()
        {
            List<TipoDocumentoModel> Lista = new List<TipoDocumentoModel>();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_TipoDocumentoLista", conexion);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Lista.Add(new TipoDocumentoModel
                    {
                        idTipoDocumento = Convert.ToInt32(rd["IDTipoDocumento"].ToString()),
                        Documento = rd["Documento"].ToString(),
                        Siglas = rd["Siglas"].ToString()
                    });
                }
            }
            return Lista;
        }
        public List<RolModel> RolListaDatos()
        {
            List<RolModel> Lista = new List<RolModel>();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_RolLista", conexion);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Lista.Add(new RolModel
                    {
                        idrol = Convert.ToInt32(rd["RolId"].ToString()),
                        rolnombre = rd["Codigo"].ToString(),
                        descripcion = rd["Descripcion"].ToString()
                    });
                }
            }
            return Lista;
        }


        public IndicadoresModel UsuariosIndicadoresDatos()
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("uspUsuarioIndicadores", conexion);

                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    indicadores.cantUsuarios = Convert.ToInt32(rd["CantUsuarios"].ToString());
                    indicadores.usuariosActivos = Convert.ToInt32(rd["UsuariosActivos"].ToString());
                    indicadores.usuariosInactivos = Convert.ToInt32(rd["UsuariosInactivos"].ToString());
                    //indicadores.cantidadArea = Convert.ToInt32(rd["CantidadArea"].ToString());
                    //indicadores.cantidadSubArea = Convert.ToInt32(rd["CantidadSubArea"].ToString());
                }
            }
            return indicadores;

        }


        public UsuariosModel UsuarioCrearDatos(UsuariosModel use) 
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL())) 
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_UsuarioCrear", conexion);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("IDtipoDocumento", use.idTipoDocumento);
                cmd.Parameters.AddWithValue("NroDocumento", use.nroDocumento);
                cmd.Parameters.AddWithValue("NombreRazon", use.nombreRazon);
                cmd.Parameters.AddWithValue("Direccion", use.direccion);
                cmd.Parameters.AddWithValue("Ubigeo", use.ubigeo);
                cmd.Parameters.AddWithValue("Email", use.email);
                cmd.Parameters.AddWithValue("Telefono", use.telefono);
                cmd.Parameters.AddWithValue("Usuario", use.usuario);
                cmd.Parameters.AddWithValue("Contrasenia", use.contrasenia);
                cmd.Parameters.AddWithValue("IDrol", use.idrol);
                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                if (rd.Read()) 
                {
                    Usuaro.codMensaje = rd["CodMensaje"].ToString();
                    Usuaro.mensaje = rd["Mensaje"].ToString();
                }
            }
            return Usuaro;
        }


        public UsuariosModel uUsuarioObtenerDatos(string id)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_UsuarioObtener", conexion);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    Usuaro.entidadId =Convert.ToInt32( rd["EntidadId"].ToString());
                    Usuaro.idTipoDocumento = Convert.ToInt32( rd["IDTipoDocumento"].ToString());
                    Usuaro.nroDocumento = rd["NroDocumento"].ToString();
                    Usuaro.nombreRazon = rd["NombreRazon"].ToString();
                    Usuaro.direccion = rd["Direccion"].ToString();
                    Usuaro.ubigeo = rd["Ubigeo"].ToString();
                    Usuaro.email = rd["Email"].ToString();
                    Usuaro.telefono = rd["Telefono"].ToString();
                    Usuaro.usuario = rd["Usuario"].ToString();
                    Usuaro.contrasenia = rd["Contrasenia"].ToString();
                    Usuaro.idrol = Convert.ToInt32(rd["RolId"].ToString());
                }
            }
            return Usuaro;
        }


        public UsuariosModel UsuarioActualizarDatos(UsuariosModel use)
        {
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                var cmd = new SqlCommand("usp_UsuarioActualizar", conexion);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ID", use.entidadId);
                cmd.Parameters.AddWithValue("IDtipoDocumento", use.idTipoDocumento);
                cmd.Parameters.AddWithValue("NroDocumento", use.nroDocumento);
                cmd.Parameters.AddWithValue("NombreRazon", use.nombreRazon);
                cmd.Parameters.AddWithValue("Direccion", use.direccion);
                cmd.Parameters.AddWithValue("Ubigeo", use.ubigeo);
                cmd.Parameters.AddWithValue("Email", use.email);
                cmd.Parameters.AddWithValue("Telefono", use.telefono);
                cmd.Parameters.AddWithValue("Usuario", use.usuario);
                cmd.Parameters.AddWithValue("Contrasenia", use.contrasenia);
                cmd.Parameters.AddWithValue("IDrol", use.idrol);
                cmd.Parameters.AddWithValue("@Activo", use.activo);
                cmd.CommandType = CommandType.StoredProcedure;
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    Usuaro.codMensaje = rd["CodMensaje"].ToString();
                    Usuaro.mensaje = rd["Mensaje"].ToString();
                }
            }
            return Usuaro;
        }


    }
}
