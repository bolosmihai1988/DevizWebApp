using DevizWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;

var builder = WebApplication.CreateBuilder(args);

// --- Configurează DbContext principal ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configurează DbContext pentru DataProtection keys ---
builder.Services.AddDbContext<DataProtectionKeyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configurează DataProtection să folosească baza de date ---
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeyContext>();

// --- MVC / Controllers ---
builder.Services.AddControllersWithViews();

// --- QuestPDF ---
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// --- Aplică migrațiile la startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var dpDb = scope.ServiceProvider.GetRequiredService<DataProtectionKeyContext>();
    dpDb.Database.Migrate();
}

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// --- Ruta implicită ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Deviz}/{action=Index}/{id?}");

app.Run();
