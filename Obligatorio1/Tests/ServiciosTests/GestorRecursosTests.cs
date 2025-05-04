using Dominio;
using Servicios.Excepciones;
using Servicios.Gestores;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorRecursosTests
{
    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;

    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(GestorRecursos).GetField("_cantidadRecursos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        _gestorProyectos = new GestorProyectos();
        _gestorRecursos = new GestorRecursos(_gestorProyectos);
    }

    private Usuario CrearAdministradorSistema()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        return admin;
    }
    
    private Usuario CrearAdministradorProyecto()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        return adminProyecto;
    }

    private Usuario CrearUsuarioNoAdmin()
    { 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com", contrasenaEncriptada);
        return usuario;
    }

    private void CrearYAgregarProyecto(Usuario adminProyecto)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto = new Proyecto("Nombre", "Descripción",fechaInicio, adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count);
    }

    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        Usuario adminSistema = CrearAdministradorSistema();

        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(adminSistema, recurso2);

        Assert.AreEqual(2, _gestorRecursos.Recursos.Count);
        Assert.AreEqual(recurso1, _gestorRecursos.Recursos.ElementAt(0));
        Assert.AreEqual(recurso2, _gestorRecursos.Recursos.ElementAt(1));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiProyectoNoAgregaRecurso()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(usuario, recurso);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto); 
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIdsIncrementales()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);

        Assert.AreEqual(1, recurso1.Id);
        Assert.AreEqual(2, recurso2.Id);
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);
        
        Assert.AreEqual(recurso1, _gestorRecursos.ObtenerRecursoPorId(1));
        Assert.AreEqual(recurso2, _gestorRecursos.ObtenerRecursoPorId(2));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void GestorNoObtieneRecursoConIdInexistente()
    {
        Recurso recurso = _gestorRecursos.ObtenerRecursoPorId(20);
    }

    [TestMethod]
    public void SeEliminaUnRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(admin, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoSeEliminaRecursoSiEstaEnUso()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        recurso.IncrementarCantidadDeTareasUsandolo();
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(admin, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoEliminaRecursos()
    {
        Usuario admin = CrearAdministradorSistema();
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(usuario, recurso.Id);
    }

    [TestMethod]
    public void AdminProyectoEliminaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursoNoExclusivo()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);

        Usuario adminProyecto = CrearAdministradorProyecto();
        
        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosExclusivosDeOtrosProyectos()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo gestiona el gestor de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo gestiona el gestor de usuarios
        
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción",fechaInicio, otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(otroAdminProyecto, recurso.Id);
    }

    [TestMethod]
    public void EliminarRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(adminSistema, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("Se eliminó el recurso Analista Senior de tipo Humano - Un analista Senior con experiencia", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void EliminarRecursoNoExclusivoNotificaAdminDeProyectosQueLoUsan()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminSistema, recurso);
        
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = new Proyecto("Nombre", "Descripción",DateTime.Today.AddDays(1), adminProyecto, new List<Usuario>());
        Tarea tarea = new Tarea("Un título", "una descripcion", 3, DateTime.Today.AddDays(10));
        tarea.AgregarRecurso(recurso);
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        proyecto.AgregarTarea(tarea);
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        
        _gestorRecursos.EliminarRecurso(adminSistema, recurso.Id);
        
        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("Se eliminó el recurso Analista Senior de tipo Humano - Un analista Senior con experiencia", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void AdminSistemaModificaNombreDeRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarNombreRecurso(admin, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [TestMethod]
    public void AdminProyectoModificaNombreDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarNombreRecurso(adminProyecto, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarNombre()
    {
        Usuario admin = CrearAdministradorSistema();
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarNombreRecurso(usuario, recurso.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el gestor de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el gestor de usuarios
        
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción", fechaInicio, otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarNombreRecurso(otroAdminProyecto, recurso.Id, "Nuevo nombre");
    }
    
    [TestMethod]
    public void AdminSistemaModificaTipoDeRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarTipoRecurso(admin, recurso.Id, "Nuevo tipo");
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [TestMethod]
    public void AdminProyectoModificaTipoDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarTipoRecurso(adminProyecto, recurso.Id, "Nuevo tipo");
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarTipo()
    {
        Usuario admin = CrearAdministradorSistema();
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarTipoRecurso(usuario, recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el gestor de usuarios
        CrearYAgregarProyecto(adminProyecto);
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el gestor de usuarios
        
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción",fechaInicio, otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarTipoRecurso(otroAdminProyecto, recurso.Id, "Nuevo tipo");
    }
    
    [TestMethod]
    public void AdminSistemaModificaDescripcionDeRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(admin, recurso.Id, "Nueva descripción");
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [TestMethod]
    public void AdminProyectoModificaDescripcionDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarDescripcionRecurso(adminProyecto, recurso.Id, "Nueva descripción");
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarDescripcion()
    {
        Usuario admin = CrearAdministradorSistema();
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(usuario, recurso.Id, "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripciónDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el gestor de usuarios
        CrearYAgregarProyecto(adminProyecto);
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el gestor de usuarios
       
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción", fechaInicio, otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarDescripcionRecurso(otroAdminProyecto, recurso.Id, "Nueva descripción");
    }
    
    [TestMethod]
    public void ModificarNombreDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarNombreRecurso(adminSistema, recurso.Id, "Otro nombre");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Otro nombre', Tipo: 'Humano', Descripción: 'Un analista Senior con experiencia'.", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    //TODO: Si no es exclusivo, se notifica a todos los admin de los proyectos que lo tienen en uso
    
    [TestMethod]
    public void ModificarTipoDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarTipoRecurso(adminSistema, recurso.Id, "Otro tipo");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', Tipo: 'Otro tipo', Descripción: 'Un analista Senior con experiencia'.", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    //TODO: Si no es exclusivo, se notifica a todos los admin de los proyectos que lo tienen en uso
    
    [TestMethod]
    public void ModificarDescripcionDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(adminSistema, recurso.Id, "Otra descripción");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', Tipo: 'Humano', Descripción: 'Otra descripción'.", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    //TODO: Si no es exclusivo, se notifica a todos los admin de los proyectos que lo tienen en uso
}
