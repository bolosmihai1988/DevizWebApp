using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Adaugă servicii MVC (pentru Views și Controllers)
builder.Services.AddControllersWithViews();

// Activează QuestPDF (licența comunitară)
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

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

// Setează ruta implicită către DevizController / Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Deviz}/{action=Index}/{id?}");

// Rulează aplicația
app.Run();
