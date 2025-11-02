using AlmacenWeb.Data;
using AlmacenWeb.Models;
using AlmacenWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlmacenWeb.Controllers
{
    [Authorize] // Todo el controlador está protegido
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        // Muestra el historial de ventas
        public async Task<IActionResult> Index()
        {
            var ventas = _context.Ventas
                                .Include(v => v.Cliente)
                                .Include(v => v.Usuario)
                                .OrderByDescending(v => v.VeFecha);

            return View(await ventas.ToListAsync());
        }

        // GET: Ventas/Details/5
        // Muestra una venta específica con sus detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.DetalleVentas) // Incluye los detalles
                    .ThenInclude(d => d.Producto) // E incluye el producto de cada detalle
                .FirstOrDefaultAsync(m => m.VeId == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/PuntoVenta
        // Muestra la interfaz del POS
        public IActionResult PuntoVenta()
        {
            // No necesitamos enviar nada, la vista carga todo con JS
            return View();
        }

        // --- [LÓGICA DE BÚSQUEDA DEL POS] ---
        // Estos métodos son llamados por JavaScript (fetch)

        // GET: /Ventas/BuscarClientes?term=...
        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var clientes = await _context.Clientes
                .Where(c => c.ClNombre.Contains(term) || c.ClApellido.Contains(term) || c.ClDniCuit.Contains(term))
                .Select(c => new {
                    id = c.ClId,
                    label = $"{c.ClNombre} {c.ClApellido} ({c.ClDniCuit})"
                })
                .ToListAsync();

            return Json(clientes);
        }

        // GET: /Ventas/BuscarProductos?term=...
        [HttpGet]
        public async Task<IActionResult> BuscarProductos(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var productos = await _context.Productos
                .Where(p => (p.PrNombre.Contains(term) || p.CodigoBarra.Contains(term)) && p.CantidadDisponible > 0)
                .Select(p => new {
                    id = p.PrId,
                    label = $"{p.PrNombre} (Stock: {p.CantidadDisponible})",
                    precio = p.Precio,
                    stock = p.CantidadDisponible
                })
                .ToListAsync();

            return Json(productos);
        }

        // --- [LÓGICA DE REGISTRO DE VENTA] ---

        // POST: /Ventas/RegistrarVenta
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaViewModel model)
        {
            if (model == null || model.Detalles == null || !model.Detalles.Any())
            {
                return BadRequest(new { success = false, message = "Datos de venta inválidos." });
            }

            // Usamos una transacción: si falla el descuento de stock,
            // no se registra la venta (y viceversa).
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Buscar al usuario que hace la venta (desde la sesión)
                    var userEmail = User.FindFirst("Email")?.Value;
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsEmail == userEmail);

                    if (usuario == null)
                    {
                        return Unauthorized(new { success = false, message = "Usuario no autenticado." });
                    }

                    // 2. Crear el encabezado de la Venta
                    var venta = new Venta
                    {
                        ClId = model.ClId,
                        UsId = usuario.UsId,
                        VeFecha = DateTime.Now,
                        VeTotal = model.Total,
                        DetalleVentas = new List<DetalleVenta>()
                    };
                    _context.Ventas.Add(venta);

                    // 3. Crear los detalles y descontar stock
                    foreach (var detalleVM in model.Detalles)
                    {
                        var producto = await _context.Productos.FindAsync(detalleVM.PrId);
                        if (producto == null || producto.CantidadDisponible < detalleVM.Cantidad)
                        {
                            // Si no hay stock, revertir todo
                            await transaction.RollbackAsync();
                            return BadRequest(new { success = false, message = $"Stock insuficiente para '{producto?.PrNombre ?? "N/A"}'." });
                        }

                        // Descontar stock
                        producto.CantidadDisponible -= detalleVM.Cantidad;
                        _context.Productos.Update(producto);

                        // Crear el detalle
                        var detalleVenta = new DetalleVenta
                        {
                            Venta = venta, // Asocia al encabezado
                            PrId = producto.PrId,
                            DeCantidad = detalleVM.Cantidad,
                            DePrecioUnitario = detalleVM.PrecioUnitario,
                            DeSubtotal = detalleVM.Subtotal
                        };

                        // Agregamos el detalle al contexto (EF Core se encargará de la FK 'VeId'
                        // cuando 'venta' sea guardada)
                        _context.DetalleVenta.Add(detalleVenta);
                    }

                    // 4. Guardar todos los cambios
                    await _context.SaveChangesAsync();

                    // 5. Confirmar la transacción
                    await transaction.CommitAsync();

                    // Devolvemos el ID de la venta creada
                    return Ok(new { success = true, message = "Venta registrada exitosamente.", ventaId = venta.VeId });
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir todo
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { success = false, message = $"Error interno del servidor: {ex.Message}" });
                }
            }
        }


        // --- (Métodos de CRUD que no usaremos en el POS, pero sí en el Index) ---
        // (El proyecto original no tenía Delete/Edit para Ventas, solo Index/Details)
    }
}
