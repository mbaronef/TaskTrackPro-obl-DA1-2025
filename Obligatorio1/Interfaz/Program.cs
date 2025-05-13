using Blazored.LocalStorage;
using Dominio;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Servicios.Gestores;
using Servicios.Utilidades;

var builder = WebApplication.CreateBuilder(args);

string contrasena = UtilidadesContrasena.ValidarYEncriptarContrasena("Admin123$");
Usuario usuario = (new Usuario("Juan", "Pérez", new DateTime(1990,1,1), "admin@gmail.com", contrasena));
usuario.EsAdministradorProyecto = true;
usuario.EsAdministradorSistema = true;

builder.Services.AddSingleton(usuario);

GestorUsuarios gestorUsuarios = new GestorUsuarios();
builder.Services.AddSingleton(gestorUsuarios);

gestorUsuarios.AgregarUsuario(usuario, usuario);

Usuario usuarioSoloAdminProyecto = gestorUsuarios.CrearUsuario("Admin", "Proyecto", new DateTime(2000, 5, 20), "adminProyecto@gmail.com", "Contrasena123$");
usuarioSoloAdminProyecto.EsAdministradorProyecto = true;
gestorUsuarios.AgregarUsuario(usuario, usuarioSoloAdminProyecto);

GestorProyectos gestorProyectos = new GestorProyectos();
gestorProyectos.CrearProyecto(new Proyecto("Proyecto A", "Descripcion", DateTime.Today.AddDays(1), usuario, new List<Usuario>{usuario}), usuario);
builder.Services.AddSingleton(gestorProyectos);

GestorRecursos gestorRecursos = new GestorRecursos(gestorProyectos);
// gestorRecursos.AgregarRecurso(usuarioSoloAdminProyecto, new Recurso("Recurso", "tipo", "descripcion"), true);
gestorRecursos.AgregarRecurso(usuario, new Recurso("Recurso GENERAL", "tipo", "descripcion"), false);
builder.Services.AddSingleton(gestorRecursos);

GestorTareas gestorTareas = new GestorTareas(gestorProyectos);
Tarea tarea1 = new Tarea("Tarea 1","Descripcion 1", 2,DateTime.Today.AddDays(1));
gestorTareas.AgregarTareaAlProyecto(1,usuario, tarea1);
builder.Services.AddSingleton(gestorTareas);

Usuario usuarioSinRol = gestorUsuarios.CrearUsuario("Sofía", "Martínez", new DateTime(2000, 5, 20), "sofia@gmail.com", "Contrasena123$");
gestorUsuarios.AgregarUsuario(usuario, usuarioSinRol);
gestorProyectos.AgregarMiembroAProyecto(1, usuario, usuarioSinRol);
Tarea tarea2 = new Tarea("Tarea 2","Descripcion 2", 2,DateTime.Today.AddDays(2));
gestorTareas.AgregarTareaAlProyecto(1,usuario, tarea2);
gestorTareas.AgregarMiembroATarea(usuario, tarea2.Id, 1, usuarioSinRol);

// Add services to the container.
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

app.Run();








