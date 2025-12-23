using LIBRERIA_APP.Datos;
using LIBRERIA_APP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_LIBRERIA.Models;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;


namespace LIBRERIA_APP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        UsuarioDatos _Login = new UsuarioDatos();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult IndexInformativo()
        {
            ViewBag.NombreCompleto = User.Identity.Name; 
            ViewBag.Rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            ViewBag.UsuarioLogin = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return View();
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult GestionUsuario()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> IniciarSesion(string usuario, string contrasenia)
        {
            try
            {
                var respuesta = _Login.IniciarSesion(usuario, contrasenia);

                if (respuesta.codMensaje == "1")
                {
                    // Guardamos info clave en claims (claims = datos dentro de la cookie)
                    var claims = new List<Claim>
            {
                // Id del usuario (esto es lo que luego usarás para ventas/cotizaciones)
                new Claim(ClaimTypes.NameIdentifier, respuesta.idUsuario.ToString()),

                // Nombre visible
                new Claim(ClaimTypes.Name, respuesta.nombreRazon ?? ""),

                // Rol (en tu BD viene como Codigo del rol)
                new Claim(ClaimTypes.Role, respuesta.rolDescripcion ?? ""),

                // Opcional: por compatibilidad si en algún lado buscas "IdUsuario"
                new Claim("IdUsuario", respuesta.idUsuario.ToString())
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            // Puedes ajustar esto si quieres una sesión más corta para demo:
                            // ExpiresUtc = DateTimeOffset.UtcNow.AddHours(5)
                        }
                    );
                }

                return Json(respuesta);
            }
            catch
            {
                return Json(new { respuesta = false });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult UsuariosListaGeneral_Controller()
        {
            var datos = _Login.UsuariosListaGeneral_Datos();
            return Json(new { data = datos });
        }


        [Authorize]
        [HttpGet]
        public IActionResult TipoDocumentoLista_Controller()
        {
            var datos = _Login.TipoDocumentoListaDatos();
            return Json(datos);
        }

        [Authorize]
        [HttpGet]
        public IActionResult RolLista_Controller()
        {
            var datos = _Login.RolListaDatos();
            return Json(datos);
        }



        [Authorize]
        [HttpGet]
        public IActionResult UsuariosIdndicadores_Controller()
        {
            var datos = _Login.UsuariosIndicadoresDatos();
            return Json(datos);
        }


        [Authorize]
        [HttpPost]
        public IActionResult UsuariosCrear_Controller([FromBody] UsuariosModel use)
        {
            var datos = _Login.UsuarioCrearDatos(use);
            return Json(datos);
        }


        [Authorize]
        [HttpGet]
        public IActionResult UsuariosObtener_Controller(string id)
        {
            var datos = _Login.uUsuarioObtenerDatos(id);
            return Json(datos);
        }


        [Authorize]
        [HttpPost]
        public IActionResult UsuariosActualizar_Controller([FromBody] UsuariosModel use)
        {
            var datos = _Login.UsuarioActualizarDatos(use);
            return Json(datos);
        }


        public async Task<IActionResult> CerrarSesion()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           // Conexion cn = new Conexion();
            //LoginModel loginModel = new LoginModel();
            //using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            //{
            //    conexion.Open();
            //    var cmd = new SqlCommand("usp_CerrarSesion", conexion);
            //    cmd.Parameters.Clear();
            //    cmd.Parameters.AddWithValue("USuario", xValor);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.ExecuteReader();
            //}

            return RedirectToAction("Index", "Home");
        }

    }
}
