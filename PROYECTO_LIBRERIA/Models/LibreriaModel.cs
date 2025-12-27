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
        public string? Estado { get; set; }
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
        public decimal SubTotal { get; set; }
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


    /// <summary>
    /// nuevos ingresos  tablas para usar api
    /// </summary>
    public class InvoiceRequest
    {
        public string? ublVersion { get; set; }
        public string? tipoOperacion { get; set; }
        public string? tipoDoc { get; set; }
        public string? serie { get; set; }
        public string? correlativo { get; set; }
        public DateTime fechaEmision { get; set; }
        public FormaPago? formaPago { get; set; }
        public string? tipoMoneda { get; set; }
        public Client? client { get; set; }
        public Company? company { get; set; }
        public decimal mtoOperGratuitas { get; set; }
        public decimal mtoIGVGratuitas { get; set; }
        public decimal mtoIGV { get; set; }
        public decimal totalImpuestos { get; set; }
        public decimal valorVenta { get; set; }
        public decimal subTotal { get; set; }
        public decimal mtoImpVenta { get; set; }
        public List<Detail>? details { get; set; }
        public List<Legend>? legends { get; set; }
    }

    public class FormaPago
    {
        public string? moneda { get; set; }
        public string? tipo { get; set; }
    }

    public class Client
    {
        public string? tipoDoc { get; set; }
        public long numDoc { get; set; }
        public string? rznSocial { get; set; }
        public Address? address { get; set; }
    }

    public class Company
    {
        public long ruc { get; set; }
        public string? razonSocial { get; set; }
        public string? nombreComercial { get; set; }
        public Address? address { get; set; }
    }

    public class Address
    {
        public string? direccion { get; set; }
        public string? provincia { get; set; }
        public string? departamento { get; set; }
        public string? distrito { get; set; }
        public string? ubigueo { get; set; }
    }

    public class Detail
    {
        public string? codProducto { get; set; }
        public string? unidad { get; set; }
        public string? descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal mtoValorUnitario { get; set; }
        public decimal mtoValorGratuito { get; set; }
        public decimal mtoValorVenta { get; set; }
        public decimal mtoBaseIgv { get; set; }
        public decimal porcentajeIgv { get; set; }
        public decimal igv { get; set; }
        public int tipAfeIgv { get; set; }
        public decimal totalImpuestos { get; set; }
        public decimal mtoPrecioUnitario { get; set; }
    }

    public class Legend
    {
        public string? code { get; set; }
        public string? value { get; set; }
    }

    public class InvoiceApiResponse
    {
        public SunatResponse? sunatResponse { get; set; }
        public string? xml { get; set; }
        public string? pdf { get; set; }
    }

    public class SunatResponse
    {
        public bool success { get; set; }
        public CdrResponse? cdrResponse { get; set; }
    }

    public class CdrResponse
    {
        public string? code { get; set; }
        public string? description { get; set; }
    }
    public class ApiModel
    {
        public string? ruc { get; set; }
        public string? razonSocial { get; set; }
        public string? estado { get; set; }
        public string? condicion { get; set; }
        public string? direccion { get; set; }
        public string? departamento { get; set; }
        public string? provincia { get; set; }
        public string? distrito { get; set; }
        public string? ubigeo { get; set; }
        public string? capital { get; set; }
        public string? direccionUbigeo { get; set; }
    }
}
