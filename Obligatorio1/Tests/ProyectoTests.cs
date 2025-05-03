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
            _admin = CrearAdmin();
            _gestorUsuarios.agregarUsuario(_admin);
            _miembros = new List<Usuario>();
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
            Usuario admin = CrearAdmin(1);
            Proyecto proyecto = CrearProyectoCon(admin);
            proyecto.AsignarId(42);
            Assert.AreEqual(42, proyecto.Id);
        }
        
        // validacion de parametros: FechaInicio y FechaFinMasTemprana
        
        [TestMethod]
        public void FechaInicio_InicializadaConFechaActualPorDefecto()
        {
            DateTime antes = DateTime.Now;
            _proyecto = CrearProyectoCon(_admin, _miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(_proyecto.FechaInicio >= antes && _proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
        [TestMethod]
        public void FechaFinMasTemprana_InicializadaConFechaActualPorDefecto()
        {
            DateTime antes = DateTime.Now;
            _proyecto = CrearProyectoCon(_admin, _miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(_proyecto.FechaFinMasTemprana >= antes && _proyecto.FechaFinMasTemprana <= despues); // porque DateTime.Now cambia
        }
        
        //AgregarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void AgregarTarea_AgregarUnaTareaALaLista()
        {
            Usuario admin = CrearAdmin();
            Proyecto proyecto = CrearProyectoCon(admin);
            Tarea tarea = CrearTarea();
            
            proyecto.AgregarTarea(tarea); 
            
            Assert.IsTrue(proyecto.Tareas.Contains(tarea));
            Assert.AreEqual(1, proyecto.Tareas.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaEsNull()
        {
            _proyecto = CrearProyectoCon(_admin);
            Tarea tarea1 = null;
            _proyecto.AgregarTarea(tarea1); 
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaYaEstaEnTareas()
        {
            Tarea tarea1 = CrearTarea();
            _proyecto = CrearProyectoCon(_admin);
            _proyecto.AgregarTarea(tarea1);
            _proyecto.AgregarTarea(tarea1);
        }
        
        //eliminarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarTarea_EliminaTareaDeLaLista()
        {
            Tarea tarea = CrearTarea(1);
            _proyecto = CrearProyectoCon(_admin, _miembros);
            _proyecto.AgregarTarea(tarea);
            _proyecto.EliminarTarea(1);

            Assert.IsFalse(_proyecto.Tareas.Any(t => t.Id == 1));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTarea_LanzaExcepcionSiTareaNoExiste()
        {
            Tarea tarea = CrearTarea(1);
            
            _proyecto = CrearProyectoCon(_admin);
            _proyecto.AgregarTarea(tarea);
            _proyecto.EliminarTarea(2); // ID que no existe
        }
        
        
        //AsignarMiembro (En GESTOR: solo admin proyecto puede)

        [TestMethod]
        public void AsignarMiembro_AgregarUsuarioALaListaDeMiembros()
        {
            _proyecto = CrearProyectoCon(_admin);

            Usuario nuevoMiembro = new Usuario();

            _proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(_proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioEsNull()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.AsignarMiembro(null); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioYaEstaEnMiembros()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.AsignarMiembro(_admin); 
        }
        
        //EliminarMiembro (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarMiembro_EliminaUsuarioCorrectamenteDeLaLista()
        {
            Usuario admin = CrearAdmin(1);
            Usuario miembro = CrearMiembro(2);
            _proyecto = CrearProyectoCon(admin); 
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
            _proyecto = CrearProyectoCon(_admin);
            
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
            
            _proyecto = CrearProyectoCon(_admin);
            
            _proyecto.AsignarMiembro(miembro);
            _proyecto.EliminarMiembro(1); // Intenta eliminar al admin
        }
        
        //EsAdministrador
        [TestMethod]
        public void EsAdministrador_RetornaTrueSiUsuarioEsAdministrador()
        {
            _proyecto = CrearProyectoCon(_admin, _miembros);

            bool resultado = _proyecto.EsAdministrador(_admin);

            Assert.IsTrue(resultado);
        }
        
        [TestMethod]
        public void EsAdministrador_RetornarFalseSiUsuarioNoEsAdministrador()
        {
            Usuario otro = new Usuario();
            _proyecto = CrearProyectoCon(_admin, _miembros);
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
            Proyecto _proyecto = CrearProyectoCon(_admin, _miembros);
            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _proyecto.ModificarFechaInicio(nuevaFecha);

            Assert.AreEqual(nuevaFecha, _proyecto.FechaInicio);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaInicio_LanzaExcepcionSiFechaEsAnteriorAHoy() 
        {
            _proyecto = CrearProyectoCon(_admin);

            DateTime fechaPasada = DateTime.Now.AddDays(-1);

            _proyecto.ModificarFechaInicio(fechaPasada);
        } 
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaInicio_LanzaExcepcionSiEsPosteriorALaFechaDeInicioDeUnaTarea()
        {
            Proyecto _proyecto = CrearProyectoCon(_admin, _miembros);
            Tarea tarea = CrearTarea(1, new DateTime(2026, 1, 1));
            _proyecto.AgregarTarea(tarea);

            _proyecto.ModificarFechaInicio(new DateTime(2027, 1, 1));
        }
        
        //modificar fecha de fin mas temprana
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaFinMasTemprana_LanzaExcepcionSiEsAnteriorALaFechaInicio()
        {
            _proyecto = CrearProyectoCon(_admin);
            DateTime nuevaFechaFin = _proyecto.FechaInicio.AddDays(-1);
            _proyecto.ModificarFechaFinMasTemprana(nuevaFechaFin);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaFinMasTemprana_LanzaExcepcionSiEsMenorALaFechaFinDeUnaTarea()
        {
            _proyecto = CrearProyectoCon(_admin);

            Tarea tarea = CrearTarea(1, DateTime.Today, new DateTime(2026, 6, 1));
            _proyecto.AgregarTarea(tarea);

            _proyecto.ModificarFechaFinMasTemprana(new DateTime(2026, 5, 1));
        }
        
        [TestMethod]
        public void ModificarFechaFinMasTemprana_ActualizaCorrectamente()
        {
            _proyecto = CrearProyectoCon(_admin);
            DateTime fecha = _proyecto.FechaInicio.AddDays(10);

            _proyecto.ModificarFechaFinMasTemprana(fecha);

            Assert.AreEqual(fecha, _proyecto.FechaFinMasTemprana);
        }
        
        // modificationNombre (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarNombre_DeberiaActualizarElNombre()
        {
            _proyecto = CrearProyectoCon(_admin, _miembros);
            
            _proyecto.ModificarNombre("nombre nuevo");

            Assert.AreEqual("nombre nuevo", _proyecto.Nombre);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsNull()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.ModificarNombre(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsVacio()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.ModificarNombre("");
        }
        
        // modificarDescripcion (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarDescripcion_ActualizaLaDescripcion()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.ModificarDescripcion("Descripcion nueva");

            Assert.AreEqual("Descripcion nueva", _proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
        {
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.ModificarDescripcion(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
        {
            _proyecto = CrearProyectoCon(_admin);
            
            _proyecto.ModificarDescripcion("");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionSupera400Caracteres()
        {
            _proyecto = CrearProyectoCon(_admin);

            string descripcionLarga = new string('a', 401); // 401 caracteres

            _proyecto.ModificarDescripcion(descripcionLarga);
        }
        
        // reasignar el administrador de proyecto a otro
        
        [TestMethod]
        public void AsignarNuevoAdministrador_CambiaElAdministradorDelProyecto()
        {
            
            Usuario admin = CrearAdmin(1);
            Usuario nuevoAdmin = CrearMiembro(2);
            
            _proyecto = CrearProyectoCon(admin);
            
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
            
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.AsignarNuevoAdministrador(2); // ID 2 no está en miembros
        }
        
        //notificarMiembros
        [TestMethod]
        public void NotificarMiembros_AgregaNotificacionATodosLosMiembros()
        {
            Usuario miembro = new Usuario();
            _proyecto = CrearProyectoCon(_admin);
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
            _proyecto = CrearProyectoCon(_admin);

            _proyecto.NotificarAdministrador("Mensaje para admin");

            Assert.IsTrue(_admin.Notificaciones.Any(n => n.Mensaje == "Mensaje para admin"));
        }
        
        //es miembro por id
        
        [TestMethod]
        public void EsMiembro_PorId_DevuelveTrueSiUsuarioPertenece()
        {
            Usuario admin = CrearAdmin();
            Usuario miembro = CrearMiembro(2);
            Proyecto proyecto = CrearProyectoCon(admin, new List<Usuario>{miembro});    
            
            bool resultado = proyecto.EsMiembro(2);
    
            Assert.IsTrue(resultado);
        }
        
        [TestMethod]
        public void EsMiembro_PorId_DevuelveFalseSiUsuarioNoPertenece()
        {
            Usuario admin = CrearAdmin();
            Proyecto proyecto = CrearProyectoCon(admin);
            Assert.IsFalse(proyecto.EsMiembro(100));
        }
        
        [TestMethod]
        public void EsMiembro_PorObjeto_DevuelveTrueSiUsuarioPertenece()
        {
            Usuario admin = CrearAdmin();
            Usuario miembro = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(admin, new List<Usuario> { miembro });

            Assert.IsTrue(proyecto.EsMiembro(miembro));
        }
        
        [TestMethod]
        public void EsMiembro_PorObjeto_DevuelveFalseSiUsuarioNoPertenece()
        {
            Usuario otro = new Usuario();
            Usuario admin = CrearAdmin();
            Usuario miembro = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(admin, new List<Usuario> { miembro });

            Assert.IsFalse(proyecto.EsMiembro(otro));
        }
        
        //HELPERS
        
        private Usuario CrearAdmin(int id = 1)
        {
            return new Usuario { Id = id, EsAdministradorProyecto = true };
        }

        private Usuario CrearMiembro(int id = 2)
        {
            return new Usuario { Id = id };
        }

        private Proyecto CrearProyectoCon(Usuario admin, List<Usuario> miembros = null)
        {
            miembros ??= new List<Usuario>();
            return new Proyecto("Proyecto", "Descripción", admin, miembros);
        }

        private Tarea CrearTarea(int id = 1, DateTime? inicio = null, DateTime? fin = null)
        {
            return new Tarea
            {
                Id = id,
                FechaInicio = inicio ?? DateTime.Today,
                FechaFinMasTemprana = fin ?? DateTime.Today.AddDays(1)
            };
        }
        
    }
}
