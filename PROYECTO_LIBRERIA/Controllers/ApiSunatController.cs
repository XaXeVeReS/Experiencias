using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LIBRERIA_APP.Models;

namespace PROYECTO_LIBRERIA.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ApiSunatController : ControllerBase
    {



        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ApiSunatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("invoice/enviar-sunat")]
        public async Task<IActionResult> EnviarFacturaSunat([FromBody] InvoiceRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApisPeru:BaseUrl"];
            var token = _configuration["ApisPeru:Token"];

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("invoice/send", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest(responseBody);

            var apiResponse = JsonSerializer.Deserialize<InvoiceApiResponse>(responseBody);

            // Convertir Base64 a bytes
            var xmlBytes = Convert.FromBase64String(apiResponse.xml);
            var pdfBytes = Convert.FromBase64String(apiResponse.pdf);

            // Guardar archivos (opcional)
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/facturas");
            Directory.CreateDirectory(path);

            var nombreBase = $"{request.serie}-{request.correlativo}";
            var xmlPath = Path.Combine(path, $"{nombreBase}.xml");
            var pdfPath = Path.Combine(path, $"{nombreBase}.pdf");

            System.IO.File.WriteAllBytes(xmlPath, xmlBytes);
            System.IO.File.WriteAllBytes(pdfPath, pdfBytes);

            return Ok(new
            {
                mensaje = apiResponse.sunatResponse.cdrResponse.description,
                xmlUrl = $"/facturas/{nombreBase}.xml",
                pdfUrl = $"/facturas/{nombreBase}.pdf",
                sunat = apiResponse.sunatResponse
            });
        }





        [HttpPost("enviar-sunat")]
        public async Task<IActionResult> EnviarFacturaSunat_primero([FromBody] InvoiceRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            var baseUrl = _configuration["ApisPeru:BaseUrl"];
            var token = _configuration["ApisPeru:Token"];

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("invoice/send", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new
                {
                    mensaje = "Error al enviar factura a SUNAT",
                    detalle = responseBody
                });
            }

            return Ok(new
            {
                mensaje = "Factura enviada correctamente",
                resultado = responseBody
            });
        }

    }
}
