using LIBRERIA_APP.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROYECTO_LIBRERIA.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("invoice/send", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest(responseBody);

            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<InvoiceApiResponse>(responseBody);

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

        //  private readonly HttpClient _httpClient;
        /// <summary>
        ///  private const string BaseUrl = "https://dniruc.apisperu.com/api/v1/ruc/";
        /// </summary>
        ///  private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImZzb25jb21vbnRlc0BnbWFpbC5jb20ifQ.o74PkSOUc0f1cmfVXyww7l1djUbxv7NzKz3YxV4TFkg";


        // CONSULTAR RUC

        [HttpGet("consultar-ruc/{ruc}")]
        public async Task<IActionResult> ConsultarRuc(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Debe ingresar un RUC válido."
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var baseUrl = "https://dniruc.apisperu.com/api/v1/ruc/";
                var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6ImZzb25jb21vbnRlc0BnbWFpbC5jb20ifQ.o74PkSOUc0f1cmfVXyww7l1djUbxv7NzKz3YxV4TFkg";

                var response = await client.GetAsync($"{baseUrl}{ruc}?token={token}");

                if (!response.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "RUC no encontrado."
                    });
                }

                var json = await response.Content.ReadAsStringAsync();

                // DESERIALIZACIÓN FUERTE
                var apiModel = JsonConvert.DeserializeObject<ApiModel>(json);

                if (apiModel == null || string.IsNullOrEmpty(apiModel.razonSocial))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No se encontraron datos del RUC."
                    });
                }

                return Ok(new
                {
                    success = true,
                    razonSocial = apiModel.razonSocial,
                    direccion = apiModel.direccion,
                    departamento = apiModel.departamento,
                    provincia = apiModel.provincia,
                    distrito = apiModel.distrito,
                    ubigeo = apiModel.ubigeo,
                    estado = apiModel.estado,
                    condicion = apiModel.condicion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al consultar el RUC",
                    error = ex.Message
                });
            }
        }

    }
}
