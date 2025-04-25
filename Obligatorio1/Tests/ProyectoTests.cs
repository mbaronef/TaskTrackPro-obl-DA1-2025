using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class ProyectoTests
    {
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
        public void FechaInicioInicializadaConFechaActual()
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
        public void FechaFinMasTempranaInicializadaConMinValue()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            var proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);

            Assert.AreEqual(DateTime.MinValue, proyecto.FechaFinMasTemprana);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiNombreEsVacio()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };

            Proyecto proyecto = new Proyecto("", "Descripción válida", new List<Tarea>(), admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiNombreEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };

            Proyecto proyecto = new Proyecto(null, "Descripción válida", new List<Tarea>(), admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiDescripcionEsVacia()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };

            Proyecto proyecto = new Proyecto("Nombre válido", "", new List<Tarea>(), admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiDescripcionEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            
            Proyecto proyecto = new Proyecto("Nombre válido", null, new List<Tarea>(), admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiAdministradorEsNull()
        {
            List<Usuario> miembros = new List<Usuario> { new Usuario() };

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", new List<Tarea>(), null, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiMiembrosEsNull()
        {
            Usuario admin = new Usuario();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", new List<Tarea>(), admin, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiMiembrosNoContieneAdministrador() // este no se si alguna vez pasa porque lo pusimos que cuando se crea lo agrega
        {
            Usuario admin = new Usuario();
            Usuario otro = new Usuario();
            List<Usuario> miembros = new List<Usuario> { otro };

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", new List<Tarea>(), admin, miembros);
        }
        
        [TestMethod]
        public void AgregarTarea_DeberiaAgregarUnaTareaALaLista()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            Tarea tarea1 = new Tarea();
            
            proyecto.AgregarTarea(tarea1); 
            
            Assert.IsTrue(proyecto.Tareas.Contains(tarea1));
            Assert.AreEqual(1, proyecto.Tareas.Count);
        }
        
        [TestMethod]
        public void AsignarMiembro_DeberiaAgregarUsuarioALaListaDeMiembros()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            Usuario nuevoMiembro = new Usuario();

            proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_DeberiaLanzarExcepcionSiUsuarioEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            proyecto.AsignarMiembro(null); 
        }
        
        
        
        
    }
}