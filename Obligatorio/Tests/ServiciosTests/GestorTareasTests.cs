using Dominio;
using DTOs;
using Repositorios;
using Excepciones;
using Servicios.Gestores;
using Servicios.Utilidades;
using Servicios.CaminoCritico;
using Servicios.Notificaciones;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorTareasTests
{
    private GestorTareas _gestorTareas;
    private GestorProyectos _gestorProyectos;
    private UsuarioDTO _admin;
    private UsuarioDTO _noAdmin;
    private RepositorioUsuarios _repositorioUsuarios;
    private RepositorioProyectos _repositorioProyectos;
    private RepositorioRecursos _repositorioRecursos;
    private Notificador _notificador;
    private CaminoCritico _caminoCritico;

    [TestInitialize]
    public void Inicializar()
    {
        // setup para reiniciar las variables estáticas, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(GestorTareas).GetField("_cantidadTareas",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        typeof(RepositorioProyectos).GetField("_cantidadProyectos",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);

        _notificador = new Notificador();
        _caminoCritico = new CaminoCritico();
        _repositorioUsuarios = new RepositorioUsuarios();
        _repositorioProyectos = new RepositorioProyectos();
        _repositorioRecursos = new RepositorioRecursos();
        _gestorProyectos =
            new GestorProyectos(_repositorioUsuarios, _repositorioProyectos, _notificador, _caminoCritico);
        _gestorTareas = new GestorTareas(_gestorProyectos, _repositorioUsuarios, _repositorioRecursos, _notificador, _caminoCritico);
        _admin = CrearAdministradorProyecto();
        _noAdmin = CrearUsuarioNoAdmin();
    }

    private UsuarioDTO CrearAdministradorSistema()
    {
        //simulación del gestor
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        _repositorioUsuarios.Agregar(admin);
        return UsuarioDTO.DesdeEntidad(admin); //dto
    }

    private UsuarioDTO CrearAdministradorProyecto()
    {
        //simulación del gestor 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        _repositorioUsuarios.Agregar(adminProyecto);
        return UsuarioDTO.DesdeEntidad(adminProyecto); // dto
    }

    private UsuarioDTO CrearUsuarioNoAdmin()
    {
        //simulación del gestor 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        _repositorioUsuarios.Agregar(usuario);
        return UsuarioDTO.DesdeEntidad(usuario); // dto
    }

    private TareaDTO CrearTarea()
    {
        return new TareaDTO()
        {
            Titulo = "tituloTarea",
            Descripcion = "descripcionTarea",
            DuracionEnDias = 3,
            FechaInicioMasTemprana = new DateTime(2026, 01, 01),
        };
    }

    private ProyectoDTO CrearYAgregarProyecto(UsuarioDTO adminProyecto)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        ProyectoDTO proyecto = new ProyectoDTO()
        {
            Nombre = "Proyecto",
            Descripcion = "Descripción",
            FechaInicio = fechaInicio,
            Administrador = adminProyecto,
            Miembros = new List<UsuarioListarDTO>()
        };
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        return proyecto;
    }

    private RecursoDTO CrearYAgregarRecurso(string nombre = "Nombre", string tipo = "Tipo", string descripcion = "Descripción", int capacidad = 1)
    {
        Recurso recurso = new Recurso(nombre, tipo, descripcion, capacidad);
        _repositorioRecursos.Agregar(recurso);
        return RecursoDTO.DesdeEntidad(recurso);
    }
    
    [TestMethod]
    public void Constructor_CreaGestorValido()
    {
        Assert.IsNotNull(_gestorTareas);
    }

    [TestMethod]
    public void AgregarTareaAlProyecto_IncrementaIdConCadaTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        Assert.AreEqual(1, tarea1.Id);
        Assert.AreEqual(2, tarea2.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(1000, _admin, tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionUsuario))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoExiste()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();

        UsuarioDTO usuarioNoExistente = new UsuarioDTO() { Id = 9999 };
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, usuarioNoExistente, tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _noAdmin, tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdministradorDelProyecto()
    {
        UsuarioDTO otroAdmin = CrearAdministradorProyecto();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, otroAdmin, tarea);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiLaTareaIniciaAntesDelProyecto()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        tarea.FechaInicioMasTemprana = proyecto.FechaInicio.AddDays(-1);

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
    }


    [TestMethod]
    public void AgregarTareaAlProyecto_AgregaTareaConIdOK()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        proyecto = _gestorProyectos.ObtenerProyectoPorId(proyecto.Id); // actualización 

        Assert.AreEqual(1, proyecto.Tareas.Count);
        Assert.IsTrue(proyecto.Tareas.Any(t => t.Id == tarea.Id));
        Assert.AreEqual(1, tarea.Id);
    }

    [TestMethod]
    public void AgregarTareaAlProyecto_NotificaAMiembros()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        string esperado = MensajesNotificacion.TareaAgregada(tarea.Id, proyecto.Nombre);

        foreach (UsuarioListarDTO miembro in proyecto.Miembros)
        {
            Assert.AreEqual(esperado, miembro.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        TareaDTO tarea = CrearTarea();
        _gestorTareas.EliminarTareaDelProyecto(1000, _admin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarTareaDelProyectoo_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _noAdmin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdministradorDelProyecto()
    {
        UsuarioDTO otroAdmin = CrearAdministradorProyecto();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, otroAdmin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionTarea))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiOtrasTareasDependenDeElla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        TareaDTO tareaSucesora = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaSucesora);
        _gestorTareas.AgregarDependenciaATarea(_admin, tareaSucesora.Id, tarea.Id, proyecto.Id, "FS");

        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);
    }

    [TestMethod]
    public void EliminarTareaDelProyecto_EliminaTareaOK()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

        Assert.AreEqual(0, proyecto.Tareas.Count);
        Assert.IsFalse(proyecto.Tareas.Contains(tarea));
    }

    [TestMethod]
    public void EliminarTareaDelProyecto_NotificaAMiembros()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

        string esperado = MensajesNotificacion.TareaEliminada(tarea.Id, proyecto.Nombre);

        foreach (UsuarioListarDTO miembro in proyecto.Miembros)
        {
            Assert.AreEqual(esperado, miembro.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    public void ObtenerTareaPorId_DevuelveLaTareaCorrecta()
    {
        UsuarioDTO admin = CrearAdministradorProyecto();
        ProyectoDTO proyecto = CrearYAgregarProyecto(admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, admin, tarea2);

        TareaDTO tareaObtenida1 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea1.Id);
        TareaDTO tareaObtenida2 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea2.Id);

        Assert.AreEqual(tarea1.Id, tareaObtenida1.Id);
        Assert.AreEqual(tarea2.Id, tareaObtenida2.Id);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void ObtenerTareaPorId_LanzaExcepcionSiNoHayTareaConEseId()
    {
        UsuarioDTO admin = CrearAdministradorProyecto();
        ProyectoDTO proyecto = CrearYAgregarProyecto(admin);
        TareaDTO tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, -1);
    }

    [ExpectedException(typeof(ExcepcionProyecto))]
    [TestMethod]
    public void ObtenerTareaPorId_LanzaExcepcionSiNoHayProyecto()
    {
        TareaDTO tareaParaId = CrearTarea();
        TareaDTO tarea = _gestorTareas.ObtenerTareaPorId(4, tareaParaId.Id);
    }

    [TestMethod]
    public void ModificarTitulo_AdminProyectoModificaTituloTareaOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarTituloTarea(_admin, tarea.Id, proyecto.Id, "Nuevo nombre");

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual("Nuevo nombre", tarea.Titulo);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarTitulo_UsuarioNoAdminNoPuedeModificarlo()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarTituloTarea(_noAdmin, tarea.Id, proyecto.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarTitulo_UsuarioAdminNoDelProyectoNoPuedeModificarlo()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarTituloTarea(adminSistema, tarea.Id, proyecto.Id, "Nuevo nombre");
    }

    [TestMethod]
    public void ModificarDescripcion_AdminProyectoModificaDescripcionTareaOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarDescripcionTarea(_admin, tarea.Id, proyecto.Id, "Nueva descripcion");

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual("Nueva descripcion", tarea.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDescripcion_UsuarioNoAdminNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDescripcionTarea(_noAdmin, tarea.Id, proyecto.Id, "Nueva descripcion");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDescripcion_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDescripcionTarea(adminSistema, tarea.Id, proyecto.Id, "Nueva descripcion");
    }

    [TestMethod]
    public void ModificarDuracion_AdminProyectoModificaDuracionTareaOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarDuracionTarea(_admin, tarea.Id, proyecto.Id, 4);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual(4, tarea.DuracionEnDias);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDuracion_UsuarioNoAdminNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDuracionTarea(_noAdmin, tarea.Id, proyecto.Id, 4);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDuracion_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDuracionTarea(adminSistema, tarea.Id, proyecto.Id, 4);
    }

    [TestMethod]
    public void ModificarFechaInicio_AdminProyectoModificaFechaInicioTareaOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);

        _gestorTareas.ModificarFechaInicioTarea(_admin, tarea.Id, proyecto.Id, fechaNueva);
        // No debería lanzar excepción. Luego se llama a CPM que modifica la fecha de inicio por la del proyecto

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual(proyecto.FechaInicio, tarea.FechaInicioMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarFechaInicio_UsuarioNoAdminNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);
        _gestorTareas.ModificarFechaInicioTarea(_noAdmin, tarea.Id, proyecto.Id, fechaNueva);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarFechaInicio_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);
        _gestorTareas.ModificarFechaInicioTarea(adminSistema, tarea.Id, proyecto.Id, fechaNueva);
    }

    [TestMethod]
    public void CambiarEstadoTarea_AdminProyectoCambiaEstadoOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.CambiarEstadoTarea(_admin, tarea.Id, proyecto.Id, EstadoTareaDTO.EnProceso);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual(EstadoTarea.EnProceso.ToString(), tarea.Estado.ToString());
    }

    [TestMethod]
    public void CambiarEstadoTarea_MiembroTareaCambiaEstadoOk()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTareaDTO.EnProceso);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual(EstadoTarea.EnProceso.ToString(), tarea.Estado.ToString());
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void CambiarEstado_UsuarioNoMiembroNoPuedeModificarla()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTareaDTO.EnProceso);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void MiembroNoPuedeCambiarEstadoABloqueada()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTareaDTO.Bloqueada);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void MiembroNoPuedeCambiarEstadoAPendiente()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTareaDTO.Pendiente);
    }

    [TestMethod]
    public void TareaConDependenciaSeBloquea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tareaD = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaD);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarDependenciaATarea(_admin, tarea.Id, tareaD.Id, proyecto.Id, "SS");

        Assert.AreEqual(EstadoTarea.Bloqueada.ToString(), tarea.Estado.ToString());
    }

    [TestMethod]
    public void SeActualizaEstadoCuandoSeCompletaUnaDependencia()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tareaD = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaD);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarDependenciaATarea(_admin, tarea.Id, tareaD.Id, proyecto.Id, "FS");

        _gestorTareas.CambiarEstadoTarea(_noAdmin, tareaD.Id, proyecto.Id, EstadoTareaDTO.EnProceso);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tareaD.Id, proyecto.Id, EstadoTareaDTO.Completada);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.AreEqual(EstadoTarea.Pendiente.ToString(), tarea.Estado.ToString());
    }

    [TestMethod]
    public void AgregarDependencia_AdminAgregaDependenciaCorrectamente()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tareaPrincipal = CrearTarea();
        TareaDTO tareaDependencia = CrearTarea();

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaPrincipal);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaDependencia);

        _gestorTareas.AgregarDependenciaATarea(_admin, tareaPrincipal.Id, tareaDependencia.Id, proyecto.Id, "FS");

        tareaPrincipal = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tareaPrincipal.Id); // actualización
        Assert.IsTrue(tareaPrincipal.Dependencias.Any(dep =>
            dep.TareaPrevia.Id == tareaDependencia.Id && dep.Tipo == "FS"));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionTarea))]
    public void AgregarDependenciaCiclicaLanzaExcepcion()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.AgregarDependenciaATarea(_admin, tarea2.Id, tarea1.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_MiembroNoAdminNoPuedeAgregarLanzaExcepcion()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioNoMiembroNoPuedeAgregarLanzaExcepcion()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioAdminSistemaNoPuedeAgregarLanzaExcepcion()
    {
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioAdminSistemaPeroMiembroNoPuedeAgregarLanzaExcepcion()
    {
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, adminSistema);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    public void EliminarDependencia_AdminEliminaDependenciaCorrectamente()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tareaPrincipal = CrearTarea();
        TareaDTO tareaDependenciaDTO = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaPrincipal);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaDependenciaDTO);

        _gestorTareas.AgregarDependenciaATarea(_admin, tareaPrincipal.Id, tareaDependenciaDTO.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_admin, tareaPrincipal.Id, tareaDependenciaDTO.Id, proyecto.Id);

        tareaPrincipal = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tareaPrincipal.Id); // actualización
        Assert.AreEqual(0, tareaPrincipal.Dependencias.Count);
    }


    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_MiembroNoAdminNoPuedeEliminarLanzaExcepcion()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioNoMiembroNoPuedeEliminarLanzaExcepcion()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioAdminSistemaNoPuedeEliminarLanzaExcepcion()
    {
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioAdminSistemaPeroMiembroNoPuedeEliminarLanzaExcepcion()
    {
        UsuarioDTO adminSistema = CrearAdministradorSistema();
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, adminSistema);
        TareaDTO tarea1 = CrearTarea();
        TareaDTO tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id);
    }

    [TestMethod]
    public void AdminDeProyectoPuedeAgregarMiembroATarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización

        Assert.IsTrue(tarea.UsuariosAsignados.Any(u => u.Id == _noAdmin.Id));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeAgregarMiembroATarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_noAdmin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoSePuedeAgregarMiembroATareaSiNoEsMiembroDelProyecto()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [TestMethod]
    public void SeNotificaLaAsignacionDeUnMiembroALosMiembrosDeLaTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        _admin = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(_admin.Id)); // actualización

        string mensajeEsperado = MensajesNotificacion.CampoTareaAgregado(
            $"miembro {_noAdmin.Nombre} {_noAdmin.Apellido} ({_noAdmin.Email})", tarea.Id, proyecto.Nombre);
        NotificacionDTO ultimaNotificacion = _admin.Notificaciones.Last();

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void AdminDeProyectoPuedeEliminarMiembroDeTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        Assert.IsFalse(tarea.UsuariosAsignados.Any(u => u.Id == _noAdmin.Id));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeEliminarMiembroDeTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_noAdmin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoSePuedeEliminarNoMiembroDeTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [TestMethod]
    public void SeNotificaLaEliminacionDeUnMiembroALosMiembrosDeLaTarea()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        _admin = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(_admin.Id)); // actualización

        string mensajeEsperado = MensajesNotificacion.CampoTareaEliminado(
            $"miembro {_noAdmin.Nombre} {_noAdmin.Apellido} ({_noAdmin.Email})", tarea.Id, proyecto.Nombre);
        NotificacionDTO ultimaNotificacion = _admin.Notificaciones.Last();

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void AdminDeProyectoPuedeAgregarRecursoATarea()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción",1);
        _repositorioRecursos.Agregar(recurso); 
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO, 1);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.IsTrue(tarea.RecursosNecesarios.Any(recurso => recurso.Recurso.Id == recursoDTO.Id));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeAgregarRecursoATarea()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción",1);
        _repositorioRecursos.Agregar(recurso); 
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AsignarRecursoATarea(_noAdmin, tarea.Id, proyecto.Id, recursoDTO, 1);
    }

    [TestMethod]
    public void SeNotificaElAgregadoDeUnRecursoALosMiembrosDeLaTarea()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción",2);
        _repositorioRecursos.Agregar(recurso); 
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO,1);

        _admin = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(_admin.Id)); // actualización
        string mensajeEsperado =
            MensajesNotificacion.CampoTareaAgregado($"recurso {recurso.Nombre}", tarea.Id, proyecto.Nombre);
        NotificacionDTO ultimaNotificacion = _admin.Notificaciones.Last();

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionRecurso))]
    public void AgregarRecursoATarea_LanzaExcepcionSiRecursoNoExiste()
    {
        TareaDTO tarea = CrearTarea();
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción",1);
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO,1);
    }

    [TestMethod]
    public void AdminDeProyectoPuedeEliminarRecursoNecesarioDeTarea()
    {
        RecursoDTO recursoDTO = CrearYAgregarRecurso();

        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO,1);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recursoDTO);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); // actualización
        Assert.IsFalse(tarea.RecursosNecesarios.Any(recurso => recurso.Recurso.Id == recursoDTO.Id));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeEliminarRecursoDeTarea()
    {
        RecursoDTO recursoDTO = CrearYAgregarRecurso();

        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO,1);
        _gestorTareas.EliminarRecursoDeTarea(_noAdmin, tarea.Id, proyecto.Id, recursoDTO);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void NoSePuedeEliminarRecursoDeTareaNoExistente()
    {
        RecursoDTO recursoDTO = CrearYAgregarRecurso();

        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recursoDTO);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void EliminarRecursoNoExistenteDeTareaDaError()
    {
        RecursoDTO recursoNoExistente = CrearYAgregarRecurso();


        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recursoNoExistente);
    }

    [TestMethod]
    public void SeNotificaLaEliminacionDeUnRecursoALosMiembrosDeLaTarea()
    {
        RecursoDTO recursoDTO = CrearYAgregarRecurso();

        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);

        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AsignarRecursoATarea(_admin, tarea.Id, proyecto.Id, recursoDTO, 1);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recursoDTO);

        _admin = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(_admin.Id)); // actualización
        string mensajeEsperado =
            MensajesNotificacion.CampoTareaEliminado($"recurso {recursoDTO.Nombre}", tarea.Id, proyecto.Nombre);
        NotificacionDTO ultimaNotificacion = _admin.Notificaciones.Last();

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void EsMiembroDeTareaDevuelveTrueSiEsMiembro()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);

        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        Assert.IsTrue(_gestorTareas.EsMiembroDeTarea(_noAdmin, tarea.Id, proyecto.Id));
    }

    [TestMethod]
    public void EsMiembroDeTareaDevuelveFalseSiNoEsMiembro()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        Assert.IsFalse(_gestorTareas.EsMiembroDeTarea(_admin, tarea.Id, proyecto.Id));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionRecurso))]
    public void ValidarYAsignarRecurso_LanzaExcepcionSiRecursoNoExiste()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        RecursoDTO recursoInexistente = new RecursoDTO { Id = 999 };
        _gestorTareas.ValidarYAsignarRecurso(_admin, tarea.Id, proyecto.Id, recursoInexistente, 1);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionRecurso))]
    public void ValidarYAsignarRecurso_LanzaExcepcionSiCantidadSuperaCapacidad()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 1);
        _repositorioRecursos.Agregar(recurso);
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);

        _gestorTareas.ValidarYAsignarRecurso(_admin, tarea.Id, proyecto.Id, recursoDTO, 10); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionConflicto))]
    public void ValidarYAsignarRecurso_LanzaExcepcionSiHayConflictoEnFechas()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
    
        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); 

        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 1);
        recurso.AgregarRangoDeUso(tarea.FechaInicioMasTemprana, tarea.FechaFinMasTemprana, 1); 
        _repositorioRecursos.Agregar(recurso);

        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);

        _gestorTareas.ValidarYAsignarRecurso(_admin, tarea.Id, proyecto.Id, recursoDTO, 1);
    }
    
    [TestMethod]
    public void ValidarYAsignarRecurso_AsignaRecursoCorrectamente()
    {
        ProyectoDTO proyecto = CrearYAgregarProyecto(_admin);
        TareaDTO tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 2); 
        _repositorioRecursos.Agregar(recurso);
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);

        _gestorTareas.ValidarYAsignarRecurso(_admin, tarea.Id, proyecto.Id, recursoDTO, 1);

        tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea.Id); 
        Assert.IsTrue(tarea.RecursosNecesarios.Any(r => r.Recurso.Id == recurso.Id));
    }
    
    [TestMethod]
    public void ForzarAsignacion_AsignaRecursoYNotificaMiembros()
    {
        ProyectoDTO proyectoDTO = CrearYAgregarProyecto(_admin);
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(proyectoDTO.Id);

        TareaDTO tareaDTO = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyectoDTO.Id, _admin, tareaDTO);

        Tarea tarea = proyecto.Tareas.First();
        RecursoDTO recursoDTO = CrearYAgregarRecurso("Recurso", "Tipo", "Desc", 5);

        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        
        Usuario miembro = new Usuario("Miembro", "miembro@gmail.com", DateTime.Today.AddYears(-20), "miembro@mail.com", "Contraseña#3");
        _repositorioUsuarios.Agregar(miembro);
        proyecto.Miembros.Add(miembro);
        
        _gestorTareas.ForzarAsignacion(_admin, tarea.Id, proyecto.Id, recursoDTO, 2);

        RecursoNecesario recursoAsignado = tarea.RecursosNecesarios.FirstOrDefault(rn => rn.Recurso.Id == recurso.Id);
        Assert.IsNotNull(recursoAsignado);
        Assert.AreEqual(2, recursoAsignado.Cantidad);

        RangoDeUso rangoAsignado = recurso.RangosEnUso.FirstOrDefault(r =>
            r.FechaInicio == tarea.FechaInicioMasTemprana &&
            r.FechaFin == tarea.FechaFinMasTemprana &&
            r.CantidadDeUsos == 2);
        Assert.IsNotNull(rangoAsignado);

        List<Notificacion> notificaciones = miembro.Notificaciones;
        Assert.IsTrue(notificaciones.Any(n => n.Mensaje.Contains("asignado forzadamente")));
    }
    
    [TestMethod]
    public void EliminarTarea_LiberaRangosEnRecurso()
    {
        UsuarioDTO admin = CrearAdministradorProyecto();
        ProyectoDTO proyecto = CrearYAgregarProyecto(admin);
    
        TareaDTO tarea = CrearTarea();
        tarea.FechaInicioMasTemprana = DateTime.Today.AddDays(1);
        tarea.DuracionEnDias = 2;
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, admin, tarea);
    
        Recurso recurso = new Recurso("Dev", "Humano", "Desarrollador", 1);
        _repositorioRecursos.Agregar(recurso);
        RecursoDTO recursoDTO = RecursoDTO.DesdeEntidad(recurso);
    
        _gestorTareas.ValidarYAsignarRecurso(admin, tarea.Id, proyecto.Id, recursoDTO, 1);
        Assert.AreEqual(1, recurso.RangosEnUso.Count); 
    
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, admin, tarea.Id);
    
        Assert.AreEqual(0, recurso.RangosEnUso.Count);
    }

    [TestMethod]
    public void EncontrarMismoTipoAlternativo_EncuentraRecursoAlternativo()
    {
        UsuarioDTO admin = CrearAdministradorProyecto();
        ProyectoDTO proyectoDTO = CrearYAgregarProyecto(admin);
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(proyectoDTO.Id);
        TareaDTO tarea = CrearTarea();
        
        Recurso recursoOriginal = new Recurso("Original", "TipoX", "desc", 1);
        recursoOriginal.Id = 1;
        _repositorioRecursos.Agregar(recursoOriginal);
        RecursoDTO recursoOriginalDTO = RecursoDTO.DesdeEntidad(recursoOriginal);
        
        Recurso alternativo = new Recurso("Alternativo", "TipoX", "desc", 5);
        alternativo.Id = 2;
        _repositorioRecursos.Agregar(alternativo);
        
        _gestorTareas.EncontrarRecursosAlternativosMismoTipo(admin, proyecto.Id, recursoOriginalDTO,new DateTime(2026, 01, 01), new DateTime(2026, 01, 04), 1);
        
        Usuario adminEntidad = _repositorioUsuarios.ObtenerPorId(admin.Id);
        Assert.IsTrue(adminEntidad.Notificaciones.Any(n => n.Mensaje.Contains("Alternativo")));
        
    }
}