using System;
using System.Collections.Generic;

namespace LIBRERIA_APP.Models
{
    public class LibreriaModel
    {
        public int IdCabecera { get; set; }
        public int IdUsuario { get; set; }
        public int EntidadCliente { get; set; }
        public int IdEmpresa { get; set; }

        public string? Direccion { get; set; }
        public string? TipoMoneda { get; set; }
        public string? TipoDocumento { get; set; }
        public decimal ImporteTotal { get; set; }
        public DateTime? FechaValidez { get; set; }

        // Relación Venta ← Cotización
        public int IdCabeceraOrigen { get; set; }
        public int DiasValidez { get; set; }

        // Datos del cliente según formulario
        public int IdTipoDocumentoCliente { get; set; }
        public string? NroDocumentoCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? DireccionCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string? TelefonoCliente { get; set; }

        public List<DetalleVentaModel> Detalle { get; set; } = new List<DetalleVentaModel>();

        // Resultado de SP
        public string? CodMensaje { get; set; }
        public string? Mensaje { get; set; }
    }

    public class DetalleVentaModel
    {
        public int Item { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
    }

    public class ClienteModel
    {
        public int EntidadId { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NroDocumento { get; set; } = string.Empty;
        public string NombreRazon { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}
