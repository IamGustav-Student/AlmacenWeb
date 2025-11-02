using AlmacenWeb.Data;
using AlmacenWeb.Models;
using AlmacenWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlmacenWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Encrypt _encrypt;

        public AccesoController(AppDbContext context, Encrypt encrypt)
        {
            _context = context;
            _encrypt = encrypt;
        }

        // GET: /Acceso/ o /Acceso/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Si el usuario ya está logueado, redirigir al Home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Login"); // Muestra la vista Login.cshtml
        }

        // POST: /Acceso/Index
        [HttpPost]
        public async Task<IActionResult> Index(Login model)
        {
            if (ModelState.IsValid)
            {
                // Buscar al usuario por email
                var usuario = await _context.Usuarios
                                    .Include(u => u.Rol) // Incluir el Rol
                                    .FirstOrDefaultAsync(u => u.UsEmail == model.Email);

                if (usuario == null)
                {
                    ViewData["Mensaje"] = "Usuario no encontrado";
                    return View("Login");
                }

                // Verificar la contraseña
                if (!usuario.UsActivo)
                {
                    ViewData["Mensaje"] = "Tu cuenta está desactivada";
                    return View("Login");
                }

                // Usamos el servicio Encrypt para verificar
                if (!_encrypt.VerifyPassword(model.Password, usuario.UsPassword))
                {
                    ViewData["Mensaje"] = "Contraseña incorrecta";
                    return View("Login");
                }

                // --- Si la validación es exitosa, creamos la sesión (Cookie) ---

                // 1. Crear 'Claims' (Datos del usuario para la sesión)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.UsNombre),
                    new Claim("Email", usuario.UsEmail),
                    new Claim(ClaimTypes.Role, usuario.Rol.RoNombre), // ¡Guardamos el Rol!
                };

                // 2. Crear identidad
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Propiedades de autenticación
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // IsPersistent = true (para "Recordarme")
                };

                // 4. Iniciar Sesión (crear la cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            return View("Login", model);
        }

        // GET: /Acceso/Registrarse
        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        // POST: /Acceso/Registrarse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(Usuario usuario)
        {
            // --- ¡CORRECCIÓN! ---
            // Removemos 'RoId' y 'Rol' de la validación del modelo.
            // El modelo 'Usuario' los marca como [Required], pero en el
            // registro público (self-service), el Rol se asigna automáticamente.
            ModelState.Remove("RoId");
            ModelState.Remove("Rol");
            // ---------------------

            // Ahora sí, validamos el resto del modelo (Nombre, Email, Pass, etc.)
            if (ModelState.IsValid)
            {
                // 1. Verificar si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.UsEmail == usuario.UsEmail))
                {
                    ModelState.AddModelError("UsEmail", "El email ya está registrado.");
                    return View(usuario);
                }

                // 2. Asignar rol por defecto "Usuario"
                var rolUsuario = await _context.Roles.FirstOrDefaultAsync(r => r.RoNombre == "Usuario");

                if (rolUsuario == null)
                {
                    // Esto sería un error crítico si el DbInitialize no corrió
                    ModelState.AddModelError(string.Empty, "Error interno al asignar rol. Contacte al administrador.");
                    // O podríamos usar el ID 4 que sabemos que es, pero buscarlo es más robusto
                    // usuario.RoId = 4;
                    return View(usuario);
                }

                usuario.RoId = rolUsuario.RoId; // Asignamos el ID del rol "Usuario"

                // 3. Hashear la contraseña
                usuario.UsPassword = _encrypt.HashPassword(usuario.UsPassword);
                usuario.UsActivo = true;
                usuario.UsFechaRegistro = DateTime.Now;
                usuario.date_created = DateTime.Now; // Aseguramos que este campo también se llene

                // 4. Guardar en la BD
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // 5. Redirigir a Login con mensaje de éxito
                TempData["RegistroExitoso"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
                return RedirectToAction("Index"); // Redirigir a Login
            }

            // Si el modelo (ej: email inválido, pass vacía) no es válido, volver
            return View(usuario);
        }


        // GET: /Acceso/Salir
        [HttpGet]
        public async Task<IActionResult> Salir()
        {
            // Cerrar sesión (borra la cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acceso"); // Volver al Login
        }
    }
}