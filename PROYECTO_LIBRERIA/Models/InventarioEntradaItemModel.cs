namespace PROYECTO_LIBRERIA.Models
{
    public class InventarioEntradaItemModel
    {
        public string? sku { get; set; }
        public string? nombre { get; set; }       // solo para mostrar
        public decimal cantidad { get; set; }
        public decimal? costoUnit { get; set; }
    }
}
