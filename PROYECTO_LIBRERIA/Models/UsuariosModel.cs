namespace LIBRERIA_APP.Models
{
    public class UsuariosModel
    {
        public int entidadId { get; set; }
        public int idTipoDocumento { get; set; }
        public string? nroDocumento { get; set; }
        public string? nombreRazon { get; set; }
        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public bool? activo { get; set; }
        public string? fechaRegistro { get; set; }
        public string? codigo { get; set; }
        public string? email { get; set; }

        public int idUsuario { get; set; }
        public string? usuario { get; set; }
        public string? contrasenia { get; set; }
        public string? ubigeo { get; set; }
        public int idrol { get; set; }
        public int entidadRolId { get; set; }





        public string? codMensaje { get; set; }
        public string? mensaje { get; set; }

        public string? rolDescripcion { get; set; }


        public string? correo { get; set; }

      //  public List<RolesModel> lstEntidadRol { get; set; }

    }



    public class RolModel
    {
        public int idrol { get; set; }
        public string? rolnombre { get; set; }
        public string? descripcion { get; set; }
        public string? Rol { get; set; }
    }


    public class TipoDocumentoModel
    {
        public int idTipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Siglas { get; set; }

    }

    public class IndicadoresModel
    {
        public int cantUsuarios { get; set; }
        public int usuariosActivos { get; set; }
        public int usuariosInactivos { get; set; }

        public int cantidadArea { get; set; }
        public int cantidadSubArea { get; set; }


    }

}
