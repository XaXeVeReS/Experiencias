using PROYECTO_LIBRERIA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PROYECTO_LIBRERIA.Datos;
using System.Text.Json;
using LIBRERIA_APP.Models;

namespace PROYECTO_LIBRERIA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerarDocController : ControllerBase
    {
        [HttpPost("FacturaElectronicaGenerar")]
        public IActionResult FacturaElectronicaGenerar([FromBody] InvoiceRequest invoice)
        {
            // ===============================
            // VALIDACIONES PRODUCCIÓN
            // ===============================
            if (invoice == null)
                return BadRequest("Solicitud inválida.");

            if (string.IsNullOrEmpty(invoice.serie) || string.IsNullOrEmpty(invoice.correlativo))
                return BadRequest("Serie y correlativo son obligatorios.");

            if (invoice.client == null || string.IsNullOrEmpty(invoice.client.rznSocial))
                return BadRequest("Datos del cliente incompletos.");

            if (invoice.details == null || !invoice.details.Any())
                return BadRequest("La factura no tiene detalles.");

            if (invoice.details.Any(d => d.cantidad <= 0 || d.mtoPrecioUnitario <= 0))
                return BadRequest("Cantidad y precio unitario deben ser mayores a cero.");

            QuestPDF.Settings.License = LicenseType.Community;

            byte[] pdf;

            try
            {
                pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Content().Column(col =>
                        {
                            // ===============================
                            // CABECERA
                            // ===============================
                            col.Item().AlignCenter()
                                .Text("FACTURA ELECTRÓNICA")
                                .Bold().FontSize(18);

                            col.Item().Text($"Serie: {invoice.serie}-{invoice.correlativo}");
                            col.Item().Text($"Fecha emisión: {invoice.fechaEmision:dd/MM/yyyy HH:mm}");
                            col.Item().Text($"Moneda: {invoice.tipoMoneda}");

                            col.Item().PaddingVertical(5);

                            // ===============================
                            // DATOS EMPRESA
                            // ===============================
                            col.Item().Text($"Empresa: {invoice.company?.razonSocial}");
                            col.Item().Text($"RUC: {invoice.company?.ruc}");
                            col.Item().Text($"Dirección: {invoice.company?.address?.direccion}");

                            col.Item().PaddingVertical(5);

                            // ===============================
                            // DATOS CLIENTE
                            // ===============================
                            col.Item().Text($"Cliente: {invoice.client.rznSocial}");
                            col.Item().Text($"Documento: {invoice.client.tipoDoc} - {invoice.client.numDoc}");
                            col.Item().Text($"Dirección: {invoice.client.address?.direccion}");

                            col.Item().PaddingVertical(10);

                            // ===============================
                            // TABLA DETALLE
                            // ===============================
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(40); // Item
                                    c.RelativeColumn();   // Descripción
                                    c.ConstantColumn(60); // Cantidad
                                    c.ConstantColumn(80); // P. Unit
                                    c.ConstantColumn(80); // Subtotal
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Background(Colors.Black).AlignCenter().Text("ITEM").FontColor(Colors.White);
                                    h.Cell().Background(Colors.Black).AlignCenter().Text("DESCRIPCIÓN").FontColor(Colors.White);
                                    h.Cell().Background(Colors.Black).AlignCenter().Text("CANT.").FontColor(Colors.White);
                                    h.Cell().Background(Colors.Black).AlignCenter().Text("P. UNIT").FontColor(Colors.White);
                                    h.Cell().Background(Colors.Black).AlignCenter().Text("SUBTOTAL").FontColor(Colors.White);
                                });

                                int i = 1;
                                foreach (var d in invoice.details)
                                {
                                    var subtotal = d.cantidad * d.mtoPrecioUnitario;

                                    table.Cell().Border(1).AlignCenter().Text(i++.ToString());
                                    table.Cell().Border(1).Text(d.descripcion ?? "");
                                    table.Cell().Border(1).AlignCenter().Text(d.cantidad.ToString("0.##"));
                                    table.Cell().Border(1).AlignRight().Text($"S/ {d.mtoPrecioUnitario:0.00}");
                                    table.Cell().Border(1).AlignRight().Text($"S/ {subtotal:0.00}");
                                }
                            });

                            // ===============================
                            // TOTALES
                            // ===============================
                            col.Item().PaddingTop(10).AlignRight().Text($"SUB TOTAL: S/ {invoice.subTotal:0.00}");
                            col.Item().AlignRight().Text($"IGV (18%): S/ {invoice.mtoIGV:0.00}");
                            col.Item().AlignRight().Text($"TOTAL A PAGAR: S/ {invoice.mtoImpVenta:0.00}")
                                .Bold();

                            // ===============================
                            // LEYENDAS
                            // ===============================
                            if (invoice.legends != null && invoice.legends.Any())
                            {
                                col.Item().PaddingTop(10);
                                foreach (var l in invoice.legends)
                                {
                                    col.Item().Text(l.value ?? "").Italic();
                                }
                            }
                        });
                    });
                }).GeneratePdf();
            }
            catch (Exception ex)
            {
                // LOG PRODUCCIÓN
                Console.WriteLine("ERROR AL GENERAR PDF:");
                Console.WriteLine(JsonSerializer.Serialize(invoice));
                Console.WriteLine(ex);

                return StatusCode(500, "Error al generar el PDF.");
            }

            // ===============================
            // DESCARGA
            // ===============================
            return File(pdf, "application/pdf", "factura.pdf");
        }
    }
}
