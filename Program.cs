using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using DevizWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurează DbContext pentru aplicație
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurează DbContext pentru DataProtection keys
builder.Services.AddDbContext<DataProtectionKeyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurează DataProtection să salveze cheile în baza de date
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeyContext>();

// Adaugă servicii MVC (pentru Views și Controllers)
builder.Services.AddControllersWithViews();

// Activează QuestPDF (licența comunitară)
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// --- Aplică migrațiile pentru ambele contexte ---
using (var scope = app.Services.CreateScope())
{
    var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDb.Database.Migrate();

    var dpDb = scope.ServiceProvider.GetRequiredService<DataProtectionKeyContext>();
    dpDb.Database.Migrate();
}

// Middleware pentru erori și HSTS
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

// --- Configurare port pentru Render ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// Setează ruta implicită
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Deviz}/{action=Index}/{id?}");

app.Run();
