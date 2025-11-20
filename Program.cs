using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DevizWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ========================
// CONFIGURARE BAZA DE DATE (PostgreSQL)
// ========================

// Citește connection string-ul din variabila de mediu Render
var databaseUrl = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Fallback la appsettings.json dacă nu e setat (pentru dev local)
if (string.IsNullOrEmpty(databaseUrl))
{
    databaseUrl = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Parsează DATABASE_URL într-un connection string compatibil Npgsql
var uri = new Uri(databaseUrl);
var userInfo = uri.UserInfo.Split(':', 2);

var npgsqlBuilder = new NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = uri.Port,
    Username = userInfo[0],
    Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
    Database = uri.AbsolutePath.TrimStart('/'),
    SslMode = SslMode.Require,
    TrustServerCertificate = true,
    Pooling = true
};

// Configurează DbContext cu PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(npgsqlBuilder.ConnectionString));

// ========================
// SERVICII MVC / Views
// ========================
builder.Services.AddControllersWithViews();

// Activează QuestPDF (licența comunitară)
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// ========================
// MIGRAȚII AUTOMATE
// ========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
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
