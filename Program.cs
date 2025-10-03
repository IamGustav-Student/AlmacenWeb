using AlmacenWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Sección para inicializar roles
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService();
//    var userManager = scope.ServiceProvider.GetRequiredService();

//    string[] roles = new string[] { "Admin", "Dueño", "Empleado", "Proveedor" };
//    foreach (var role in roles)
//    {
//        if (!roleManager.RoleExistsAsync(role).Result)
//        {
//            roleManager.CreateAsync(new IdentityRole(role)).Wait();
//        }
//    }

//    // Crear un usuario de prueba con el rol "Admin" si no existe
//    string emailAdmin = "admin@almacen.com";
//    if (userManager.FindByEmailAsync(emailAdmin).Result == null)
//    {
//        IdentityUser adminUser = new IdentityUser
//        {
//            UserName = emailAdmin,
//            Email = emailAdmin
//        };

//        IdentityResult result = userManager.CreateAsync(adminUser, "Password123!").Result;
//        if (result.Succeeded)
//        {
//            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
//        }
//    }
//}

app.Run();
