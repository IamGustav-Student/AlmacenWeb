using AlmacenWeb.Data;
using AlmacenWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// --- Configuración de Servicios ---

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
// Añadir servicios de Sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<Encrypt>();
// Configurar los EmailSettings desde appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Registrar el servicio de Email
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 4. Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duración de la sesión
        options.LoginPath = "/Acceso/Index"; // Página de Login
        options.LogoutPath = "/Acceso/Salir"; // Página de Logout
        options.AccessDeniedPath = "/Home/Privacy"; // Página para "Acceso Denegado"
    });

// Configuración de Autorización (para los Roles)
builder.Services.AddAuthorization(options =>
{
    // Aquí se podrían agregar políticas de roles, pero por ahora
    // usaremos la autorización simple [Authorize(Roles = "Admin")]
});

var app = builder.Build();

// --- Inicilizador/Seeder de Base de Datos ---

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var encryptService = services.GetRequiredService<Encrypt>();
        
        context.DbInitialize(encryptService);
    }
    catch (Exception ex)
    {
       
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); 
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
