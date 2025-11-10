using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlmacenWeb.Data;
using AlmacenWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace AlmacenWeb.Controllers
{
    [Authorize(Roles = "Admin, Empleador, Empleado")]
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.PrId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        // o
        // GET: Productos/Create?codigoBarra=12345
        public IActionResult Create(string codigoBarra)
        {
            var model = new Producto();

            if (!string.IsNullOrEmpty(codigoBarra))
            {
                model.CodigoBarra = codigoBarra;
            }

            return View(model);
        }
        // GET: /Productos/CargarStock
        [HttpGet]
        public IActionResult CargarStock()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsCodigoBarraAvailable(string CodigoBarra, int PrId)
        {
            

            bool exists = await _context.Productos.AnyAsync(
                p => p.CodigoBarra == CodigoBarra && p.PrId != PrId
            );

            if (exists)
            {
                
                return Json($"El código de barras '{CodigoBarra}' ya está en uso.");
            }

            
            return Json(true);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrId,PrNombre,CodigoBarra,Precio,CantidadDisponible")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
        
        // GET: /Productos/BuscarProductoPorCodigo?codigo=...
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                return BadRequest(new { message = "El código no puede ser nulo." });
            }

            var producto = await _context.Productos
                                 .FirstOrDefaultAsync(p => p.CodigoBarra == codigo);

            if (producto != null)
            {
               
                // Devolvemos la URL para EDITAR el producto existente
                return Json(new
                {
                    message = "Producto encontrado. Redirigiendo a Edición...",
                    redirectTo = Url.Action("Edit", "Productos", new { id = producto.PrId })
                });
            }
            else
            {
                
                // Devolvemos la URL para CREAR un nuevo producto,
                // pasando el código como parámetro.
                return Json(new
                {
                    message = "Producto nuevo. Redirigiendo a Creación...",
                    redirectTo = Url.Action("Create", "Productos", new { codigoBarra = codigo })
                });
            }
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrId,PrNombre,CodigoBarra,Precio,CantidadDisponible")] Producto producto)
        {
            if (id != producto.PrId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.PrId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.PrId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.PrId == id);
        }
    }
}
