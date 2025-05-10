using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Gestores;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorRecursosTests
{
    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;
    private Usuario _adminSistema;

    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(RepositorioRecursos).GetField("_cantidadRecursos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);

       _gestorProyectos = new GestorProyectos();
        _gestorRecursos = new GestorRecursos(_gestorProyectos);
        _adminSistema = CrearAdministradorSistema();
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

    private Tarea CrearTarea()
    {
        return new Tarea("Un título", "una descripcion", 3, DateTime.Today.AddDays(10));
    }

    private Proyecto CrearYAgregarProyecto(Usuario adminProyecto)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto = new Proyecto("Nombre", "Descripción",fechaInicio, adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        return proyecto;
    }

    private Recurso CrearRecurso()
    {
        return new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.Recursos.ObtenerTodos().Count);
    }

    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        Recurso recurso1 = CrearRecurso();
        Recurso recurso2 = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso2);

        Assert.AreEqual(2, _gestorRecursos.Recursos.ObtenerTodos().Count);
        Assert.AreEqual(recurso1, _gestorRecursos.Recursos.ObtenerTodos().ElementAt(0));
        Assert.AreEqual(recurso2, _gestorRecursos.Recursos.ObtenerTodos().ElementAt(1));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiProyectoNoAgregaRecurso()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(usuario, recurso);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto); 
        
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        Recurso recurso1 = CrearRecurso();
        Recurso recurso2 = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso2);
        
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
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.EliminarRecurso(_adminSistema, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.ObtenerTodos().Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoSeEliminaRecursoSiEstaEnUso()
    {
        Recurso recurso = CrearRecurso();
        recurso.IncrementarCantidadDeTareasUsandolo();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.EliminarRecurso(_adminSistema, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoEliminaRecursos()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.EliminarRecurso(usuario, recurso.Id);
    }

    [TestMethod]
    public void AdminProyectoEliminaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.ObtenerTodos().Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursoNoExclusivo()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);

        Usuario adminProyecto = CrearAdministradorProyecto();
        
        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosExclusivosDeOtrosProyectos()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo gestiona el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);
       
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo gestiona el repo de usuarios
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción",DateTime.Today.AddDays(1),otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(otroAdminProyecto, recurso.Id);
    }

    [TestMethod]
    public void EliminarRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(_adminSistema, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("Se eliminó el recurso Analista Senior de tipo Humano - Un analista Senior con experiencia", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void EliminarRecursoNoExclusivoNotificaAdminDeProyectos()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        
        _gestorRecursos.EliminarRecurso(_adminSistema, recurso.Id);
        
        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("Se eliminó el recurso Analista Senior de tipo Humano - Un analista Senior con experiencia", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void AdminSistemaModificaNombreDeRecursoOk()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarNombreRecurso(_adminSistema, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [TestMethod]
    public void AdminProyectoModificaNombreDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarNombreRecurso(adminProyecto, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarNombre()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarNombreRecurso(usuario, recurso.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios
        
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción",DateTime.Today.AddDays(1),otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto( otroProyecto, otroAdminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarNombreRecurso(otroAdminProyecto, recurso.Id, "Nuevo nombre");
    }
    
    [TestMethod]
    public void AdminSistemaModificaTipoDeRecursoOk()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarTipoRecurso(_adminSistema, recurso.Id, "Nuevo tipo");
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [TestMethod]
    public void AdminProyectoModificaTipoDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarTipoRecurso(adminProyecto, recurso.Id, "Nuevo tipo");
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarTipo()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarTipoRecurso(usuario, recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción",DateTime.Today.AddDays(1),otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto( otroProyecto, otroAdminProyecto);
        
        
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarTipoRecurso(otroAdminProyecto, recurso.Id, "Nuevo tipo");
    }
    
    [TestMethod]
    public void AdminSistemaModificaDescripcionDeRecursoOk()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistema, recurso.Id, "Nueva descripción");
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [TestMethod]
    public void AdminProyectoModificaDescripcionDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarDescripcionRecurso(adminProyecto, recurso.Id, "Nueva descripción");
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarDescripcion()
    {
        Usuario usuario = CrearUsuarioNoAdmin();
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(usuario, recurso.Id, "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripciónDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción",DateTime.Today.AddDays(1),otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto( otroProyecto, otroAdminProyecto);

        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarDescripcionRecurso(otroAdminProyecto, recurso.Id, "Nueva descripción");
    }
    
    [TestMethod]
    public void ModificarNombreDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarNombreRecurso(_adminSistema, recurso.Id, "Otro nombre");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Otro nombre', tipo: 'Humano', descripción: 'Un analista Senior con experiencia'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void ModificarNombreDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AgregarRecurso(recurso);
        proyecto.AgregarTarea(tarea);
        
        _gestorRecursos.ModificarNombreRecurso(_adminSistema, recurso.Id, "Otro nombre");
        
        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Otro nombre', tipo: 'Humano', descripción: 'Un analista Senior con experiencia'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarTipoDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarTipoRecurso(_adminSistema, recurso.Id, "Otro tipo");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Otro tipo', descripción: 'Un analista Senior con experiencia'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void ModificarTipoDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AgregarRecurso(recurso);
        proyecto.AgregarTarea(tarea);
        
        _gestorRecursos.ModificarTipoRecurso(_adminSistema, recurso.Id, "Otro tipo");
        
        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Otro tipo', descripción: 'Un analista Senior con experiencia'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void ModificarDescripcionDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistema, recurso.Id, "Otra descripción");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Humano', descripción: 'Otra descripción'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
    
    [TestMethod]
    public void ModificarDescripcionDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        Recurso recurso = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso);
        
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AgregarRecurso(recurso);
        proyecto.AgregarTarea(tarea);
        
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistema, recurso.Id, "Otra descripción");
        
        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual("El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Humano', descripción: 'Otra descripción'", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeMuestranRecursosGeneralesOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto); 
        Recurso recursoExclusivo = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recursoExclusivo);
        
        Recurso recurso1 = CrearRecurso();
        Recurso recurso2 = CrearRecurso();
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(_adminSistema, recurso2);

        Assert.AreEqual(2, _gestorRecursos.ObtenerRecursosGenerales().Count);
        Assert.AreEqual(recurso1, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(0));
        Assert.AreEqual(recurso2, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(1));
    }
    
    [TestMethod]
    public void SeMuestranRecursosExclusivosDeUnProyectoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        
        Recurso recursoExclusivo1 = CrearRecurso();
        Recurso recursoExclusivo2 = CrearRecurso();
        _gestorRecursos.AgregarRecurso(adminProyecto, recursoExclusivo1);
        _gestorRecursos.AgregarRecurso(adminProyecto, recursoExclusivo2);
        
        List<Recurso> recursosExclusivosDelProyecto = _gestorRecursos.ObtenerRecursosExclusivos(proyecto.Id);
        Assert.AreEqual(2, recursosExclusivosDelProyecto.Count());
        Assert.AreEqual(recursoExclusivo1, recursosExclusivosDelProyecto.ElementAt(0));
        Assert.AreEqual(recursoExclusivo2, recursosExclusivosDelProyecto.ElementAt(1));
    }
}