using Dominio;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Servicios.Gestores;

var builder = WebApplication.CreateBuilder(args);

UsuarioActual usuarioActual = new UsuarioActual();
Usuario usuario = (new Usuario("Juan", "Pérez", new DateTime(1990,1,1), "admin@gmail.com", "Admin123$"));
usuario.EsAdministradorProyecto = true;
usuarioActual.EstablecerUsuario(usuario);

builder.Services.AddSingleton(usuarioActual);

GestorProyectos gestorProyectos = new GestorProyectos();
gestorProyectos.CrearProyecto(new Proyecto("Proyecto A", "Descripcion", DateTime.Today.AddDays(1), usuarioActual.UsuarioLogueado, new List<Usuario>()), usuarioActual.UsuarioLogueado);
builder.Services.AddSingleton(gestorProyectos);

// Add services to the container.
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

app.Run();