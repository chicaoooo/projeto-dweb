using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DIEARD.Data;
using Microsoft.Extensions.Logging; // Adicione esta linha

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Adicione esta linha explicitamente
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ROTAS ESPECÍFICAS PARA USERDIARIES - ADICIONAR ANTES DA ROTA DEFAULT
app.MapControllerRoute(
    name: "userDiaries",
    pattern: "Home/UserDiaries",
    defaults: new { controller = "Diario", action = "UserDiaries" });

app.MapControllerRoute(
    name: "diarioUserDiaries",
    pattern: "Diario/UserDiaries",
    defaults: new { controller = "Diario", action = "UserDiaries" });

// ROTAS DE DEBUG
app.MapControllerRoute(
    name: "debugCategorias",
    pattern: "Diario/DebugCategorias",
    defaults: new { controller = "Diario", action = "DebugCategorias" });

app.MapControllerRoute(
    name: "userDiariesSimple",
    pattern: "Diario/UserDiariesSimple",
    defaults: new { controller = "Diario", action = "UserDiariesSimple" });

app.MapControllerRoute(
    name: "testeCategorias",
    pattern: "Diario/TesteCategorias",
    defaults: new { controller = "Diario", action = "TesteCategorias" });

app.MapControllerRoute(
    name: "testeSimples",
    pattern: "Diario/TesteSimples",
    defaults: new { controller = "Diario", action = "TesteSimples" });

// ROTA DEFAULT - DEVE SER A ÚLTIMA
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();