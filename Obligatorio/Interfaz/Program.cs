using Blazored.LocalStorage;
using Controladores;
using Dominio;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Microsoft.EntityFrameworkCore;
using Repositorios;
using Repositorios.Interfaces;
using Servicios.CaminoCritico;
using Servicios.Gestores;
using Servicios.Gestores.Interfaces;
using Servicios.Notificaciones;
using Servicios.Utilidades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<SqlContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        providerOptions => providerOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorio<Proyecto>, RepositorioProyectos>();
builder.Services.AddScoped<IRepositorio<Recurso>, RepositorioRecursos>();

builder.Services.AddScoped<IGestorUsuarios, GestorUsuarios>();
builder.Services.AddScoped<IGestorProyectos, GestorProyectos>();
builder.Services.AddScoped<IGestorRecursos, GestorRecursos>();
builder.Services.AddScoped<IGestorTareas, GestorTareas>();
builder.Services.AddScoped<INotificador, Notificador>();
builder.Services.AddScoped<ICalculadorCaminoCritico, CaminoCritico>();

builder.Services.AddScoped<ControladorTareas>();
builder.Services.AddScoped<ControladorProyectos>();
builder.Services.AddScoped<ControladorRecursos>();
builder.Services.AddScoped<ControladorUsuarios>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LogicaSesion>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    IRepositorioUsuarios repositorioUsuarios = servicios.GetRequiredService<IRepositorioUsuarios>();
    
    InicializarAdministradorSiNoExiste(repositorioUsuarios);
}

app.Run();


static void InicializarAdministradorSiNoExiste(IRepositorioUsuarios repositorioUsuarios)
{
    const string emailAdmin = "admin@sistema.com";

    var adminExistente = repositorioUsuarios.ObtenerUsuarioPorEmail(emailAdmin);
    if (adminExistente == null)
    {
        string contrasenaPorDefecto = "TaskTrackPro@2025";
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(contrasenaPorDefecto);

        var adminInicial = new Usuario("Admin", "Admin", new DateTime(1999, 01, 01), emailAdmin, contrasenaEncriptada);
        adminInicial.EsAdministradorSistema = true;
        repositorioUsuarios.Agregar(adminInicial);
    }
}