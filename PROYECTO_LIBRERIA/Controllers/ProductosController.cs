using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_LIBRERIA.Datos;
using PROYECTO_LIBRERIA.Models;
using System.Security.Claims;


namespace LIBRERIA_APP.Controllers
{
    public class ProductosController : Controller
    {

        CategoriaDatos _categoriaDatos = new CategoriaDatos();
        ProductoDatos _productoDatos = new ProductoDatos();
        InventarioDatos _inventarioDatos = new InventarioDatos();

        [Authorize]
        public IActionResult IndexProducto()
        {
            return View();
        }
        [Authorize]
        public IActionResult StockAlmacen()
        {
            return View();
        }
        [Authorize]
        public IActionResult CategoriasProductos()
        {
            return View();
        }

        // ============= API PARA CATEGORÍAS =============

        #region Listar categorías para DataTable

        [Authorize]
        [HttpGet]
        public IActionResult CategoriasLista_Controller()
        {
            var lista = _categoriaDatos.CategoriaListaDatos();
            // DataTables espera un JSON con { data: [...] }
            return Json(new { data = lista });

        }
        #endregion

        #region Obtener una categoría por ID
        [Authorize]
        [HttpGet]
        public IActionResult CategoriasObtener_Controller(int id)
        {
            var cat = _categoriaDatos.CategoriaObtenerDatos(id);
            return Json(cat);
        }
        #endregion

        #region Crear nueva categoría
        [Authorize]
        [HttpPost]
        public IActionResult CategoriasCrear_Controller([FromBody] CategoriaModel cat)
        {
            var result = _categoriaDatos.CategoriaCrearDatos(cat);
            return Json(result);
        }
        #endregion

        #region Actualizar categoría existente
        [Authorize]
        [HttpPost]
        public IActionResult CategoriasActualizar_Controller([FromBody] CategoriaModel cat)
        {
            var result = _categoriaDatos.CategoriaActualizarDatos(cat);
            return Json(result);
        }
        #endregion


        // ========== API PRODUCTOS ==========

        #region Listar productos para DataTable
        [Authorize]
        [HttpGet]
        public IActionResult ProductosLista_Controller()
        {
            var lista = _productoDatos.ProductoListaDatos();
            return Json(new { data = lista });
        }
        #endregion

        #region Obtener un producto por ID
        [Authorize]
        [HttpGet]
        public IActionResult ProductosObtener_Controller(string sku)
        {
            var prod = _productoDatos.ProductoObtenerDatos(sku);
            return Json(prod);
        }
        #endregion

        #region Crear nuevo Producto
        [Authorize]
        [HttpPost]
        public IActionResult ProductosCrear_Controller([FromBody] ProductoModel prod)
        {
            var result = _productoDatos.ProductoCrearDatos(prod);
            return Json(result);
        }
        #endregion

        #region Actualizar producto existente
        [Authorize]
        [HttpPost]
        public IActionResult ProductosActualizar_Controller([FromBody] ProductoModel prod)
        {
            var result = _productoDatos.ProductoActualizarDatos(prod);
            return Json(result);
        }
        #endregion

       private int ObtenerIdUsuarioActual()
        {
            var claim =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("IdUsuario") ??
                User.FindFirstValue(ClaimTypes.Name);

            if (int.TryParse(claim, out var id))
                return id;

            // fallback (igual que LibreriaController)
            return 16;
        }

        [Authorize]
        [HttpPost]
        public IActionResult InventarioEntradaGrupo_Controller([FromBody] PROYECTO_LIBRERIA.Models.InventarioEntradaRequest req)
        {
            var idUsuario = ObtenerIdUsuarioActual();

            var origen = string.IsNullOrWhiteSpace(req?.origen) ? "AJUSTE" : req.origen!.Trim();
            var detalle = req?.detalle ?? new List<PROYECTO_LIBRERIA.Models.InventarioEntradaItemModel>();

            var resp = _inventarioDatos.InventarioEntradaGrupoDatos(idUsuario, origen, detalle);
            return Json(resp);
        }

        [Authorize]
        [HttpGet]
        public IActionResult StockDisponible_Controller(string sku)
        {
            var stock = _inventarioDatos.StockDisponibleDatos(sku);
            return Json(new { stock });
        }

        [Authorize]
        [HttpGet]
        public IActionResult KardexPorSku_Controller(string sku)
        {
            var idUsuario = ObtenerIdUsuarioActual();
            var lista = _inventarioDatos.KardexPorSkuDatos(idUsuario, sku);
            return Json(new { data = lista });
        }

        [Authorize]
        [HttpGet]
        public IActionResult CostoSugerido_Controller(string sku)
        {
            var costo = _inventarioDatos.UltimoCostoINDatos(sku);
            return Json(new { costoUnit = costo });
        }
    }
}
