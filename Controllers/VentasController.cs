using AlmacenWeb.Data;
using AlmacenWeb.Models;
using AlmacenWeb.Services; 
using AlmacenWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlmacenWeb.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISession _session;

        public VentasController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _session = httpContextAccessor.HttpContext.Session;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = _context.Ventas
                                .Include(v => v.Cliente)
                                .Include(v => v.Usuario)
                                .OrderByDescending(v => v.VeFecha);

            return View(await ventas.ToListAsync());
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

        #region --- (Punto de Venta) ---
        // GET: Ventas/PuntoVenta
        [HttpGet]
        public async Task<IActionResult> PuntoVenta()
        {
            // Cargar el carrito desde la sesión
            var carrito = SessionHelper.Get<List<CarritoItemViewModel>>(_session, "Carrito") ?? new List<CarritoItemViewModel>();

            // Cargar productos (solo con stock) para el dropdown
            var productosDb = await _context.Productos
                                    .Where(p => p.CantidadDisponible > 0)
                                    .ToListAsync();

            // Cargar clientes para el dropdown
            var clientesDb = await _context.Clientes.ToListAsync();

            // Crear el ViewModel
            var viewModel = new PosViewModel
            {
                Carrito = carrito,
                TotalCarrito = carrito.Sum(item => item.Subtotal),

                
                ProductosDisponibles = new SelectList(productosDb, "PrId", "PrNombre"),

                ClientesDisponibles = new SelectList(clientesDb.Select(c => new {
                    ClId = c.ClId,
                    NombreCompleto = $"{c.ClNombre} {c.ClApellido ?? ""} ({c.ClDniCuit})"
                }), "ClId", "NombreCompleto")
            };

            
            if (TempData["ErrorVenta"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["ErrorVenta"].ToString());
            }

            return View(viewModel);
        }
        #endregion


        #region --- Lógica para Agregar al Carrito ---
        // POST: Ventas/AgregarAlCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAlCarrito(PosViewModel model)
        {
            // 1. Recuperar el carrito
            var carrito = SessionHelper.Get<List<CarritoItemViewModel>>(_session, "Carrito") ?? new List<CarritoItemViewModel>();

            // 2. Buscar el producto en la BD
            var productoDb = await _context.Productos.FindAsync(model.ProductoSeleccionadoId);

            if (productoDb == null || productoDb.CantidadDisponible < model.Cantidad)
            {
                TempData["ErrorVenta"] = "Stock insuficiente o producto no encontrado.";
                return RedirectToAction("PuntoVenta");
            }

            // Buscar si el producto ya está en el carrito
            var itemEnCarrito = carrito.FirstOrDefault(item => item.ProductoId == productoDb.PrId);

            if (itemEnCarrito != null)
            {
                
                if (itemEnCarrito.Cantidad + model.Cantidad > productoDb.CantidadDisponible)
                {
                    TempData["ErrorVenta"] = $"No hay stock suficiente para agregar {model.Cantidad} más. Stock disponible: {productoDb.CantidadDisponible}.";
                }
                else
                {
                    itemEnCarrito.Cantidad += model.Cantidad;
                }
            }
            else
            {
                
                carrito.Add(new CarritoItemViewModel
                {
                    ProductoId = productoDb.PrId,
                    Nombre = productoDb.PrNombre,
                    Cantidad = model.Cantidad,
                    Precio = productoDb.Precio
                });
            }

           
            SessionHelper.Set(_session, "Carrito", carrito);

            return RedirectToAction("PuntoVenta");
        }
        #endregion

        #region --- Lógica para Quitar del Carrito y Cancelar Venta ---
        // POST: Ventas/QuitarDelCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuitarDelCarrito(int productoId)
        {
            var carrito = SessionHelper.Get<List<CarritoItemViewModel>>(_session, "Carrito");

            if (carrito != null)
            {
                var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);
                if (item != null)
                {
                    carrito.Remove(item);
                    SessionHelper.Set(_session, "Carrito", carrito);
                }
            }
            return RedirectToAction("PuntoVenta");
        }
        #endregion

        #region --- Lógica para Cancelar Venta ---

        // POST: Ventas/CancelarVenta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarVenta()
        {
            // Se borra el carrito de la sesión
            _session.Remove("Carrito");
            return RedirectToAction("PuntoVenta");
        }
        #endregion


        #region --- Lógica para Buscar Producto por Código de Barras ---
        // GET: /Ventas/GetProductoPorCodigo?codigo=...
        [HttpGet]
        public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                return BadRequest("El código no puede ser nulo.");
            }

            
            var producto = await _context.Productos
                .Where(p => p.CodigoBarra == codigo && p.CantidadDisponible > 0)
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                // Si no se encuentra mostramos mensaje de error
                return NotFound(new { message = "Producto no encontrado o sin stock." });
            }

            // Si se encuentra, devolvemos el objeto con los datos necesarios
            var productoViewModel = new
            {
                id = producto.PrId,
                label = $"{producto.PrNombre} (Stock: {producto.CantidadDisponible})",
                precio = producto.Precio,
                stock = producto.CantidadDisponible
            };

            return Json(productoViewModel);
        }
        #endregion


        #region --- Lógica para Registrar la Venta ---
        // POST: Ventas/RegistrarVenta
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  Recibimos el 'ClienteSeleccionadoId' y el 'MetodoPagoSeleccionado'
        public async Task<IActionResult> RegistrarVenta(int ClienteSeleccionadoId, MetodoPago MetodoPagoSeleccionado)
        {
            var carrito = SessionHelper.Get<List<CarritoItemViewModel>>(_session, "Carrito");

            if (carrito == null || !carrito.Any())
            {
                TempData["ErrorVenta"] = "El carrito está vacío.";
                return RedirectToAction("PuntoVenta");
            }

            if (ClienteSeleccionadoId == 0)
            {
                TempData["ErrorVenta"] = "Debe seleccionar un cliente.";
                return RedirectToAction("PuntoVenta");
            }

            
            bool pagado = true;
            if (MetodoPagoSeleccionado == MetodoPago.Fiado)
            {
                
                if (ClienteSeleccionadoId == 1) // 1 es "Consumidor Final"
                {
                    TempData["ErrorVenta"] = "No se puede registrar 'Fiado' al Consumidor Final. Seleccione otro cliente.";
                    return RedirectToAction("PuntoVenta");
                }
                pagado = false; 
            }
            

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userEmail = User.FindFirst("Email")?.Value;
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsEmail == userEmail);
                    if (usuario == null) throw new Exception("Usuario no encontrado.");

                    var venta = new Venta
                    {
                        VeFecha = DateTime.Now,
                        ClId = ClienteSeleccionadoId,
                        UsId = usuario.UsId,
                        VeTotal = carrito.Sum(item => item.Subtotal),
                        MetodoPago = MetodoPagoSeleccionado, 
                        Pagado = pagado                     
                    };
                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    foreach (var item in carrito)
                    {
                        var productoDb = await _context.Productos.FindAsync(item.ProductoId);
                        if (productoDb == null || productoDb.CantidadDisponible < item.Cantidad)
                        {
                            await transaction.RollbackAsync();
                            TempData["ErrorVenta"] = $"Stock insuficiente para '{item.Nombre}'. Venta cancelada.";
                            return RedirectToAction("PuntoVenta");
                        }

                        productoDb.CantidadDisponible -= item.Cantidad;
                        _context.Productos.Update(productoDb);

                        var detalle = new DetalleVenta
                        {
                            VeId = venta.VeId,
                            PrId = item.ProductoId,
                            DeCantidad = item.Cantidad,
                            DePrecioUnitario = item.Precio,
                            DeSubtotal = item.Subtotal
                        };
                        _context.DetalleVenta.Add(detalle);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _session.Remove("Carrito");

                    return RedirectToAction( "Index","Ventas", new { id = venta.VeId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorVenta"] = $"Error interno: {ex.Message}";
                    return RedirectToAction("PuntoVenta");
                }
            }
        }
        #endregion


    }
}
