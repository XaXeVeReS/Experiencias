using Microsoft.AspNetCore.Authentication.Cookies;
using PROYECTO_LIBRERIA.Datos;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ReporteDatos>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();//// nuevo

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Home/Index";

        //option.ExpireTimeSpan = TimeSpan.(10);// duracion de la sesion 
        option.AccessDeniedPath = "/Home/Privacy";
        option.ExpireTimeSpan = TimeSpan.FromHours(5); // Tiempo de vida de la cookie (1 hora)
        option.SlidingExpiration = false; // No renueva la cookie automáticamente
        option.Cookie.HttpOnly = true; // Protege la cookie de accesos por scripts


    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // habilitAR
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
