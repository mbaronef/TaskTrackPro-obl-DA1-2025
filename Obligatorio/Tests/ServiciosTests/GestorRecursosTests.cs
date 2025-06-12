using Dominio;
using DTOs;
using Repositorios;
using Servicios.CaminoCritico;
using Excepciones;
using Servicios.Gestores;
using Servicios.Notificaciones;
using Servicios.Utilidades;
using Microsoft.EntityFrameworkCore;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorRecursosTests
{
    private RepositorioRecursos _repositorioRecursos;
    private RepositorioUsuarios _repositorioUsuarios;
    private RepositorioProyectos _repositorioProyectos;

    private SqlContext _contexto;

    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;
    private UsuarioDTO _adminSistemaDTO;

    private Notificador _notificador;
    private CaminoCritico _caminoCritico;

    [TestInitialize]
    public void SetUp()
    {
        _contexto = SqlContextFactory.CreateMemoryContext();

        _notificador = new Notificador();
        _caminoCritico = new CaminoCritico();

        _repositorioRecursos = new RepositorioRecursos(_contexto);
        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _repositorioProyectos = new RepositorioProyectos(_contexto);

        _gestorProyectos =
            new GestorProyectos(_repositorioUsuarios, _repositorioProyectos, _notificador, _caminoCritico);

        _gestorRecursos =
            new GestorRecursos(_repositorioRecursos, _gestorProyectos, _repositorioUsuarios, _notificador);

        _adminSistemaDTO = CrearAdministradorSistemaDTO();
    }

    private UsuarioDTO CrearAdministradorSistemaDTO()
    {
        //simulación del gestor 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        _repositorioUsuarios.Agregar(admin);
        return UsuarioDTO.DesdeEntidad(admin); // dto
    }

    private Usuario CrearAdministradorProyecto()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        _repositorioUsuarios.Agregar(adminProyecto);
        return adminProyecto;
    }

    private UsuarioDTO CrearUsuarioNoAdminDTO()
    {
        //simulación del gestor 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        _repositorioUsuarios.Agregar(usuario);
        return UsuarioDTO.DesdeEntidad(usuario); // dto
    }

    private Tarea CrearTarea()
    {
        return new Tarea("Un título", "una descripcion", 3, DateTime.Today.AddDays(10));
    }

    private Proyecto CrearYAgregarProyecto(Usuario adminProyecto)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        ProyectoDTO proyecto = new ProyectoDTO()
        {
            Nombre = "Nombre", Descripcion = "Descripción", FechaInicio = fechaInicio,
            Administrador = UsuarioDTO.DesdeEntidad(adminProyecto)
        };
        _gestorProyectos.CrearProyecto(proyecto, UsuarioDTO.DesdeEntidad(adminProyecto));
        return _gestorProyectos.ObtenerProyectoDominioPorId(proyecto.Id);
    }

    private RecursoDTO CrearRecursoDTO()
    {
        return new RecursoDTO()
            { Nombre = "Analista Senior", Tipo = "Humano", Descripcion = "Un analista Senior con experiencia" };
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.ObtenerRecursosGenerales().Count);
    }

    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(2, _gestorRecursos.ObtenerRecursosGenerales().Count);
        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(0).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(1).Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiProyectoNoAgregaRecurso()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(usuario, recurso, false);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        RecursoDTO recursoDTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recursoDTO, true);

        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursoPorId(1).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursoPorId(2).Id);
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void GestorNoObtieneRecursoConIdInexistente()
    {
        RecursoDTO recurso = _gestorRecursos.ObtenerRecursoPorId(20);
    }

    [TestMethod]
    public void SeEliminaUnRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.ObtenerRecursosGenerales().Count());
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void NoSeEliminaRecursoSiEstaEnUso()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        recurso.CantidadDeTareasUsandolo++;
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoEliminaRecursos()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(usuario, recurso.Id);
    }

    [TestMethod]
    public void AdminProyectoEliminaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id);
        Assert.AreEqual(0, _repositorioRecursos.ObtenerTodos().Count());
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.EliminarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosExclusivosDeOtrosProyectos()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        UsuarioDTO otroAdminProyecto = UsuarioDTO.DesdeEntidad(CrearAdministradorProyecto());
        ProyectoDTO otroProyecto = new ProyectoDTO()
        {
            Nombre = "Otro Nombre", Descripcion = "Descripción", FechaInicio = DateTime.Today.AddDays(1),
            Administrador = otroAdminProyecto
        };
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(otroAdminProyecto, recurso.Id);
    }

    [TestMethod]
    public void EliminarRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado =
            MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion);

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void EliminarRecursoNoExclusivoNotificaAdminDeProyectos()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);

        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado =
            MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion);

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void AdminSistemaModificaNombreDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Nuevo nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [TestMethod]
    public void AdminProyectoModificaNombreDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarNombreRecurso(adminProyectoDTO, recurso.Id, "Nuevo nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarNombre()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarNombreRecurso(usuario, recurso.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otro nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios

        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id, "Nuevo nombre");
    }

    [TestMethod]
    public void AdminSistemaModificaTipoDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Nuevo tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [TestMethod]
    public void AdminProyectoModificaTipoDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarTipoRecurso(adminProyectoDTO, recurso.Id, "Nuevo tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarTipo()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarTipoRecurso(usuario, recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));


        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarTipoRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarTipoRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otro tipo");
    }

    [TestMethod]
    public void AdminSistemaModificaDescripcionDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Nueva descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [TestMethod]
    public void AdminProyectoModificaDescripcionDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarDescripcionRecurso(adminProyectoDTO, recurso.Id, "Nueva descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarDescripcion()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarDescripcionRecurso(usuario, recurso.Id, "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripciónDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto);

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarDescripcionRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id,
            "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripcionDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otra descripción");
    }

    [TestMethod]
    public void ModificarNombreDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Otro nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarNombreDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AsignarRecurso(recurso.AEntidad());
        proyecto.AgregarTarea(tarea);

        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Otro nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarTipoDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Otro tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarTipoDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AsignarRecurso(recurso.AEntidad());
        proyecto.AgregarTarea(tarea);

        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Otro tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarDescripcionDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Otra descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void ModificarDescripcionDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        Tarea tarea = CrearTarea();
        tarea.AsignarRecurso(recurso.AEntidad());
        proyecto.AgregarTarea(tarea);

        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Otra descripción");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        Assert.AreEqual(
            "El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Humano', descripción: 'Otra descripción'",
            ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeMuestranRecursosGeneralesOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto);
        RecursoDTO recursoExclusivo = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recursoExclusivo, true);

        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(2, _gestorRecursos.ObtenerRecursosGenerales().Count);
        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(0).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(1).Id);
    }

    [TestMethod]
    public void SeMuestranRecursosExclusivosDeUnProyectoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);

        RecursoDTO recursoExclusivo1 = CrearRecursoDTO();
        RecursoDTO recursoExclusivo2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo1, true);
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo2, true);

        List<RecursoDTO> recursosExclusivosDelProyecto = _gestorRecursos.ObtenerRecursosExclusivos(proyecto.Id);
        Assert.AreEqual(2, recursosExclusivosDelProyecto.Count());
        Assert.AreEqual(recursoExclusivo1.Id, recursosExclusivosDelProyecto.ElementAt(0).Id);
        Assert.AreEqual(recursoExclusivo2.Id, recursosExclusivosDelProyecto.ElementAt(1).Id);
    }

    [TestMethod]
    public void SeObtieneUnRecursoExclusivoPorIdOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recursoExclusivo = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo, true);

        RecursoDTO recursoObtenido = _gestorRecursos.ObtenerRecursoExclusivoPorId(proyecto.Id, recursoExclusivo.Id);

        Assert.AreEqual(recursoObtenido.Id, recursoExclusivo.Id);
        Assert.AreEqual(recursoObtenido.Nombre, recursoExclusivo.Nombre);
        Assert.AreEqual(recursoObtenido.Tipo, recursoExclusivo.Tipo);
        Assert.AreEqual(recursoObtenido.Descripcion, recursoExclusivo.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void ObtenerUnRecursoExclusivoInexistenteLanzaExcepcion()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto);

        _gestorRecursos.ObtenerRecursoExclusivoPorId(proyecto.Id, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void NoPuedeAgregarUnRecursoUnSolicitanteInexistente()
    {
        UsuarioDTO usuarioNoEnRepositorio = new UsuarioDTO()
        {
            Id = 99999, Nombre = "Juan", Apellido = "Pérez", FechaNacimiento = new DateTime(2000, 01, 01),
            Email = "juan@perez.com", Contrasena = "Juan123>$%"
        };
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(usuarioNoEnRepositorio, recurso, false);
    }
}