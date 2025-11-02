using AlmacenWeb.Data; // <-- Añadir este using
using AlmacenWeb.Models;
using AlmacenWeb.ViewModels; // <-- Añadir este using
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Añadir este using
using System.Diagnostics;
using System.Threading.Tasks; // <-- Añadir este using

namespace AlmacenWeb.Controllers
{
    [Authorize] // Protegido: Solo usuarios logueados pueden ver el Dashboard
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // <-- Inyectamos el DbContext

        // Actualizamos el constructor para recibir el AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; // <-- Asignamos el DbContext
        }

        // Convertimos el método Index en asíncrono
        public async Task<IActionResult> Index()
        {
            // Creamos el ViewModel
            var vm = new DashboardViewModel();

            // --- Lógica para buscar datos reales ---

            // 1. Productos (¡Este dato es real!)
            vm.TotalProductos = await _context.Productos.CountAsync();
            // 2. Stock Bajo (¡Este dato es real!)
            // Definimos "Stock Bajo" como <= 10 unidades
            vm.ProductosStockBajo = await _context.Productos.CountAsync(p => p.CantidadDisponible <= 10);

            // 3. Clientes y Ventas (Aún no tenemos los módulos, usamos los datos del archivo original)
            vm.TotalClientes = await _context.Clientes.CountAsync();
            var hoy = DateTime.Today;
            vm.VentasHoy = await _context.Ventas
                                .CountAsync(v => v.VeFecha.Date == hoy);

            vm.TotalVentasHoy = await _context.Ventas
                                .Where(v => v.VeFecha.Date == hoy)
                                .SumAsync(v => (decimal?)v.VeTotal) ?? 0; // Usamos SumAsync y ?? 0 por si no hay ventas

            // Enviamos el ViewModel a la vista
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}