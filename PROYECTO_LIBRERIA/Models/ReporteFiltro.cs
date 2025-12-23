namespace PROYECTO_LIBRERIA.Models
{
    public class ReporteFiltro
    {
        public string Modo { get; set; } = "VENTAS_VENDEDOR";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdCliente { get; set; }
        public int? IdCategoria { get; set; }
        public string? SKU { get; set; }
    }
}
