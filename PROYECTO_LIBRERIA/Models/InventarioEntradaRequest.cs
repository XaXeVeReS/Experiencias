
namespace PROYECTO_LIBRERIA.Models
{
    public class InventarioEntradaRequest
    {
        public string? origen { get; set; } // AJUSTE / COMPRA
        public List<InventarioEntradaItemModel>? detalle { get; set; }
    }
}