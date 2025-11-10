using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlmacenWeb.Data;
using AlmacenWeb.Models;
using Microsoft.AspNetCore.Authorization;
using AlmacenWeb.Services;
using AlmacenWeb.ViewModels;

namespace AlmacenWeb.Controllers
{
    
    [Authorize(Roles = "Admin, Dueño")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Encrypt _encrypt;

        public UsuariosController(AppDbContext context, Encrypt encrypt)
        {
            _context = context;
            _encrypt = encrypt;
        }

        // GET: Usuarios 
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Usuarios.Include(u => u.Rol);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Usuarios/Details/5 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsId == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        
        // GET: Usuarios/Create
        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        public IActionResult Create()
        {
            // Enviamos la lista de Roles a la vista
            ViewData["RoId"] = new SelectList(_context.Roles, "RoId", "RoNombre");
            
            return View(new UsuarioCreateViewModel());
        }

        
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UsuarioCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (await _context.Usuarios.AnyAsync(u => u.UsEmail == model.UsEmail))
                {
                    ModelState.AddModelError("UsEmail", "El email ya está registrado.");
                }
                
                else if (await _context.Usuarios.AnyAsync(u => u.UsNombre == model.UsNombre && u.UsApellido == model.UsApellido))
                {
                    ModelState.AddModelError("UsNombre", "Ya existe un usuario con ese nombre y apellido.");
                }
                else
                {
                    
                    var usuario = new Usuario
                    {
                        UsNombre = model.UsNombre,
                        UsApellido = model.UsApellido,
                        UsEmail = model.UsEmail,
                        UsPassword = _encrypt.HashPassword(model.Password),
                        UsActivo = model.UsActivo,
                        RoId = model.RoId,
                        UsFechaRegistro = DateTime.Now,
                        date_created = DateTime.Now
                    };

                    
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["RoId"] = new SelectList(_context.Roles, "RoId", "RoNombre", model.RoId);
            return View(model);
        }

        
        // GET: Usuarios/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Mapear la Entidad -> ViewModel
            var model = new UsuarioEditViewModel
            {
                UsId = usuario.UsId,
                UsNombre = usuario.UsNombre,
                UsApellido = usuario.UsApellido,
                UsEmail = usuario.UsEmail,
                UsActivo = usuario.UsActivo,
                RoId = usuario.RoId,
                Password = "" 
            };

            //Enviar Roles y el ViewModel
            ViewData["RoId"] = new SelectList(_context.Roles, "RoId", "RoNombre", model.RoId);
            return View(model);
        }

        
        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, UsuarioEditViewModel model)
        {
            if (id != model.UsId) return NotFound();

            
            if (ModelState.IsValid)
            {
                try
                {
                    //  Buscar el usuario existente en la BD
                    var usuarioEnDb = await _context.Usuarios.FindAsync(model.UsId);
                    if (usuarioEnDb == null) return NotFound();

                    //  Verificar si el Email cambió y si ya existe
                    if (usuarioEnDb.UsEmail != model.UsEmail &&
                        await _context.Usuarios.AnyAsync(u => u.UsEmail == model.UsEmail && u.UsId != model.UsId))
                    {
                        ModelState.AddModelError("UsEmail", "Ese email ya está en uso por otra cuenta.");
                    }
                    else
                    {
                        //  Mapear campos
                        usuarioEnDb.UsNombre = model.UsNombre;
                        usuarioEnDb.UsApellido = model.UsApellido;
                        usuarioEnDb.UsEmail = model.UsEmail;
                        usuarioEnDb.UsActivo = model.UsActivo;
                        usuarioEnDb.RoId = model.RoId;

                        //  Lógica de Contraseña Opcional
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            // Solo se actualiza si se escribió algo
                            usuarioEnDb.UsPassword = _encrypt.HashPassword(model.Password);
                        }

                        // Guardar
                        _context.Update(usuarioEnDb);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(model.UsId)) return NotFound();
                    else throw;
                }
            }

            // Si falla, recargar roles y volver
            ViewData["RoId"] = new SelectList(_context.Roles, "RoId", "RoNombre", model.RoId);
            return View(model);
        }

        // GET: Usuarios/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsId == id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuarios/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            if (id == 1)
            {
                TempData["Error"] = "No se puede borrar al usuario Administrador principal.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return RedirectToAction(nameof(Index));

            // Lógica de seguridad: Verificar si tiene ventas
            var tieneVentas = await _context.Ventas.AnyAsync(v => v.UsId == id);
            if (tieneVentas)
            {
                TempData["Error"] = "No se puede borrar el usuario porque tiene ventas asociadas. Desactívelo en su lugar (Editar -> Quitar check 'Usuario Activo').";
                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsId == id);
        }
    }
}