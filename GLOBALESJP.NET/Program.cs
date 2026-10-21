using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configuración de puertos y URLs:
// 1. En Desarrollo local: respeta launchSettings.json (http://localhost:5000 o https://localhost:5001) permitiendo que Visual Studio lance automáticamente el navegador.
// 2. En Render / Producción: lee la variable de entorno dinámicamente PORT o usa el puerto 8080 por defecto.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}
else if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:8080");
}

// Servicios MVC ligeros sin dependencias de base de datos
builder.Services.AddControllersWithViews();

// Servicio de envío de correo (MailKit + Gmail SMTP)
builder.Services.AddTransient<GLOBALESJP.NET.Services.IEmailService, GLOBALESJP.NET.Services.EmailService>();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Encabezados reenviados para proxies inversos (Cloudflare, Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// Ruta predeterminada dirigida a HomeController.Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
