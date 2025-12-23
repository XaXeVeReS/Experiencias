using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_LIBRERIA.Datos;
using PROYECTO_LIBRERIA.Models;

namespace LIBRERIA_APP.Controllers
{
    public class ProveedoresController : Controller
    {
        ProveedorDatos _proveedorDatos = new ProveedorDatos();

        [Authorize]
        public IActionResult IndexProveedor()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProveedoresLista_Controller()
        {
            var lista = _proveedorDatos.ProveedorListaDatos();
            return Json(new { data = lista });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProveedoresListaSimple_Controller()
        {
            var lista = _proveedorDatos.ProveedorListaSimpleDatos();
            return Json(new { data = lista });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProveedoresObtener_Controller(int entidadId)
        {
            var prov = _proveedorDatos.ProveedorObtenerDatos(entidadId);
            return Json(prov);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProveedoresCrear_Controller([FromBody] ProveedorModel prov)
        {
            var result = _proveedorDatos.ProveedorCrearDatos(prov);
            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProveedoresActualizar_Controller([FromBody] ProveedorModel prov)
        {
            var result = _proveedorDatos.ProveedorActualizarDatos(prov);
            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProveedoresCambiarEstado_Controller(int entidadId, bool activo)
        {
            var result = _proveedorDatos.ProveedorCambiarEstadoDatos(entidadId, activo);
            return Json(result);
        }
    }
}
