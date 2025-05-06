using Dominio;
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
    
    [TestInitialize]
    public void Inicializar()
    {
        typeof(GestorTareas).GetField("_cantidadTareas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        _gestorProyectos = new GestorProyectos();
        _gestorTareas = new GestorTareas(_gestorProyectos);
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
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Tarea tarea = CrearTarea();
            _gestorTareas.AgregarTareaAlProyecto(1000, _admin, tarea);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearYAgregarProyecto(_admin);

            Tarea tarea = CrearTarea();
            _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _noAdmin, tarea);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdministradorDelProyecto()
        {
            Usuario otroAdmin = CrearAdministradorProyecto();
            otroAdmin.Id = 9;
            Proyecto proyecto = CrearYAgregarProyecto(_admin);

            Tarea tarea = CrearTarea();
            _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, otroAdmin, tarea);
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
            Proyecto proyecto = CrearYAgregarProyecto(_admin); //agrego como miembro a _noAdmin?

            Tarea tarea = CrearTarea();
            _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

            string esperado = $"Se agregó la tarea (id {tarea.Id}) al proyecto '{proyecto.Nombre}'.";

            foreach (var miembro in proyecto.Miembros)
            {
                Assert.AreEqual(2, miembro.Notificaciones.Count);
                Assert.AreEqual(esperado, miembro.Notificaciones[1].Mensaje);
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarTareaDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Tarea tarea = CrearTarea();
            _gestorTareas.EliminarTareaDelProyecto(1000, _admin, tarea.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarTareaDelProyectoo_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearYAgregarProyecto(_admin);
        
            Tarea tarea = CrearTarea();
            _gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
        
            _gestorTareas.EliminarTareaDelProyecto(proyecto.Id, _noAdmin, tarea.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
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
                Assert.AreEqual(3, miembro.Notificaciones.Count);
                Assert.AreEqual(esperado, miembro.Notificaciones[2].Mensaje);
            }
        }
        
        [TestMethod]
        public void ObtenerTareaPorId_DevuelveLaTareaCorrecta()
        {
            Usuario admin = CrearAdministradorProyecto();
            Proyecto proyecto = CrearYAgregarProyecto(admin);
            Tarea tarea1 = CrearTarea();
            Tarea tarea2 = CrearTarea();
            proyecto.AgregarTarea(tarea1);
            proyecto.AgregarTarea(tarea2);
        
            Tarea tareaObtenida1 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea1.Id);
            Tarea tareaObtenida2 = _gestorTareas.ObtenerTareaPorId(proyecto.Id, tarea2.Id);
        
            Assert.AreEqual(tareaObtenida1, tarea1);
            Assert.AreEqual(tareaObtenida2, tarea2);
        }

    
}
