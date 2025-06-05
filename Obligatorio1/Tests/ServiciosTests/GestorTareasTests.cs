using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Gestores;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorTareasTests
{
    private GestorTareas _gestorTareas;
    private GestorProyectos _gestorProyectos;
    private Usuario _admin;
    private Usuario _noAdmin;
    private MockNotificador _mockNotificador;
    private MockCalculadorCaminoCritico _mockCalculadorCaminoCritico;

    [TestInitialize]
    public void Inicializar()
    {
        typeof(GestorTareas).GetField("_cantidadTareas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        typeof(RepositorioProyectos).GetField("_cantidadProyectos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        _mockCalculadorCaminoCritico = new MockCalculadorCaminoCritico();
        _mockNotificador = new MockNotificador();
        _gestorProyectos = new GestorProyectos(_mockNotificador, _mockCalculadorCaminoCritico);
        _gestorTareas = new GestorTareas(_gestorProyectos, _mockNotificador, _mockCalculadorCaminoCritico);
        _admin = CrearAdministradorProyecto();
        _noAdmin = CrearUsuarioNoAdmin();
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
        Usuario usuario = new Usuario("Tomas", "Pérez", new DateTime(2003, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        return usuario;
    }

    private Tarea CrearTarea()
    {
        return new Tarea("Un título", "una descripcion", 3, DateTime.Today.AddDays(10));
    }

    private Proyecto CrearYAgregarProyecto(Usuario adminProyecto)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", fechaInicio, adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        return proyecto;
    }

    [TestMethod]
    public void Constructor_CreaGestorValido()
    {
        Assert.IsNotNull(_gestorTareas);
    }

    [TestMethod]
    public void AgregarTareaAlProyecto_IncrementaIdConCadaTarea()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        Assert.AreEqual(1, tarea1.Id);
        Assert.AreEqual(2, tarea2.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(1000, _admin, tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _noAdmin, tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdministradorDelProyecto()
    {
        Usuario otroAdmin = CrearAdministradorProyecto();
        otroAdmin.Id = 9;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, otroAdmin, tarea);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void AgregarTareaAlProyecto_LanzaExcepcionSiLaTareaIniciaAntesDelProyecto()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        tarea.ModificarFechaInicioMasTemprana(proyecto.FechaInicio.AddDays(-1));

        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
    }
    
    
    [TestMethod]
    public void AgregarTareaAlProyecto_AgregaTareaConIdOK()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        Assert.AreEqual(1, proyecto.Tareas.Count);
        Assert.IsTrue(proyecto.Tareas.Contains(tarea));
        Assert.AreEqual(tarea.Id, 1);
    }

    [TestMethod]
    public void AgregarTareaAlProyecto_NotificaAMiembros()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        string esperado = $"Se agregó la tarea (id {tarea.Id}) al proyecto '{proyecto.Nombre}'.";

        foreach (Usuario miembro in proyecto.Miembros)
        {
            Assert.AreEqual(esperado, miembro.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        Tarea tarea = CrearTarea();
        _gestorTareas.EliminarTareaDelProyecto(1000, _admin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarTareaDelProyectoo_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _noAdmin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiSolicitanteNoElAdministradorDelProyecto()
    {
        Usuario otroAdmin = CrearAdministradorProyecto();
        otroAdmin.Id = 9;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, otroAdmin, tarea.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionTarea))]
    public void EliminarTareaDelProyecto_LanzaExcepcionSiOtrasTareasDependenDeElla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        Tarea tareaSucesora = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaSucesora);
        tareaSucesora.AgregarDependencia(new Dependencia("FS", tarea));
        
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);
    }

    [TestMethod]
    public void EliminarTareaDelProyecto_EliminaTareaOK()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

        Assert.AreEqual(0, proyecto.Tareas.Count);
        Assert.IsFalse(proyecto.Tareas.Contains(tarea));
    }

    [TestMethod]
    public void EliminarTareaDelProyecto_NotificaAMiembros()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

        string esperado = $"Se eliminó la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";

        foreach (var miembro in proyecto.Miembros)
        {
            Assert.AreEqual(esperado, miembro.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    public void ObtenerTareaPorId_DevuelveLaTareaCorrecta()
    {
        Usuario admin = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        Tarea tareaObtenida1 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea1.Id);
        Tarea tareaObtenida2 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea2.Id);

        Assert.AreEqual(tarea1, tareaObtenida1);
        Assert.AreEqual(tarea2, tareaObtenida2);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void ObtenerTareaPorId_LanzaExcepcionSiNoHayTareaConEseId()
    {
        Usuario admin = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(admin);
        Tarea tarea = _gestorTareas.ObtenerTareaPorId(proyecto.Id, -1);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ObtenerTareaPorId_LanzaExcepcionSiNoHayProyecto()
    {
        Tarea tareaParaId = CrearTarea();
        Tarea tarea = _gestorTareas.ObtenerTareaPorId(4, tareaParaId.Id);
    }

    [TestMethod]
    public void ModificarTitulo_AdminProyectoModificaTituloTareaOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarTituloTarea(_admin, tarea.Id, proyecto.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", tarea.Titulo);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarTitulo_UsuarioNoAdminNoPuedeModificarlo()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarTituloTarea(_noAdmin, tarea.Id, proyecto.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarTitulo_UsuarioAdminNoDelProyectoNoPuedeModificarlo()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Usuario adminSistema = CrearAdministradorSistema();
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarTituloTarea(adminSistema, tarea.Id, proyecto.Id, "Nuevo nombre");
    }

    [TestMethod]
    public void ModificarDescripcion_AdminProyectoModificaDescripcionTareaOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarDescripcionTarea(_admin, tarea.Id, proyecto.Id, "Nueva descripcion");
        Assert.AreEqual("Nueva descripcion", tarea.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDescripcion_UsuarioNoAdminNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDescripcionTarea(_noAdmin, tarea.Id, proyecto.Id, "Nueva descripcion");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDescripcion_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Usuario adminSistema = CrearAdministradorSistema();
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDescripcionTarea(adminSistema, tarea.Id, proyecto.Id, "Nueva descripcion");
    }

    [TestMethod]
    public void ModificarDuracion_AdminProyectoModificaDuracionTareaOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.ModificarDuracionTarea(_admin, tarea.Id, proyecto.Id, 4);
        Assert.AreEqual(4, tarea.DuracionEnDias);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDuracion_UsuarioNoAdminNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDuracionTarea(_noAdmin, tarea.Id, proyecto.Id, 4);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarDuracion_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Usuario adminSistema = CrearAdministradorSistema();
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.ModificarDuracionTarea(adminSistema, tarea.Id, proyecto.Id, 4);
    }

    [TestMethod]
    public void ModificarFechaInicio_AdminProyectoModificaFechaInicioTareaOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);

        _gestorTareas.ModificarFechaInicioTarea(_admin, tarea.Id, proyecto.Id, fechaNueva);
        // No debería lanzar excepción. Luego se llama a CPM que modifica la fecha de inicio por la del proyecto
        //Assert.AreEqual(proyecto.FechaInicio, tarea.FechaInicioMasTemprana);
        Assert.AreEqual(fechaNueva, tarea.FechaInicioMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarFechaInicio_UsuarioNoAdminNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);
        _gestorTareas.ModificarFechaInicioTarea(_noAdmin, tarea.Id, proyecto.Id, fechaNueva);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ModificarFechaInicio_UsuarioAdminNoDelProyectoNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _admin.Id = 50;
        Tarea tarea = CrearTarea();
        Usuario adminSistema = CrearAdministradorSistema();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        DateTime fechaNueva = new DateTime(2030, 01, 01);
        _gestorTareas.ModificarFechaInicioTarea(adminSistema, tarea.Id, proyecto.Id, fechaNueva);
    }

    [TestMethod]
    public void CambiarEstadoTarea_AdminProyectoCambiaEstadoOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);

        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

        _gestorTareas.CambiarEstadoTarea(_admin, tarea.Id, proyecto.Id, EstadoTarea.EnProceso);
        Assert.AreEqual(tarea.Estado, EstadoTarea.EnProceso);
    }

    [TestMethod]
    public void CambiarEstadoTarea_MiembroTareaCambiaEstadoOk()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _noAdmin.Id = 40;
        proyecto.AsignarMiembro(_noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTarea.EnProceso);
        Assert.AreEqual(tarea.Estado, EstadoTarea.EnProceso);
    }

    [ExpectedException(typeof(ExcepcionProyecto))]
    [TestMethod]
    public void CambiarEstado_UsuarioNoMiembroNoPuedeModificarla()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _noAdmin.Id = 40;
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTarea.EnProceso);
        Assert.AreEqual(tarea.Estado, EstadoTarea.EnProceso);
    }

    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void MiembroNoPuedeCambiarEstadoABloqueada()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _noAdmin.Id = 40;
        proyecto.AsignarMiembro(_noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTarea.Bloqueada);
    }
    
    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void MiembroNoPuedeCambiarEstadoAPendiente()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _noAdmin.Id = 40;
        proyecto.AsignarMiembro(_noAdmin);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tarea.Id, proyecto.Id, EstadoTarea.Pendiente);
    }

    [TestMethod]
    public void TareaConDependenciaSeBloquea()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _noAdmin.Id = 1; // se hardcodea por simplicidad de tests, los ids los maneja el repo.
        proyecto.AsignarMiembro(_noAdmin);
        
        Tarea tareaD = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaD);
        
        Tarea tarea = CrearTarea();
        tarea.AgregarDependencia(new Dependencia("SS", tareaD));
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        
        Assert.AreEqual(EstadoTarea.Bloqueada, tarea.Estado);
    }

    [TestMethod]
    public void SeActualizaEstadoCuandoSeCompletaUnaDependencia()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        _noAdmin.Id = 1; // se hardcodea por simplicidad de tests, los ids los maneja el repo.
        proyecto.AsignarMiembro(_noAdmin);
        
        Tarea tareaD = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaD);
        
        Tarea tarea = CrearTarea();
        tarea.AgregarDependencia(new Dependencia("FS", tareaD));
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        
        
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tareaD.Id, proyecto.Id, EstadoTarea.EnProceso);
        _gestorTareas.CambiarEstadoTarea(_noAdmin, tareaD.Id, proyecto.Id, EstadoTarea.Completada);
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
    }

    [TestMethod]
    public void AgregarDependencia_AdminAgregaDependenciaCorrectamente()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tareaPrincipal = CrearTarea();
        Tarea tareaDependencia = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaPrincipal);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaDependencia);

        _gestorTareas.AgregarDependenciaATarea(_admin, tareaPrincipal.Id, tareaDependencia.Id, proyecto.Id, "FS");
        Dependencia dependencia = new Dependencia("FS", tareaDependencia);
        Assert.IsTrue(tareaPrincipal.Dependencias.Contains(dependencia));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionTarea))]
    public void AgregarDependenciaCiclicaLanzaExcepcion()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.AgregarDependenciaATarea(_admin, tarea2.Id, tarea1.Id, proyecto.Id, "FS");
        
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_MiembroNoAdminNoPuedeAgregarLanzaExcepcion()
    {
        _noAdmin.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        proyecto.AsignarMiembro(_noAdmin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioNoMiembroNoPuedeAgregarLanzaExcepcion()
    {
        _noAdmin.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioAdminSistemaNoPuedeAgregarLanzaExcepcion()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        adminSistema.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarDependencia_UsuarioAdminSistemaPeroMiembroNoPuedeAgregarLanzaExcepcion()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        adminSistema.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        proyecto.AsignarMiembro(adminSistema);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
    }

    [TestMethod]
    public void EliminarDependencia_AdminEliminaDependenciaCorrectamente()
    {
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tareaPrincipal = CrearTarea();
        Tarea tareaDependencia = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaPrincipal);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tareaDependencia);
        _gestorTareas.AgregarDependenciaATarea(_admin, tareaPrincipal.Id, tareaDependencia.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_admin, tareaPrincipal.Id, tareaDependencia.Id, proyecto.Id);
        Dependencia dependencia = new Dependencia("FS", tareaDependencia);
        Assert.IsFalse(tareaPrincipal.Dependencias.Contains(dependencia));
    }


    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_MiembroNoAdminNoPuedeEliminarLanzaExcepcion()
    {
        _noAdmin.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        proyecto.AsignarMiembro(_noAdmin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id);
    }
        
    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioNoMiembroNoPuedeEliminarLanzaExcepcion()
    {
        _noAdmin.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(_noAdmin, tarea1.Id, tarea2.Id, proyecto.Id);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioAdminSistemaNoPuedeEliminarLanzaExcepcion()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        adminSistema.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id);
    }
        
    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarDependencia_UsuarioAdminSistemaPeroMiembroNoPuedeEliminarLanzaExcepcion()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        adminSistema.Id = 40;
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        proyecto.AsignarMiembro(adminSistema);
        Tarea tarea1 = CrearTarea();
        Tarea tarea2 = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea1);
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea2);

        _gestorTareas.AgregarDependenciaATarea(_admin, tarea1.Id, tarea2.Id, proyecto.Id, "FS");
        _gestorTareas.EliminarDependenciaDeTarea(adminSistema, tarea1.Id, tarea2.Id, proyecto.Id);
    }

    [TestMethod]
    public void AdminDeProyectoPuedeAgregarMiembroATarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        Assert.IsTrue(tarea.UsuariosAsignados.Contains(_noAdmin));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeAgregarMiembroATarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_noAdmin, tarea.Id, proyecto.Id, _noAdmin);
    }
    
    [ExpectedException(typeof(ExcepcionProyecto))]
    [TestMethod]
    public void NoSePuedeAgregarMiembroATareaSiNoEsMiembroDelProyecto()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [TestMethod]
    public void SeNotificaLaAsignacionDeUnMiembroALosMiembrosDeLaTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        string mensajeEsperado = $"Se agregó el miembro {_noAdmin.ToString()} de la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";
        Notificacion ultimaNotificacion = _admin.Notificaciones.Last();
        
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }
    
    [TestMethod]
    public void AdminDeProyectoPuedeEliminarMiembroDeTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        Assert.IsFalse(tarea.UsuariosAsignados.Contains(_noAdmin));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeEliminarMiembroDeTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_noAdmin, tarea.Id, proyecto.Id, _noAdmin);
    }
    
    [ExpectedException(typeof(ExcepcionProyecto))]
    [TestMethod]
    public void NoSePuedeEliminarNoMiembroDeTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        
        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
    }

    [TestMethod]
    public void SeNotificaLaEliminacionDeUnMiembroALosMiembrosDeLaTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.EliminarMiembroDeTarea(_admin, tarea.Id, proyecto.Id, _noAdmin);

        string mensajeEsperado = $"Se eliminó el miembro {_noAdmin.ToString()} de la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";
        Notificacion ultimaNotificacion = _admin.Notificaciones.Last();
        
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }
    
    [TestMethod]
    public void AdminDeProyectoPuedeAgregarRecursoATarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarRecursoATarea(_admin, tarea.Id, proyecto.Id, recurso);

        Assert.IsTrue(tarea.RecursosNecesarios.Contains(recurso));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeAgregarRecursoATarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarRecursoATarea(_noAdmin, tarea.Id, proyecto.Id, recurso);
    }

    [TestMethod]
    public void SeNotificaElAgregadoDeUnRecursoALosMiembrosDeLaTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarMiembroATarea(_admin, tarea.Id, proyecto.Id, _noAdmin);
        _gestorTareas.AgregarRecursoATarea(_admin, tarea.Id, proyecto.Id, recurso);

        string mensajeEsperado = $"Se agregó el recurso {recurso.Nombre} de la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";
        Notificacion ultimaNotificacion = _admin.Notificaciones.Last();
        
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }
    
    [TestMethod]
    public void AdminDeProyectoPuedeEliminarRecursoNecesarioDeTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarRecursoATarea(_admin, tarea.Id, proyecto.Id, recurso);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recurso);

        Assert.IsFalse(tarea.RecursosNecesarios.Contains(recurso));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminNoPuedeEliminarRecursoDeTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarRecursoATarea(_admin, tarea.Id, proyecto.Id, recurso);
        _gestorTareas.EliminarRecursoDeTarea(_noAdmin, tarea.Id, proyecto.Id, recurso);
    }
    
    [ExpectedException(typeof(ExcepcionTarea))]
    [TestMethod]
    public void NoSePuedeEliminarRecursoDeTareaNoExistente()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recurso);
    }

    [TestMethod]
    public void SeNotificaLaEliminacionDeUnRecursoALosMiembrosDeLaTarea()
    {
        _admin.Id = 1;
        _noAdmin.Id = 2;
        
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
        _gestorProyectos.AgregarMiembroAProyecto(proyecto.Id, _admin, _noAdmin);
        
        Tarea tarea = CrearTarea();
        _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        _gestorTareas.AgregarRecursoATarea(_admin, tarea.Id, proyecto.Id, recurso);
        _gestorTareas.EliminarRecursoDeTarea(_admin, tarea.Id, proyecto.Id, recurso);

        string mensajeEsperado = $"Se eliminó el recurso {recurso.Nombre} de la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";
        Notificacion ultimaNotificacion = _admin.Notificaciones.Last();
        
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }
}