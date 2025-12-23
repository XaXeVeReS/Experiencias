namespace PROYECTO_LIBRERIA.Models
{
    public class ProductoModel
    {
        public string? sku { get; set; }

        public string? nombre { get; set; }           // ProductoNombre
        public string? descripcion { get; set; }      // ProductoDescripcion

        public decimal? precio { get; set; }          // PrecioUnid
        public decimal? stockMin { get; set; }        // StockMin

        public int? idCategoria { get; set; }
        public string? nombreCategoria { get; set; }

        public int? entidadIdProveedor { get; set; }
        public string? nombreProveedor { get; set; }

        public string? estado { get; set; }

        // Mensajes de los SP (Stored Procedures)
        public string? codMensaje { get; set; }
        public string? mensaje { get; set; }
    }
}
