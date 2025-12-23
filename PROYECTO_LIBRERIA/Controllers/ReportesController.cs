using LIBRERIA_APP.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using PROYECTO_LIBRERIA.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PROYECTO_LIBRERIA.Datos;

namespace LIBRERIA_APP.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ReporteDatos _repo;

        public ReportesController(ReporteDatos repo)
        {
            _repo = repo;
            // Corregido: Usar el LicenseType correcto de QuestPDF, no de NuGet.Packaging
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }



        [HttpGet]
        public IActionResult Index()
        {
            var filtro = new ReporteFiltro
            {
                FechaInicio = DateTime.Today.AddDays(-30),
                FechaFin = DateTime.Today,
                Modo = "VENTAS_VENDEDOR"
            };
            return View(filtro);
        }

        [HttpPost]
        public async Task<IActionResult> Generar(ReporteFiltro filtro)
        {
            var data = await _repo.EjecutarReporteAsync(filtro);
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(ReporteFiltro filtro)
        {
            var data = await _repo.EjecutarReporteAsync(filtro);
            if (data.Count == 0)
                return BadRequest("No hay datos para exportar.");

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Reporte");

            // headers
            var headers = data[0].Keys.ToList();
            for (int i = 0; i < headers.Count; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            // rows
            for (int r = 0; r < data.Count; r++)
            {
                var row = data[r];
                int c = 1;
                foreach (var h in headers)
                {
                    ws.Cell(r + 2, c).Value = row[h]?.ToString() ?? "";
                    c++;
                }
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;
            string fileName = $"Reporte_{filtro.Modo}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportPDF(ReporteFiltro filtro)
        {
            var data = await _repo.EjecutarReporteAsync(filtro);

            if (data.Count == 0)
                return BadRequest("No hay datos para exportar.");

            var headers = data[0].Keys.ToList();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Element(header =>
                    {
                        header
                            .PaddingBottom(10)
                            .AlignCenter()
                            .Text($"Reporte - {filtro.Modo}")
                            .FontSize(18)
                            .Bold();
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in headers)
                                columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (var h in headers)
                                header.Cell().Element(CellStyle).Text(h);
                        });

                        foreach (var row in data)
                        {
                            foreach (var h in headers)
                            {
                                table.Cell()
                                    .Element(CellStyle)
                                    .Text(row[h]?.ToString() ?? "");
                            }
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Padding(5)
                                .BorderBottom(1)
                                .BorderColor("#CCCCCC");
                        }
                    });
                });
            });

            byte[] pdf = doc.GeneratePdf();

            string fileName = $"Reporte_{filtro.Modo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            return File(pdf, "application/pdf", fileName);
        }


        // Autocomplete
        [HttpGet]
        public async Task<IActionResult> BuscarUsuarios(string filtro)
        {
            var data = await _repo.BuscarUsuariosAsync(filtro ?? "");
            return Json(data.Select(x => new { id = x.Id, text = x.Nombre }));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string filtro)
        {
            var data = await _repo.BuscarClientesAsync(filtro ?? "");
            return Json(data.Select(x => new { id = x.Id, text = x.Nombre }));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCategorias(string filtro)
        {
            var data = await _repo.BuscarCategoriasAsync(filtro ?? "");
            return Json(data.Select(x => new { id = x.Id, text = x.Nombre }));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string filtro, int? idCategoria)
        {
            var data = await _repo.BuscarProductosAsync(filtro ?? "", idCategoria);
            return Json(data.Select(x => new { id = x.Id, text = x.Nombre }));
        }
    }
}
