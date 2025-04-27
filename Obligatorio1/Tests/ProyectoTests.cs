using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class ProyectoTests
    {
        private Proyecto proyecto;
        private Usuario admin;
        private List<Usuario> miembros;
        
        [TestInitialize]
        public void AntesDeCadaTest()
        {
            admin = new Usuario();
            miembros = new List<Usuario>(); // SIN el "List<Usuario>" adelante
        }
        
        //Constructor
        [TestMethod]
        public void Constructor_ConParametrosAsignadosCorrectamente()
        {
            string nombre = "Proyecto 1";
            string descripcion = "Descripción";
            
            proyecto = new Proyecto (nombre, descripcion, admin, miembros);
            
            Assert.AreEqual(nombre, proyecto.Nombre);
            Assert.AreEqual(descripcion, proyecto.Descripcion);
            Assert.AreEqual(admin, proyecto.Administrador);
            Assert.AreEqual(miembros, proyecto.Miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionSupera400Caracteres()
        {
            string descripcion = new string('a', 401); 

            proyecto = new Proyecto("Proyecto", descripcion, admin, miembros);
        }
        
        [TestMethod]
        public void Constructor_PermiteDescripcionConMenosDe400Caracteres()
        {
            string descripcion = new string('a', 399);

            proyecto = new Proyecto("Proyecto", descripcion, admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        [TestMethod]
        public void Constructor_PermiteDescripcionDeHasta400Caracteres()
        {
            string descripcion = new string('a', 400); // 400 caracteres exactos
            
            proyecto = new Proyecto("Proyecto", descripcion,  admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsVacio()
        { 
            proyecto = new Proyecto("", "Descripción válida",  admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsNull()
        {
            proyecto = new Proyecto(null, "Descripción válida",  admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
        { 
            proyecto = new Proyecto("Nombre válido", "",  admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsNull()
        {
            proyecto = new Proyecto("Nombre válido", null,  admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiAdministradorEsNull()
        {
            proyecto = new Proyecto("Nombre", "Descripción",  null, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiMiembrosEsNull()
        {
            proyecto = new Proyecto("Nombre", "Descripción",  admin, null);
        }
        
        //Costructor con id
        
        [TestMethod]
        public void ConstructorConId_DeberiaAsignarCorrectamenteElId()
        {
            admin.Id = 1;
            proyecto = new Proyecto(42, "Proyecto Test", "Descripción de prueba", admin, miembros);

            Assert.AreEqual(42, proyecto.Id);
        }
        
        // validacion de parametros: FechaInicio y FechaFinMasTemprana
        
        [TestMethod]
        public void FechaInicio_InicializadaConFechaActualPorDefecto()
        {
            DateTime antes = DateTime.Now;
            proyecto = new Proyecto("Nombre", "Descripción", admin, miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(proyecto.FechaInicio >= antes && proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
        [TestMethod]
        public void FechaFinMasTemprana_InicializadaConMinValuePorDefecto()
        { 
            proyecto = new Proyecto("Nombre", "Descripción", admin, miembros);
            
            Assert.AreEqual(DateTime.MinValue, proyecto.FechaFinMasTemprana);
        }
        
        //AgregarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void AgregarTarea_AgregarUnaTareaALaLista()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);

            Tarea tarea1 = new Tarea();
            
            proyecto.AgregarTarea(tarea1); 
            
            Assert.IsTrue(proyecto.Tareas.Contains(tarea1));
            Assert.AreEqual(1, proyecto.Tareas.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaEsNull()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción",  admin, miembros);
            Tarea tarea1 = null;
            proyecto.AgregarTarea(tarea1); 
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaYaEstaEnTareas()
        {
            Tarea tarea1 = new Tarea(); 
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            proyecto.AgregarTarea(tarea1);
            proyecto.AgregarTarea(tarea1);
        }
        
        //eliminarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarTarea_EliminaTareaDeLaLista()
        {
            Tarea tarea = new Tarea();
            tarea.Id = 1;
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            proyecto.AgregarTarea(tarea);
            proyecto.EliminarTarea(1);

            Assert.IsFalse(proyecto.Tareas.Any(t => t.Id == 1));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTarea_LanzaExcepcionSiTareaNoExiste()
        {
            Tarea tarea = new Tarea();
            tarea.Id = 1;
            
            proyecto = new Proyecto("Proyecto", "Descripción", admin, miembros);
            proyecto.AgregarTarea(tarea);
            proyecto.EliminarTarea(2); // ID que no existe
        }
        
        
        //AsignarMiembro (En GESTOR: solo admin proyecto puede)

        [TestMethod]
        public void AsignarMiembro_AgregarUsuarioALaListaDeMiembros()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción",  admin, miembros);

            Usuario nuevoMiembro = new Usuario();

            proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioEsNull()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción",  admin, miembros);

            proyecto.AsignarMiembro(null); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioYaEstaEnMiembros()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción",  admin, miembros);

            proyecto.AsignarMiembro(admin); 
        }
        
        //EliminarMiembro (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarMiembro_EliminaUsuarioCorrectamenteDeLaLista()
        {
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            proyecto.AsignarMiembro(miembro);
            proyecto.EliminarMiembro(2); // Elimino al miembro

            Assert.IsFalse(proyecto.Miembros.Any(u => u.Id == 2));
            Assert.AreEqual(1, proyecto.Miembros.Count);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzaExcepcionSiElUsuarioNoExisteEnMiembros()
        {
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            
            proyecto.AsignarMiembro(miembro);
            proyecto.EliminarMiembro(3); // ID que no existe
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzaExcepcionSiUsuarioEsAdministrador()
        {
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
            
            proyecto = new Proyecto("Proyecto 1", "Descripción",  admin, miembros);
            
            proyecto.AsignarMiembro(miembro);
            proyecto.EliminarMiembro(1); // Intenta eliminar al admin
        }
        
        //EsAdministrador
        [TestMethod]
        public void EsAdministrador_RetornaTrueSiUsuarioEsAdministrador()
        {
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);

            bool resultado = proyecto.EsAdministrador(admin);

            Assert.IsTrue(resultado);
        }
        
        [TestMethod]
        public void EsAdministrador_RetornarFalseSiUsuarioNoEsAdministrador()
        {
            Usuario otro = new Usuario();
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            
            proyecto.AsignarMiembro(otro);
            bool resultado = proyecto.EsAdministrador(otro);

            Assert.IsFalse(resultado);
        }
        
        //MODIFICACIONES
        
        //modificarFechaDeInicio (EN GESTOR: puede ser modificada por admin sistema o por admin proyecto)
        
        [TestMethod]
        public void ModificarFechaInicio_ActualizaLaFechaOK()
        { 
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);

            DateTime nuevaFecha = new DateTime(2025, 5, 1);
            proyecto.ModificarFechaInicio(nuevaFecha);

            Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaInicio_LanzaExcepcionSiFechaEsAnteriorAHoy() // capaz no estaria tan bien (?) Podria ser un error al ingresar los datos
        {
            proyecto = new Proyecto("Proyecto", "Descripción",  admin, miembros);

            DateTime fechaPasada = DateTime.Now.AddDays(-1);

            proyecto.ModificarFechaInicio(fechaPasada);
        } 
        
        // modificarNombre (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarNombre_DeberiaActualizarElNombre()
        {
            proyecto = new Proyecto("nombre viejo", "Desc",  admin, miembros);

            proyecto.ModificarNombre("nombre nuevo");

            Assert.AreEqual("nombre nuevo", proyecto.Nombre);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsNull()
        {
            proyecto = new Proyecto("Proyecto Original", "Descripción",  admin, miembros);

            proyecto.ModificarNombre(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsVacio()
        {
            proyecto = new Proyecto("Proyecto Original", "Descripción",  admin, miembros);

            proyecto.ModificarNombre("");
        }
        
        // modificarDescripcion (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarDescripcion_ActualizaLaDescripcion()
        {
            proyecto = new Proyecto("Proyecto", "Descripcion vieja",  admin, miembros);

            proyecto.ModificarDescripcion("Descripcion nueva");

            Assert.AreEqual("Descripcion nueva", proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
        {
            proyecto = new Proyecto("Proyecto Original", "Descripción", admin, miembros);

            proyecto.ModificarDescripcion(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
        {
            proyecto = new Proyecto("Proyecto Original", "Descripción",  admin, miembros);

            proyecto.ModificarDescripcion("");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_LanzaExcepcionSiDescripcionSupera400Caracteres()
        {
            proyecto = new Proyecto("Proyecto Original", "Descripción", admin, miembros);

            string descripcionLarga = new string('a', 401); // 401 caracteres

            proyecto.ModificarDescripcion(descripcionLarga);
        }
        
        // reasignar el administrador de proyecto a otro
        
        [TestMethod]
        public void AsignarNuevoAdministrador_CambiaElAdministradorDelProyecto()
        {
            
            admin.Id = 1;
    
            Usuario nuevoAdmin = new Usuario();
            nuevoAdmin.Id = 2;
            
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);
            
            proyecto.AsignarMiembro(nuevoAdmin);
            proyecto.AsignarNuevoAdministrador(2);

            Assert.AreEqual(nuevoAdmin, proyecto.Administrador);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarNuevoAdministrador_LanzaExcepcionSiIdNoCorrespondeAMiembro()
        {
            admin.Id = 1;
    
            Usuario miembro = new Usuario();
            miembro.Id = 2; 
            
            proyecto = new Proyecto("Proyecto 1", "Descripción", admin, miembros);

            proyecto.AsignarNuevoAdministrador(2); // ID 2 no está en miembros
        }
        
        
        // DAR LISTAS:
        
        // de miembros:
        
        [TestMethod]
        public void DarListaMiembros_DevuelveListaDeMiembros()
        {
            Usuario miembro = new Usuario();
            proyecto = new Proyecto("Proyecto", "Descripción", admin, miembros);
            
            proyecto.AsignarMiembro(miembro);

            List<Usuario> lista = proyecto.DarListaMiembros();

            Assert.AreEqual(2, lista.Count);
            Assert.IsTrue(lista.Contains(admin));
            Assert.IsTrue(lista.Contains(miembro));
        }
        
        //de tareas:
        
        [TestMethod]
        public void DarListaTareas_DevuelveListaDeTareas()
        {
            Tarea tarea = new Tarea();
            proyecto = new Proyecto("Proyecto", "Descripción",  admin, miembros);
            
            proyecto.AgregarTarea(tarea);
            List<Tarea> lista = proyecto.DarListaTareas();

            Assert.AreEqual(1, lista.Count);
            Assert.IsTrue(lista.Contains(tarea));
        }
        
        //NOTIFICACIONES (habria que validar si el mensaje es null???)
        //(GESTOR: se encarga de cuando haya una modificacion mandar una notificacion)
        
        //notificarMiembros
        [TestMethod]
        public void NotificarMiembros_AgregaNotificacionATodosLosMiembros()
        {
            Usuario miembro = new Usuario();
            proyecto = new Proyecto("Proyecto", "Descripción", admin, miembros);
            proyecto.AsignarMiembro(miembro);
            proyecto.NotificarMiembros("Se modificó el proyecto.");

            foreach (Usuario usuario in miembros)
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == "Se modificó el proyecto."));
            }
        }
        
        // notificarAdministrador
        [TestMethod]
        public void NotificarAdministrador_AgregaNotificacionAlAdministrador()
        {
            admin.Id = 1;
            proyecto = new Proyecto("Proyecto", "Descripción",  admin, miembros);

            proyecto.NotificarAdministrador("Mensaje para admin");

            Assert.IsTrue(admin.Notificaciones.Any(n => n.Mensaje == "Mensaje para admin"));
        }
        
        //darRecursosFaltantes
        
        [TestMethod]
        public void DarRecursosFaltantes_DevuelveRecursosNoEnUso()
        {
            Recurso recurso1 = new Recurso { Id = 1, EnUso = false };
            Recurso recurso2 = new Recurso { Id = 2, EnUso = true };

            Tarea tarea = new Tarea();
            tarea.RecursosNecesarios = new List<Recurso> { recurso1, recurso2 };
            
            proyecto = new Proyecto("Proyecto", "Descripción", admin, miembros);
            
            proyecto.AgregarTarea(tarea);
            List<Recurso> faltantes = proyecto.DarRecursosFaltantes();

            Assert.AreEqual(1, faltantes.Count);
            Assert.IsTrue(faltantes.Any(r => r.Id == 1));
        }
        
        // falta:
        // REFACTOR (?)
        // validaciones de recursos(?? Esperar a que definamos bien recurso)
        
        // metodos faltantes:
        // calcularRutaCritica()
        
    }
}