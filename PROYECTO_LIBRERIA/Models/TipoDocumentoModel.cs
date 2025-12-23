namespace PROYECTO_LIBRERIA.Models
{
    public class TipoDocumentoModel
    {
        public int idTipoDocumento { get; set; }
        public string? documento { get; set; }
        public string? siglas { get; set; }
        public int? longMin { get; set; }
        public int? longMax { get; set; }
    }
}
