var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Ascultă pe toate interfețele din container
app.Urls.Add("http://0.0.0.0:80");

// Pagină simplă de test
app.MapGet("/", () => "Hello from DevizWebApp!");

// Rulează aplicația
app.Run();
