using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_LIBRERIA.Datos;

namespace LIBRERIA_APP.Controllers
{
    public class TipoDocumentoController : Controller
    {
        TipoDocumentoDatos _datos = new TipoDocumentoDatos();

        [Authorize]
        [HttpGet]
        public IActionResult TipoDocumentoLista_Controller()
        {
            var lista = _datos.TipoDocumentoListaDatos();
            return Json(new { data = lista });
        }
    }
}
