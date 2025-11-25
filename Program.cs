using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DevizWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurează DbContext pentru PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adaugă servicii MVC (pentru Views și Controllers)
builder.Services.AddControllersWithViews();

// Activează QuestPDF (licența comunitară)
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// --- APLICĂ MIGRAȚII AUTOMAT LA STARTUP ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Aplică migrațiile la startup
}

// Middleware pentru gestionarea erorilor și HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware standard
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// --- CONFIGURARE PORT PENTRU RENDER ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// Setează ruta implicită către DevizController / Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Deviz}/{action=Index}/{id?}");

// Rulează aplicația
app.Run();
