using AlmacenWeb.Data;
using AlmacenWeb.Models;
using AlmacenWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using AlmacenWeb.ViewModels;
using System;
using AlmacenWeb.Services;

namespace AlmacenWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Encrypt _encrypt;
        private readonly IEmailSender _emailSender;


        public AccesoController(AppDbContext context, Encrypt encrypt, IEmailSender emailSender)
        {
            _context = context;
            _encrypt = encrypt;
            _emailSender = emailSender;
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
            return View("Login"); // Muestra la vista Login
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
        public async Task<IActionResult> Registrarse(RegistroViewModel model)
        {
           
            
            if (ModelState.IsValid)
            {
                // 1. Verificar si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.UsEmail == model.UsEmail))
                {
                    ModelState.AddModelError("UsEmail", "El email ya está registrado.");
                    return View(model);
                }

                // 2. Buscar el rol "Usuario"
                var rolUsuario = await _context.Roles.FirstOrDefaultAsync(r => r.RoNombre == "Usuario");
                if (rolUsuario == null)
                {
                    // Error crítico si el Seeder no corrió
                    ModelState.AddModelError(string.Empty, "Error interno: Rol de usuario no encontrado.");
                    return View(model);
                }

                // 3. "Mapear" el ViewModel a la Entidad
                var usuario = new Usuario
                {
                    UsNombre = model.UsNombre,
                    UsApellido = model.UsApellido,
                    UsEmail = model.UsEmail,
                    // ¡Hashear la contraseña del ViewModel!
                    UsPassword = _encrypt.HashPassword(model.Password),
                    RoId = rolUsuario.RoId, // Asignar rol por defecto
                    UsActivo = true,
                    UsFechaRegistro = DateTime.Now,
                    date_created = DateTime.Now
                };

                // 4. Guardar en la BD
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // 5. Redirigir a Login con mensaje de éxito
                TempData["RegistroExitoso"] = "¡Cuenta creada! Ya puedes iniciar sesión.";
                return RedirectToAction("Index"); 
            }

            
            return View(model);
        }


        // GET: /Acceso/Salir
        [HttpGet]
        public async Task<IActionResult> Salir()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acceso"); // Volver al Login
        }

        

        // GET: /Acceso/StartRecovery
        [HttpGet]
        public IActionResult StartRecovery()
        {
            // Muestra la vista con el formulario para pedir el email
            return View(new RecoveryViewModel());
        }

        // POST: /Acceso/StartRecovery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartRecovery(RecoveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                                    .FirstOrDefaultAsync(u => u.UsEmail == model.Email);

                if (usuario != null)
                {
                    // Generar un Token único
                    var token = Guid.NewGuid().ToString();

                    // Guardar el token y la fecha de expiración (1 hora)
                    // Usamos el campo date_created como "TokenExpiration"
                    usuario.Token = token;
                    usuario.date_created = DateTime.Now.AddHours(1);
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    // Construir el enlace de recuperación
                    var resetLink = Url.Action("RecoveryPassword", "Acceso",
                                        new { token = token }, Request.Scheme);

                    // Enviar el email
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Restablecer Contraseña - AlmacenWeb",
                        $"Has solicitado restablecer tu contraseña.<br>" +
                        $"Por favor, haz clic en el siguiente enlace para continuar:<br>" +
                        $"<a href='{resetLink}'>Restablecer Contraseña</a>" +
                        $"<br><br>Si no solicitaste esto, ignora este email." +
                        $"<br>El enlace expira en 1 hora.");
                }

                // Siempre mostramos el mismo mensaje,
                // incluso si el email no existe.
                return View("RecoveryConfirmation");
            }

            return View(model);
        }

        // Vista de confirmación
        [HttpGet]
        public IActionResult RecoveryConfirmation()
        {
            return View();
        }

        // GET: /Acceso/RecoveryPassword?token=...
        [HttpGet]
        public async Task<IActionResult> RecoveryPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return View("RecoveryError");
            }

            // Buscar el usuario por el token
            var usuario = await _context.Usuarios
                                .FirstOrDefaultAsync(u => u.Token == token);

            // Validar si el token existe Y si no ha expirado
            // (Usamos date_created como fecha de expiración)
            if (usuario == null || usuario.date_created == null || usuario.date_created < DateTime.Now)
            {
                // Token no válido o expirado
                return View("RecoveryError");
            }

            // Token válido, mostrar formulario de nueva contraseña
            var model = new RecoveryPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }

        // POST: /Acceso/RecoveryPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoveryPassword(RecoveryPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Volver a validar el token
                var usuario = await _context.Usuarios
                                    .FirstOrDefaultAsync(u => u.Token == model.Token);

                if (usuario == null || usuario.date_created == null || usuario.date_created < DateTime.Now)
                {
                    return View("RecoveryError");
                }

                // 1. Hashear la nueva contraseña
                usuario.UsPassword = _encrypt.HashPassword(model.NewPassword);

                // 2. Invalidar el token para que no se pueda reusar
                usuario.Token = null;
                usuario.date_created = null;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                // 3. Redirigir al Login con mensaje de éxito
                TempData["RegistroExitoso"] = "¡Contraseña actualizada! Ya puedes iniciar sesión.";
                return RedirectToAction("Index");
            }

            // Si las contraseñas no coinciden, volver a mostrar el formulario
            return View(model);
        }

       

    }
}