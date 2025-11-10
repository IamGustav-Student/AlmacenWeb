using AlmacenWeb.Models;
using AlmacenWeb.Services; 
using Microsoft.EntityFrameworkCore;
using System.Linq; 

namespace AlmacenWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }


        
        public void DbInitialize(Encrypt encryptService)
        {
            
            this.Database.EnsureCreated();

            
            if (!this.Roles.Any())
            {
                var roles = new Rol[]
                {
                    new Rol { RoNombre = "Admin", RoDescripcion = "Administrador del Sistema" },
                    new Rol { RoNombre = "Empleador", RoDescripcion = "Dueño del negocio" },
                    new Rol { RoNombre = "Empleado", RoDescripcion = "Empleado" },
                    new Rol { RoNombre = "Usuario", RoDescripcion = "Usuario/Cliente registrado" }
                };

                this.Roles.AddRange(roles);
                this.SaveChanges();
            }

            
            if (!this.Usuarios.Any())
            {
                var adminRol = this.Roles.FirstOrDefault(r => r.RoNombre == "Admin");

                if (adminRol != null)
                {
                    var adminUser = new Usuario
                    {
                        UsNombre = "Administrador",
                        UsApellido = "del Sistema",
                        UsEmail = "admin@almacenweb.com",
                        UsPassword = encryptService.HashPassword("admin123"),
                        UsActivo = true,
                        UsFechaRegistro = DateTime.Now,
                        RoId = adminRol.RoId,
                        date_created = DateTime.Now
                    };
                    if (!this.Clientes.Any())
                    {
                        this.Clientes.Add(new Cliente
                        {
                            ClNombre = "Consumidor",
                            ClApellido = "Final",
                            ClDniCuit = "00000000",
                            ClDireccion = "N/A",
                            ClTelefono = "N/A",
                            ClEmail = "cliente@generico.com"
                        });
                        this.SaveChanges();
                    }

                    this.Usuarios.Add(adminUser);
                    this.SaveChanges();
                }
            }
        }
    }
}