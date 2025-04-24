using Dominio;
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

            DateTime antes = DateTime.Now;
            Proyecto proyecto = new Proyecto("Nombre", "Descripción", null, admin, miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(proyecto.FechaInicio >= antes && proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
    }
}