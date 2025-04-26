using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class ProyectoTests
    {
        
        //Constructor
        [TestMethod]
        public void ConstructorConParametrosAsignadosCorrectamente()
        {
            string nombre = "Proyecto 1";
            string descripcion = "Descripción";
            List<Tarea> tareas = new List<Tarea>();
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin }; // debe agregar al admin a la lista de miembros
            
            Proyecto proyecto = new Proyecto (nombre, descripcion, tareas, admin, miembros);
            
            Assert.AreEqual(nombre, proyecto.Nombre);
            Assert.AreEqual(descripcion, proyecto.Descripcion);
            Assert.AreEqual(tareas, proyecto.Tareas);
            Assert.AreEqual(admin, proyecto.Administrador);
            Assert.AreEqual(miembros, proyecto.Miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_DeberiaLanzarExcepcionSiDescripcionSupera400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 401); 
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);
        }
        
        [TestMethod]
        public void Constructor_DeberiaPermitirDescripcionConMenosDe400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 399);
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        [TestMethod]
        public void Constructor_DeberiaPermitirDescripcionDeHasta400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 400); // 400 caracteres exactos
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsVacio()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("", "Descripción válida", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiNombreEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto(null, "Descripción válida", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre válido", "", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiDescripcionEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();
            
            Proyecto proyecto = new Proyecto("Nombre válido", null, tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiAdministradorEsNull()
        {
            List<Usuario> miembros = new List<Usuario> { new Usuario() };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, null, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiMiembrosEsNull()
        {
            Usuario admin = new Usuario();
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_LanzaExcepcionSiMiembrosNoContieneAdministrador() // este no se si alguna vez pasa porque lo pusimos que cuando se crea lo agrega
        {
            Usuario admin = new Usuario();
            Usuario otro = new Usuario();
            List<Usuario> miembros = new List<Usuario> { otro };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);
        }
        
        //Costructor con id
        
        [TestMethod]
        public void ConstructorConId_DeberiaAsignarCorrectamenteElId()
        {
            Usuario admin = new Usuario();
            admin.Id = 1;
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();
    
            Proyecto proyecto = new Proyecto(42, "Proyecto Test", "Descripción de prueba", tareas, admin, miembros);

            Assert.AreEqual(42, proyecto.Id);
        }
        
        // validacion de parametros: FechaInicio y FechaFinMasTemprana
        
        [TestMethod]
        public void FechaInicio_InicializadaConFechaActualPorDefecto()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            DateTime antes = DateTime.Now;
            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(proyecto.FechaInicio >= antes && proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
        [TestMethod]
        public void FechaFinMasTemprana_InicializadaConMinValuePorDefecto()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            var proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);

            Assert.AreEqual(DateTime.MinValue, proyecto.FechaFinMasTemprana);
        }
        
        //AgregarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void AgregarTarea_AgregarUnaTareaALaLista()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            Tarea tarea1 = new Tarea();
            
            proyecto.AgregarTarea(tarea1); 
            
            Assert.IsTrue(proyecto.Tareas.Contains(tarea1));
            Assert.AreEqual(1, proyecto.Tareas.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.AgregarTarea(null); 
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_LanzarExcepcionSiTareaYaEstaEnTareas()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Tarea  tarea1 = new Tarea();
            List<Tarea> tareas = new List<Tarea> { tarea1 };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción",  tareas, admin, miembros);
            
            proyecto.AgregarTarea(tarea1);
        }
        
        //eliminarTarea (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarTarea_EliminaTareaDeLaLista()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
    
            Tarea tarea = new Tarea();
            tarea.Id = 1;
    
            List<Tarea> tareas = new List<Tarea> { tarea };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.EliminarTarea(1);

            Assert.IsFalse(proyecto.Tareas.Any(t => t.Id == 1));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTarea_LanzaExcepcionSiTareaNoExiste()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
    
            Tarea tarea = new Tarea();
            tarea.Id = 1;
    
            List<Tarea> tareas = new List<Tarea> { tarea };
            Proyecto proyecto = new Proyecto("Proyecto", "Descripción", tareas, admin, miembros);

            proyecto.EliminarTarea(2); // ID que no existe
        }
        
        
        //AsignarMiembro (En GESTOR: solo admin proyecto puede)

        [TestMethod]
        public void AsignarMiembro_AgregarUsuarioALaListaDeMiembros()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            Usuario nuevoMiembro = new Usuario();

            proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.AsignarMiembro(null); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_LanzarExcepcionSiUsuarioYaEstaEnMiembros()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            proyecto.AsignarMiembro(admin); 
        }
        
        //EliminarMiembro (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void EliminarMiembro_EliminaUsuarioCorrectamenteDeLaLista()
        {
            Usuario admin = new Usuario();
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;
    
            List<Usuario> miembros = new List<Usuario> { admin, miembro };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.EliminarMiembro(2); // Elimino al miembro

            Assert.IsFalse(proyecto.Miembros.Any(u => u.Id == 2));
            Assert.AreEqual(1, proyecto.Miembros.Count);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzarExcepcionSiElUsuarioNoExisteEnMiembros()
        {
            Usuario admin = new Usuario();
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;

            List<Usuario> miembros = new List<Usuario> { admin, miembro };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.EliminarMiembro(3); // ID que no existe
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembro_LanzarExcepcionSiUsuarioEsAdministrador()
        {
            Usuario admin = new Usuario();
            admin.Id = 1;
            Usuario miembro = new Usuario();
            miembro.Id = 2;

            List<Usuario> miembros = new List<Usuario> { admin, miembro };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.EliminarMiembro(1); // Intenta eliminar al admin
        }
        
        //EsAdministrador
        [TestMethod]
        public void EsAdministrador_RetornaTrueSiUsuarioEsAdministrador()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            bool resultado = proyecto.EsAdministrador(admin);

            Assert.IsTrue(resultado);
        }
        
        [TestMethod]
        public void EsAdministrador_RetornarFalseSiUsuarioNoEsAdministrador()
        {
            Usuario admin = new Usuario();
            Usuario otro = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin, otro };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            bool resultado = proyecto.EsAdministrador(otro);

            Assert.IsFalse(resultado);
        }
        
        //MODIFICACIONES
        
        //modificarFechaDeInicio (EN GESTOR: puede ser modificada por admin sistema o por admin proyecto)
        
        [TestMethod]
        public void ModificarFechaInicio_ActualizaLaFechaOK()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            DateTime nuevaFecha = new DateTime(2025, 5, 1);
            proyecto.ModificarFechaInicio(nuevaFecha);

            Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaInicio_LanzaExcepcionSiFechaEsAnteriorAHoy() // capaz no estaria tan bien (?) Podria ser un error al ingresar los datos
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto", "Descripción", tareas, admin, miembros);

            DateTime fechaPasada = DateTime.Now.AddDays(-1);

            proyecto.ModificarFechaInicio(fechaPasada);
        } 
        
        // modificarNombre (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarNombre_DeberiaActualizarElNombre()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("nombre viejo", "Desc", tareas, admin, miembros);

            proyecto.ModificarNombre("nombre nuevo");

            Assert.AreEqual("nombre nuevo", proyecto.Nombre);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto Original", "Descripción", tareas, admin, miembros);

            proyecto.ModificarNombre(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombre_LanzaExcepcionSiNombreEsVacio()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto Original", "Descripción", tareas, admin, miembros);

            proyecto.ModificarNombre("");
        }
        
        // modificarDescripcion (En GESTOR: solo admin proyecto puede)
        
        [TestMethod]
        public void ModificarDescripcion_ActualizaLaDescripcion()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto", "Descripcion vieja", tareas, admin, miembros);

            proyecto.ModificarDescripcion("Descripcion nueva");

            Assert.AreEqual("Descripcion nueva", proyecto.Descripcion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_DeberiaLanzarExcepcion_SiDescripcionEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto Original", "Descripción", tareas, admin, miembros);

            proyecto.ModificarDescripcion(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_DeberiaLanzarExcepcion_SiDescripcionEsVacia()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto Original", "Descripción", tareas, admin, miembros);

            proyecto.ModificarDescripcion("");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcion_DeberiaLanzarExcepcion_SiDescripcionSupera400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto Original", "Descripción", new List<Tarea>(), admin, miembros);

            string descripcionLarga = new string('a', 401); // 401 caracteres

            proyecto.ModificarDescripcion(descripcionLarga);
        }
        
        
        // falta:
        // agregar admin siempre no lo deberia hacer solo???
        
        // metodos faltantes:
        // calcularRutaCritica()
        
        // darRecursosFaltantes()
        // que recorra la lista de tareas y se fije en cada una de ellas cuales son los recursos necesarios
        
        // modificaciones:
        // modificar el admin del proyecto (solo admin de sistema puede cambiarlo)
        
        // fecaha de inicio de las tareas campo necesario (si no depende de ninguna)
        // recursos solo se pueden dar de baja si no estan en uso
        // usuario debe ser mayor de edad
        // notificarMiembros(string mensaje)
        // notificarAdministradores (string mensaje)
        // falta eliminar tarea que no esta en UML !!!!!!!
        
    }
}