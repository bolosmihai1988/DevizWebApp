using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DevizWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Npgsql;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ========================
// CONFIGURARE BAZA DE DATE (PostgreSQL)
// ========================
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
                  ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(databaseUrl) &&
    (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    var npgsqlBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true,
        Pooling = true
    };

    databaseUrl = npgsqlBuilder.ConnectionString;
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(databaseUrl));

// ========================
// SERVICII MVC / Views
// ========================
builder.Services.AddControllersWithViews();
QuestPDF.Settings.License = LicenseType.Community;

// ========================
// DATA PROTECTION PERSISTENT (antiforgery fix)
// ========================
var keysPath = "/tmp/keys";
if (!Directory.Exists(keysPath))
{
    Directory.CreateDirectory(keysPath);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("DevizWebApp");

// ========================
// BUILD APP
// ========================
var app = builder.Build();

// ========================
// MIGRAȚII AUTOMATE
// ========================
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    Console.WriteLine("Eroare la migrații: " + ex.Message);
}

// ========================
// MIDDLEWARE STANDARD
// ========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ========================
// CONFIGURARE PORT PENTRU RENDER
// ========================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// ========================
// ROUTING
// ========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Deviz}/{action=Index}/{id?}");

// ========================
// RUN
// ========================
app.Run();
