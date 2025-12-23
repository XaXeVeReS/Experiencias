namespace PROYECTO_LIBRERIA.Models
{
    public class ProveedorModel
    {
        public int entidadId { get; set; }
        public int? idTipoDocumento { get; set; }
        public string? nroDocumento { get; set; }
        public string? nombreRazon { get; set; }
        public string? direccion { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }
        public bool? activo { get; set; }

        public string? codMensaje { get; set; }
        public string? mensaje { get; set; }

    }
}
