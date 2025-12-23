using LIBRERIA_APP.Datos;
using LIBRERIA_APP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace LIBRERIA_APP.Controllers
{
    [Authorize]
    public class LibreriaController : Controller
    {
        private readonly LibreriaDatos _libreriaDatos;

        public LibreriaController()
        {
            _libreriaDatos = new LibreriaDatos();
        }

        // ===================== VISTAS =====================
        public IActionResult GenerarVenta()
        {
            return View();
        }
        public IActionResult ConsultaVentasCotizacion()
        {
            return View();
        }

        public IActionResult GenerarCotizacion()
        {
            return View();
        }

        // ===================== HELPERS =====================
        /// <summary>
        /// Obtiene el IdUsuario desde los claims, y si no lo encuentra,
        /// usa 16 como usuario de prueba (para que no reviente la app).
        /// </summary>
        private int ObtenerIdUsuarioActual()
        {
            var claim =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("IdUsuario");

            return int.TryParse(claim, out var id) ? id : 0;
        }


        // ===================== REGISTRAR VENTA =====================
        [HttpPost]
        public IActionResult RegistrarVenta([FromBody] LibreriaModel venta)
        {
            try
            {
                if (venta == null || venta.Detalle == null || !venta.Detalle.Any())
                {
                    return Json(new
                    {
                        codMensaje = "0",
                        mensaje = "La venta no tiene detalle."
                    });
                }

                // Empresa fija por ahora
                venta.IdEmpresa = 1;

                // Tipo documento / moneda
                venta.TipoDocumento = "VEN";
                venta.TipoMoneda ??= "PEN";

                // Usuario actual (con fallback si el claim no existe)
                venta.IdUsuario = ObtenerIdUsuarioActual();
                if (venta.IdUsuario <= 0)
                {
                    return Json(new
                    {
                        codMensaje = "0",
                        mensaje = "No se pudo obtener el IdUsuario de la sesión. Vuelve a iniciar sesión."
                    });
                }

                var resultado = _libreriaDatos.RegistrarVenta(venta);

                return Json(new
                {
                    codMensaje = resultado.CodMensaje,
                    mensaje = resultado.Mensaje,
                    idCabecera = resultado.IdCabecera,
                    importeTotal = resultado.ImporteTotal
                });
            }
            catch (Exception ex)
            {
                // NUNCA devolvemos 400/401/500; siempre JSON con el error.
                return Json(new
                {
                    codMensaje = "0",
                    mensaje = "Error en el controlador (venta): " + ex.Message
                });
            }
        }

        // ===================== REGISTRAR COTIZACIÓN =====================
        [HttpPost]
        public IActionResult RegistrarCotizacion([FromBody] LibreriaModel cot)
        {
            try
            {
                if (cot == null || cot.Detalle == null || !cot.Detalle.Any())
                {
                    return Json(new
                    {
                        codMensaje = "0",
                        mensaje = "La cotización no tiene detalle."
                    });
                }

                cot.IdEmpresa = 1;
                cot.TipoDocumento = "COT";
                cot.TipoMoneda ??= "PEN";

                cot.IdUsuario = ObtenerIdUsuarioActual();
                if (cot.IdUsuario <= 0)
                {
                    return Json(new
                    {
                        codMensaje = "0",
                        mensaje = "No se pudo obtener el IdUsuario de la sesión. Vuelve a iniciar sesión."
                    });
                }

                var resultado = _libreriaDatos.RegistrarCotizacion(cot);

                return Json(new
                {
                    codMensaje = resultado.CodMensaje,
                    mensaje = resultado.Mensaje,
                    idCabecera = resultado.IdCabecera,
                    importeTotal = resultado.ImporteTotal
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codMensaje = "0",
                    mensaje = "Error en el controlador (cotización): " + ex.Message
                });
            }
        }

        // ===================== BUSCAR CLIENTE POR DOCUMENTO =====================
        [HttpGet]
        public IActionResult ClienteBuscarPorDocumento(string nroDocumento)
        {
            if (string.IsNullOrWhiteSpace(nroDocumento))
                return Json(new { encontrado = false });

            var cliente = _libreriaDatos.ClienteBuscarPorDocumento(nroDocumento);
            if (cliente == null)
                return Json(new { encontrado = false });

            return Json(new
            {
                encontrado = true,
                cliente
            });
        }

        // ===================== OBTENER COTIZACIÓN (CAB + DET) =====================
        [HttpGet]
        public IActionResult CotizacionObtener(int idCabecera)
        {
            var modelo = _libreriaDatos.ObtenerCotizacion(idCabecera);
            if (modelo == null)
                return Json(new { encontrado = false });

            return Json(new
            {
                encontrado = true,
                cabecera = new
                {
                    modelo.IdCabecera,
                    modelo.EntidadCliente,
                    modelo.IdEmpresa,
                    modelo.TipoMoneda,
                    modelo.TipoDocumento,
                    modelo.ImporteTotal,
                    modelo.FechaValidez,
                    modelo.IdTipoDocumentoCliente,
                    modelo.NroDocumentoCliente,
                    modelo.NombreCliente,
                    modelo.DireccionCliente,
                    modelo.EmailCliente,
                    modelo.TelefonoCliente
                },
                detalle = modelo.Detalle.Select(d => new
                {
                    d.Item,
                    d.SKU,
                    Descripcion = d.Descripcion,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.Descuento
                })
            });
        }
        [HttpGet]
        public JsonResult ListarVentasCot(string filtro)
        {
            // Llamamos al nuevo método que trae ventas y cotizaciones
            var datos = new LibreriaDatos().ConsultarVentCotLista(filtro);
            return Json(new { data = datos });
        }

        [HttpGet]
        public JsonResult ObtenerDetalle(int id)
        {
            // Usamos el método genérico para cualquier documento
            var datos = new LibreriaDatos().ObtenerDetalle(id);
            return Json(datos); // Devolvemos todo el objeto LibreriaModel con su lista Detalle
        }
    }
}
