using ManejoPresupuestos.Models;
using ManejoPresupuestos.Services;
using ManejoPresupuestos.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

/*Creamos politicas para que solamente los usuarios autenticados
puedan utilizar los elementos de nuestra app*/
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Agregamos las politicas
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});

builder.Services.AddTransient<IRepositoryTipoCuenta, RepositoryTipoCuenta>();
builder.Services.AddTransient<IRepositoryCuenta, RepositoryCuenta>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryTransaccion, RepositoryTransaccion>();
builder.Services.AddTransient<IRepositoryUsuarios, RepositoryUsuarios>();
builder.Services.AddTransient<IServicioUsuario, ServicioUsuario>();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();
builder.Services.AddTransient<IServicioGenerarExcel, ServicioGenerarExcel>();
builder.Services.AddHttpContextAccessor(); // Inyectamos acceso a peticiones http
builder.Services.AddAutoMapper(typeof(Program)); // Inyectamos automapper
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddTransient<SignInManager<Usuario>>();

// Configuramos las opciones de identificacion y de contraseñas
builder.Services.AddIdentityCore<Usuario>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesDeErrorIdentity>();

// Agregamos autenticacion a traves de cookies
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options => {
    options.LoginPath = "/Usuarios/Login";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute (
    name: "default",
    pattern: "{controller=Transaccion}/{action=Index}/{id?}"
);

app.Run();