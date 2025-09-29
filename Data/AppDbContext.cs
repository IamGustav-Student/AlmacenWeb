using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using AlmacenWeb.Models;

namespace AlmacenWeb.Data
{
    public class AppDbContext : DbContext
    {
        // Database context implementation
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Constructor logic here

        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }

    }

}
