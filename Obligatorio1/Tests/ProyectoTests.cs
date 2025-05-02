using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class ProyectoTests
    {
        private Proyecto _proyecto;
        private GestorUsuarios _gestorUsuarios;
        private Usuario _admin;
        private List<Usuario> _miembros;
        
        [TestInitialize]
        public void AntesDeCadaTest()
        {
            _gestorUsuarios = new GestorUsuarios();
            _admin = new Usuario();
            _gestorUsuarios.agregarUsuario(_admin);
            _miembros = new List<Usuario>(); // SIN el "List<Usuario>" adelante
        }
        
        //Constructor
        [TestMethod]
        public void Constructor_ConParametrosAsignadosCorrectamente()
        {
            string nombre = "Proyecto 1";
            string descripcion = "Descripción";
            
            _proyecto = new Proyecto (nombre, descripcion, _admin, _miembros);
            
            Assert.AreEqual(nombre, _proyecto.Nombre);
            Assert.AreEqual(descripcion, _proyecto.Descripcion);
            Assert.AreEqual(_admin, _proyecto.Administrador);
            Assert.AreEqual(_miembros, _proyecto.Miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionSupera400Caracteres()
        {
            string descripcion = new string('a', 401); 

            _proyecto = new Proyecto("Proyecto", descripcion, _admin, _miembros);
        }
        
        [TestMethod]
        public void Constructor_PermiteDescripcionConMenosDe400Caracteres()
        {
            string descripcion = new string('a', 399);

            _proyecto = new Proyecto("Proyecto", descripcion, _admin, _miembros);

            Assert.AreEqual(descripcion, _proyecto.Descripcion);
        }
        
        [TestMethod]
        public void Constructor_PermiteDescripcionDeHasta400Caracteres()
        {
            string descripcion = new string('a', 400); // 400 caracteres exactos
            
            _proyecto = new Proyecto("Proyecto", descripcion,  _admin, _miembros);

            Assert.AreEqual(descripcion, _proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsVacio()
        { 
            _proyecto = new Proyecto("", "Descripción válida",  _admin, _miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsNull()
        {
            _proyecto = new Proyecto(null, "Descripción válida",  _admin, _miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
        { 
            _proyecto = new Proyecto("Nombre válido", "",  _admin, _miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsNull()
        {
            _proyecto = new Proyecto("Nombre válido", null,  _admin, _miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiAdministradorEsNull()
        {
            _proyecto = new Proyecto("Nombre", "Descripción",  null, _miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiMiembrosEsNull()
        {
            _proyecto = new Proyecto("Nombre", "Descripción",  _admin, null);
        }
        
        //Asignar ID
        
        [TestMethod]
        public void AsignarID_DeberiaAsignarCorrectamenteElId()
        {
            _admin.Id = 1;
            _proyecto = new Proyecto( "Proyecto Test", "Descripción de prueba", _admin, _miembros);
            _proyecto.AsignarId(42);
            Assert.AreEqual(42, _proyecto.Id);
        }
        
        // validacion de parametros: FechaInicio y FechaFinMasTemprana
        
        [TestMethod]
        public void FechaInicio_InicializadaConFechaActualPorDefecto()
        {
            DateTime antes = DateTime.Now;
            _proyecto = new Proyecto("Nombre", "Descripción", _admin, _miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(_proyecto.FechaInicio >= antes && _proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
        [TestMethod]
        public void FechaFinMasTemprana_InicializadaConFechaActualPorDefecto()
        {
            DateTime antes = DateTime.Now;
            _proyecto = new Proyecto("Nombre", "Descripción", _admin, _miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(_proyecto.FechaFinMasTemprana >= antes && _proyecto.FechaFinMasTemprana <= despues); // porque DateTime.Now cambia
        }
        
        //AgregarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void AgregarTarea_AgregarUnaTareaALaLista()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);

            Tarea tarea1 = new Tarea();
            
            _proyecto.AgregarTarea(tarea1); 
            
            Assert.IsTrue(_proyecto.Tareas.Contains(tarea1));
            Assert.AreEqual(1, _proyecto.Tareas.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaEsNull()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción",  _admin, _miembros);
            Tarea tarea1 = null;
            _proyecto.AgregarTarea(tarea1); 
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaYaEstaEnTareas()
        {
            Tarea tarea1 = new Tarea(); 
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            _proyecto.AgregarTarea(tarea1);
            _proyecto.AgregarTarea(tarea1);
        }
        
        //eliminarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarTarea_EliminaTareaDeLaLista()
        {
            Tarea tarea = new Tarea();
            tarea.Id = 1;
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            _proyecto.AgregarTarea(tarea);
            _proyecto.EliminarTarea(1);

            Assert.IsFalse(_proyecto.Tareas.Any(t => t.Id == 1));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTarea_LanzaExcepcionSiTareaNoExiste()
        {
            Tarea tarea = new Tarea();
            tarea.Id = 1;
            
            _proyecto = new Proyecto("Proyecto", "Descripción", _admin, _miembros);
            _proyecto.AgregarTarea(tarea);
            _proyecto.EliminarTarea(2); // ID que no existe
        }
        
        
        //AsignarMiembro (En GESTOR: solo admin proyecto puede)

        [TestMethod]
        public void AsignarMiembro_AgregarUsuarioALaListaDeMiembros()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción",  _admin, _miembros);

            Usuario nuevoMiembro = new Usuario();

            _proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(_proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioEsNull()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción",  _admin, _miembros);

            _proyecto.AsignarMiembro(null); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioYaEstaEnMiembros()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción",  _admin, _miembros);

            _proyecto.AsignarMiembro(_admin); 
        }
        
        //EliminarMiembro (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarMiembro_EliminaUsuarioCorrectamenteDeLaLista()
        {
            _admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            _proyecto.AsignarMiembro(miembro);
            _proyecto.EliminarMiembro(2); // Elimino al miembro

            Assert.IsFalse(_proyecto.Miembros.Any(u => u.Id == 2));
            Assert.AreEqual(1, _proyecto.Miembros.Count);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzaExcepcionSiElUsuarioNoExisteEnMiembros()
        {
            _admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            
            _proyecto.AsignarMiembro(miembro);
            _proyecto.EliminarMiembro(3); // ID que no existe
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzaExcepcionSiUsuarioEsAdministrador()
        {
            _admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            
            _proyecto = new Proyecto("Proyecto 1", "Descripción",  _admin, _miembros);
            
            _proyecto.AsignarMiembro(miembro);
            _proyecto.EliminarMiembro(1); // Intenta eliminar al admin
        }
        
        //EsAdministrador
        [TestMethod]
        public void EsAdministrador_RetornaTrueSiUsuarioEsAdministrador()
        {
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);

            bool resultado = _proyecto.EsAdministrador(_admin);

            Assert.IsTrue(resultado);
        }
        
        [TestMethod]
        public void EsAdministrador_RetornarFalseSiUsuarioNoEsAdministrador()
        {
            Usuario otro = new Usuario();
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            _gestorUsuarios.agregarUsuario(otro);
            _proyecto.AsignarMiembro(otro);
            bool resultado = _proyecto.EsAdministrador(otro);
            Assert.IsFalse(resultado);
        }
        
        //MODIFICACIONES
        
        //modificarFechaDeInicio (EN GESTOR: puede ser modificada por admin sistema o por admin proyecto)
        
        [TestMethod]
        public void ModificarFechaInicio_ActualizaLaFechaOK()
        { 
            Proyecto _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _proyecto.ModificarFechaInicio(nuevaFecha);

            Assert.AreEqual(nuevaFecha, _proyecto.FechaInicio);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaInicio_LanzaExcepcionSiFechaEsAnteriorAHoy() // capaz no estaria tan bien (?) Podria ser un error al ingresar los datos
        {
            _proyecto = new Proyecto("Proyecto", "Descripción",  _admin, _miembros);

            DateTime fechaPasada = DateTime.Now.AddDays(-1);

            _proyecto.ModificarFechaInicio(fechaPasada);
        } 
        
        // modificarNombre (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarNombre_DeberiaActualizarElNombre()
        {
            _proyecto = new Proyecto("nombre viejo", "Desc",  _admin, _miembros);

            _proyecto.ModificarNombre("nombre nuevo");

            Assert.AreEqual("nombre nuevo", _proyecto.Nombre);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsNull()
        {
            _proyecto = new Proyecto("Proyecto Original", "Descripción",  _admin, _miembros);

            _proyecto.ModificarNombre(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsVacio()
        {
            _proyecto = new Proyecto("Proyecto Original", "Descripción",  _admin, _miembros);

            _proyecto.ModificarNombre("");
        }
        
        // modificarDescripcion (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarDescripcion_ActualizaLaDescripcion()
        {
            _proyecto = new Proyecto("Proyecto", "Descripcion vieja",  _admin, _miembros);

            _proyecto.ModificarDescripcion("Descripcion nueva");

            Assert.AreEqual("Descripcion nueva", _proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
        {
            _proyecto = new Proyecto("Proyecto Original", "Descripción", _admin, _miembros);

            _proyecto.ModificarDescripcion(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
        {
            _proyecto = new Proyecto("Proyecto Original", "Descripción",  _admin, _miembros);

            _proyecto.ModificarDescripcion("");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionSupera400Caracteres()
        {
            _proyecto = new Proyecto("Proyecto Original", "Descripción", _admin, _miembros);

            string descripcionLarga = new string('a', 401); // 401 caracteres

            _proyecto.ModificarDescripcion(descripcionLarga);
        }
        
        // reasignar el administrador de proyecto a otro
        
        [TestMethod]
        public void AsignarNuevoAdministrador_CambiaElAdministradorDelProyecto()
        {
            
            _admin.Id = 1;
    
            Usuario nuevoAdmin = new Usuario();
            nuevoAdmin.Id = 2;
            
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);
            
            _proyecto.AsignarMiembro(nuevoAdmin);
            _proyecto.AsignarNuevoAdministrador(2);

            Assert.AreEqual(nuevoAdmin, _proyecto.Administrador);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarNuevoAdministrador_LanzaExcepcionSiIdNoCorrespondeAMiembro()
        {
            _admin.Id = 1;
    
            Usuario miembro = new Usuario();
            miembro.Id = 2; 
            
            _proyecto = new Proyecto("Proyecto 1", "Descripción", _admin, _miembros);

            _proyecto.AsignarNuevoAdministrador(2); // ID 2 no está en miembros
        }
        
        
        // DAR LISTAS:
        
        // de miembros:
        
        [TestMethod]
        public void DarListaMiembros_DevuelveListaDeMiembros()
        {
            Usuario miembro = new Usuario();
            _proyecto = new Proyecto("Proyecto", "Descripción", _admin, _miembros);
            
            _proyecto.AsignarMiembro(miembro);

            List<Usuario> lista = _proyecto.Miembros;

            Assert.AreEqual(2, lista.Count);
            Assert.IsTrue(lista.Contains(_admin));
            Assert.IsTrue(lista.Contains(miembro));
        }
        
        //de tareas:
        
        [TestMethod]
        public void DarListaTareas_DevuelveListaDeTareas()
        {
            Tarea tarea = new Tarea();
            _proyecto = new Proyecto("Proyecto", "Descripción",  _admin, _miembros);
            
            _proyecto.AgregarTarea(tarea);
            List<Tarea> lista = _proyecto.Tareas;

            Assert.AreEqual(1, lista.Count);
            Assert.IsTrue(lista.Contains(tarea));
        }
        
        //notificarMiembros
        [TestMethod]
        public void NotificarMiembros_AgregaNotificacionATodosLosMiembros()
        {
            Usuario miembro = new Usuario();
            _proyecto = new Proyecto("Proyecto", "Descripción", _admin, _miembros);
            _proyecto.AsignarMiembro(miembro);
            _proyecto.NotificarMiembros("Se modificó el proyecto.");

            foreach (Usuario usuario in _miembros)
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == "Se modificó el proyecto."));
            }
        }
        
        // notificarAdministrador
        [TestMethod]
        public void NotificarAdministrador_AgregaNotificacionAlAdministrador()
        {
            _admin.Id = 1;
            _proyecto = new Proyecto("Proyecto", "Descripción",  _admin, _miembros);

            _proyecto.NotificarAdministrador("Mensaje para admin");

            Assert.IsTrue(_admin.Notificaciones.Any(n => n.Mensaje == "Mensaje para admin"));
        }
        
        //es miembro por id
        
        [TestMethod]
        public void EsMiembro_PorId_DevuelveTrue_SiUsuarioPertenece()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario ();
            List<Usuario> miembros = new List<Usuario>{miembro};
            Proyecto proyecto = new Proyecto("Proyecto Test", "Descripción", admin, miembros);
            
            Bool resultado = proyecto.EsMiembro(2);
            
            Assert.IsTrue(resultado);
        }
        
        
    }
}
