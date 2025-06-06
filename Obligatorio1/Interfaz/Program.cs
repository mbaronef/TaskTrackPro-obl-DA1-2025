using Blazored.LocalStorage;
using Dominio;
using Interfaces.InterfacesRepositorios;
using Interfaces.InterfacesServicios;
using InterfacesServicios;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Repositorios;
using Servicios.Gestores;
using Servicios.Notificaciones;
using Servicios.Utilidades;

var builder = WebApplication.CreateBuilder(args);

INotificador _notificador = new Notificador();
ICalculadorCaminoCritico _calculadorCaminoCritico = new CaminoCritico();
IRepositorioUsuarios repositorioUsuarios = new RepositorioUsuarios();
IRepositorio<Proyecto> repositorioProyectos = new RepositorioProyectos();
IRepositorio<Recurso> repositorioRecursos = new RepositorioRecursos();

builder.Services.AddSingleton<INotificador, Notificador>();
builder.Services.AddScoped<ICalculadorCaminoCritico, CaminoCritico>();

GestorUsuarios gestorUsuarios = new GestorUsuarios(repositorioUsuarios, _notificador);
GestorProyectos gestorProyectos = new GestorProyectos(repositorioUsuarios, repositorioProyectos, _notificador, _calculadorCaminoCritico);
GestorRecursos gestorRecursos = new GestorRecursos(repositorioRecursos, gestorProyectos, repositorioUsuarios, _notificador);
GestorTareas gestorTareas = new GestorTareas(gestorProyectos, repositorioUsuarios, _notificador, _calculadorCaminoCritico);

// Add services to the container.
builder.Services.AddSingleton(repositorioUsuarios);
builder.Services.AddSingleton(repositorioProyectos);
builder.Services.AddSingleton(repositorioRecursos);

builder.Services.AddSingleton(gestorUsuarios);
builder.Services.AddSingleton(gestorProyectos);
builder.Services.AddSingleton(gestorRecursos);
builder.Services.AddSingleton(gestorTareas);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LogicaSesion>();

// simulación de datos para probar la interfaz
InicializarDatosHardcodeados(gestorUsuarios, gestorProyectos, gestorRecursos, gestorTareas, repositorioUsuarios);

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

void InicializarDatosHardcodeados(
    GestorUsuarios gestorUsuarios, 
    GestorProyectos gestorProyectos, 
    GestorRecursos gestorRecursos, 
    GestorTareas gestorTareas, IRepositorioUsuarios repositorioUsuarios)
{
   /* //Usuario admin inicial
    Usuario adminInicial = repositorioUsuarios.ObtenerPorId(0);
    UsuarioDTO adminInicialDTO = UsuarioDTO.DesdeEntidad(adminInicial);
    
    //Usuario admin sistema y admin proyecto a la vez - Juan Pérez
    UsuarioDTO adminProyectoYSistemaDTO = new UsuarioDTO()
    {
        Nombre = "Juan", Apellido = "Pérez", FechaNacimiento = new DateTime(1990, 1, 1),
        Email = "admin@gmail.com", Contrasena = "Admin123$"
    };
    gestorUsuarios.CrearYAgregarUsuario(adminInicialDTO, adminProyectoYSistemaDTO);
    Usuario adminProyectoYSistema = repositorioUsuarios.ObtenerPorId(adminProyectoYSistemaDTO.Id);
    adminProyectoYSistema.EsAdministradorSistema = true;
    adminProyectoYSistema.EsAdministradorProyecto = true;
/*
    // Usuario solo administrador de proyecto 
    Usuario usuarioSoloAdminProyecto = gestorUsuarios.CrearUsuario("Admin", "Proyecto", new DateTime(2000, 5, 20), "adminProyecto@gmail.com", "Contrasena123$");
    usuarioSoloAdminProyecto.EsAdministradorProyecto = true;
    gestorUsuarios.AgregarUsuario(adminProyectoYSistema, usuarioSoloAdminProyecto);

    // Crear proyecto con Juan Pérez como administrador
    var proyectoA = new Proyecto("Proyecto A", "Descripcion", DateTime.Today, adminProyectoYSistema, new List<Usuario> { adminProyectoYSistema });
    gestorProyectos.CrearProyecto(proyectoA, adminProyectoYSistema);

    // Agregar un recurso general
    gestorRecursos.AgregarRecurso(adminProyectoYSistema, new Recurso("Recurso GENERAL", "tipo", "descripción"), false);
    // Agregar un recurso exclusivo
    gestorRecursos.AgregarRecurso(adminProyectoYSistema, new Recurso("Recurso EXCLUSIVO del proyecto A", "tipo", "descripción"), true);

    // Crear usuario sin rol y agregar a proyecto - Sofía Martínez
    Usuario usuarioSinRol = gestorUsuarios.CrearUsuario("Sofía", "Martínez", new DateTime(2000, 5, 20), "sofia@gmail.com", "Contrasena123$");
    gestorUsuarios.AgregarUsuario(adminProyectoYSistema, usuarioSinRol);
    gestorProyectos.AgregarMiembroAProyecto(proyectoA.Id, adminProyectoYSistema, usuarioSinRol);

    // Crear tareas y asignar al proyecto A
    Tarea tarea1 = new Tarea("Tarea 1", "Descripcion 1", 2, DateTime.Today.AddDays(1));
    gestorTareas.AgregarTareaAlProyecto(proyectoA.Id, adminProyectoYSistema, tarea1);
    Tarea tarea2 = new Tarea("Tarea 2", "Descripcion 2", 2, DateTime.Today.AddDays(2));
    gestorTareas.AgregarTareaAlProyecto(proyectoA.Id, adminProyectoYSistema, tarea2);
    
    // Asignar a Sofía Martínez a la tarea 2
    gestorTareas.AgregarMiembroATarea(adminProyectoYSistema, tarea2.Id, proyectoA.Id , usuarioSinRol);*/
}









