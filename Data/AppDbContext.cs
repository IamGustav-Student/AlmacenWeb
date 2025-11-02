using AlmacenWeb.Models;
using AlmacenWeb.Services; // ¡Nuevo using!
using Microsoft.EntityFrameworkCore;
using System.Linq; // ¡Nuevo using!

namespace AlmacenWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Agregamos los nuevos DbSet ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVenta { get; set; }


        // Esta lógica crea los Roles
        // y el usuario Admin si la base de datos está vacía.
        public void DbInitialize(Encrypt encryptService)
        {
            // Asegurarse que la base de datos fue creada
            this.Database.EnsureCreated();

            // 1. Inicializar Roles
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

            // 2. Inicializar Usuario Admin
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
                        UsPassword = encryptService.HashPassword("admin123"), // ¡Contraseña hasheada!
                        UsActivo = true,
                        UsFechaRegistro = DateTime.Now,
                        RoId = adminRol.RoId,
                        date_created = DateTime.Now
                    };

                    this.Usuarios.Add(adminUser);
                    this.SaveChanges();
                }
            }
        }
    }
}