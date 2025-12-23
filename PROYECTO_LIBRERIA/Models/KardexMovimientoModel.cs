
namespace PROYECTO_LIBRERIA.Models
{
    public class KardexMovimientoModel
    {
        public int idMovimiento { get; set; }
        public DateTime fecha { get; set; }
        public string? sku { get; set; }
        public string? productoNombre { get; set; }
        public string? tipo { get; set; }          // IN / OUT
        public decimal cantidad { get; set; }
        public decimal? costoUnit { get; set; }
        public string? origen { get; set; }
        public string? refTabla { get; set; }
        public int? refId { get; set; }
        public decimal saldo { get; set; }
    }
}
