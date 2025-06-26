using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DIEARD.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar o DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Para melhor debugging durante desenvolvimento
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuração do Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Aplica automaticamente as migrations ao arrancar, em qualquer ambiente (inclui Azure)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    

        // 1. Aplica quaisquer migrações pendentes para atualizar a ESTRUTURA da BD. (azure)
        context.Database.Migrate();

        // 2. Inicializa os DADOS (roles, admin, etc.) na BD já atualizada. (azure)
        await DbInicializerDev.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding or migrating the database.");
    }
}

// Configurações específicas de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Em desenvolvimento, mostra erros detalhados
    app.UseMigrationsEndPoint();
}

// Middleware padrão ASP.NET Core
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rota personalizada
app.MapControllerRoute(
    name: "userDiaries",
    pattern: "Home/UserDiaries",
    defaults: new { controller = "Diario", action = "UserDiaries" });

// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Iniciar a aplicação
await app.RunAsync();