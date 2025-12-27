namespace PROYECTO_LIBRERIA.Models
{
    public class ProductoModel
    {
        public string? sku { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }

  
        public decimal? precioSinIGV { get; set; } 
        public decimal? igv { get; set; }          
        public decimal? precio { get; set; }        

        public decimal? stockMin { get; set; }
        public int? idCategoria { get; set; }
        public string? nombreCategoria { get; set; }
        public int? entidadIdProveedor { get; set; }
        public string? nombreProveedor { get; set; }
        public string? estado { get; set; }

        public string? codMensaje { get; set; }
        public string? mensaje { get; set; }
    }
}