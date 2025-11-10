using AlmacenWeb.Data; 
using AlmacenWeb.Models;
using AlmacenWeb.ViewModels; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Diagnostics;
using System.Threading.Tasks; 

namespace AlmacenWeb.Controllers
{
    [Authorize] // Protegido: Solo usuarios logueados pueden ver el Dashboard
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; 

        // recibir el AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; 
        }

        
        public async Task<IActionResult> Index()
        {
            
            var vm = new DashboardViewModel();

            // --- Lógica para buscar datos reales ---

            //  Productos (¡Este dato es real!)
            vm.TotalProductos = await _context.Productos.CountAsync();
            //  Stock Bajo (¡Este dato es real!)
            // Definimos "Stock Bajo" como <= 10 unidades
            vm.ProductosStockBajo = await _context.Productos.CountAsync(p => p.CantidadDisponible <= 10);

            //  Clientes y Ventas 
            vm.TotalClientes = await _context.Clientes.CountAsync();
            var hoy = DateTime.Today;
            vm.VentasHoy = await _context.Ventas
                                .CountAsync(v => v.VeFecha.Date == hoy);

            vm.TotalVentasHoy = await _context.Ventas
                                .Where(v => v.VeFecha.Date == hoy)
                                .SumAsync(v => (decimal?)v.VeTotal) ?? 0; 

            
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