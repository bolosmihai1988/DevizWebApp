using System.Net;
using System.Text;

namespace DevizWebApp.Security
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = Environment.GetEnvironmentVariable("APP_USER");
            var pass = Environment.GetEnvironmentVariable("APP_PASS");

            // Dacă nu sunt setate, nu blocăm (util pentru rulare locală)
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers.Authorization.ToString();

            if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Basic ".Length..].Trim();
                try
                {
                    var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var parts = credentialString.Split(':', 2);

                    if (parts.Length == 2 && parts[0] == user && parts[1] == pass)
                    {
                        await _next(context);
                        return;
                    }
                }
                catch { }
            }

            context.Response.Headers.WWWAuthenticate = "Basic realm=\"DevizWebApp\"";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }
    }
}
