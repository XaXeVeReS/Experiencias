namespace PROYECTO_LIBRERIA.Models
{
    public class CategoriaModel
    {
        public int idCategoria { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }

        // Para mensajes de los SP (Stored Procedure)
        public string? codMensaje { get; set; }
        public string? mensaje { get; set; }

    }
}
