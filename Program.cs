using software.Models;
using software.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar servicios
var usuarios = new List<Usuario>();
builder.Services.AddSingleton(usuarios);
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IUserService, UserService>();

// Crear usuario administrador por defecto
var passwordHasher = new PasswordHasher();
var adminUser = new Usuario
{
    Id = Guid.NewGuid().ToString(),
    Username = "admin",
    Password = passwordHasher.HashPassword("Admin123!"),
    Email = "admin@example.com",
    Role = "Admin"
};
usuarios.Add(adminUser);

// Configurar la autenticación por cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
