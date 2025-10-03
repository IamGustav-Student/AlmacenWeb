using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlmacenWeb.Data;
using AlmacenWeb.Models;
using AlmacenWeb.ViewModels;



namespace AlmacenWeb.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }
        // Acción para buscar producto por código de barras (usando AJAX)
        [HttpPost]
        public async Task<IActionResult> BuscarProducto([FromBody] string codigoBarra)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoBarra == codigoBarra);
            if (producto == null)
            {
                return NotFound();
            }
            return Json(producto);
        }
        [HttpPost]
        public async Task<IActionResult> BuscarProductoPorNombre([FromBody] string nombre)
        {
            // Documentación: Busca productos por nombre de forma insensible a mayúsculas y minúsculas.
            var productos = await _context.Productos
                                          .Where(p => p.PrNombre.ToLower().Contains(nombre.ToLower()))
                                          .ToListAsync();
            return Json(productos);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorId(int PrId)
        {
            var producto = await _context.Productos.FindAsync(PrId);
            if (producto == null)
            {
                return NotFound();
            }
            return Json(producto);
        }

        // GET: Ventas/PuntoVenta
        public IActionResult PuntoVenta()
        {
            return View();
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ventas.Include(v => v.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClId", "ClApellido");
            return View();
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaVenta,Total,ClienteId")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClId", "ClApellido", venta.ClienteId);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClId", "ClApellido", venta.ClienteId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaVenta,Total,ClienteId")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClId", "ClApellido", venta.ClienteId);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> FinalizarVenta([FromBody] List<Producto> carrito)
        {
            // Documentación: Lógica para finalizar la venta y descontar el inventario.
            if (carrito == null || !carrito.Any())
            {
                return BadRequest("El carrito está vacío.");
            }

            // Crear una nueva venta
            var nuevaVenta = new Venta
            {
                FechaVenta = DateTime.Now,
                Total = carrito.Sum(p => p.Precio), // Sumar los precios de los productos
                DetalleVenta = new List<DetalleVenta>()
            };

            // Procesar cada producto en el carrito
            foreach (var productoEnCarrito in carrito)
            {
                var productoDb = await _context.Productos.FindAsync(productoEnCarrito.PrId);
                if (productoDb != null)
                {
                    // Verificar si hay suficiente stock
                    if (productoDb.CantidadDisponible > 0)
                    {
                        // Descontar el inventario
                        productoDb.CantidadDisponible -= 1; // Asumimos que la cantidad es 1 por cada escaneo

                        // Agregar el detalle de la venta
                        nuevaVenta.DetalleVenta.Add(new DetalleVenta
                        {
                            ProductoId = productoDb.PrId,
                            Cantidad = 1,
                            PrecioUnitario = productoDb.Precio
                        });
                    }
                    else
                    {
                        // Manejar el caso de stock insuficiente (opcional, pero buena práctica)
                        return BadRequest($"Stock insuficiente para el producto: {productoDb.PrNombre}");
                    }
                }
            }

            // Guardar la venta y actualizar los productos en la base de datos
            _context.Ventas.Add(nuevaVenta);
            await _context.SaveChangesAsync();

            return Ok("Venta finalizada con éxito.");
        }
    }
}
