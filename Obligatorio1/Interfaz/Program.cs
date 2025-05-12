using Dominio;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Servicios.Gestores;

var builder = WebApplication.CreateBuilder(args);

UsuarioActual usuarioActual = new UsuarioActual();
Usuario usuario = (new Usuario("Juan", "Pérez", new DateTime(1990,1,1), "admin@gmail.com", "Admin123$"));
// usuario.EsAdministradorProyecto = true;
usuario.EsAdministradorSistema = true;
usuarioActual.EstablecerUsuario(usuario);

builder.Services.AddSingleton(usuarioActual);

GestorUsuarios gestorUsuarios = new GestorUsuarios();
builder.Services.AddSingleton(gestorUsuarios);

gestorUsuarios.AgregarUsuario(usuario, usuario);

GestorProyectos gestorProyectos = new GestorProyectos();
// gestorProyectos.CrearProyecto(new Proyecto("Proyecto A", "Descripcion", DateTime.Today.AddDays(1), usuarioActual.UsuarioLogueado, new List<Usuario>()), usuarioActual.UsuarioLogueado);
builder.Services.AddSingleton(gestorProyectos);

GestorRecursos gestorRecursos = new GestorRecursos(gestorProyectos);
gestorRecursos.AgregarRecurso(usuarioActual.UsuarioLogueado, new Recurso("Recurso", "tipo", "descripcion"));
builder.Services.AddSingleton(gestorRecursos);


Usuario usuarioSinRol = gestorUsuarios.CrearUsuario("Sofía", "Martínez", new DateTime(2000, 5, 20), "sofia@gmail.com", "Contrasena123$");
gestorUsuarios.AgregarUsuario(usuario, usuarioSinRol);

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