using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using AlmacenWeb.Models;

namespace AlmacenWeb.Data
{
    public class AppDbContext : DbContext
    {
        // Database context 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            

        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Rol> Roles { get; set; }

    }

}
