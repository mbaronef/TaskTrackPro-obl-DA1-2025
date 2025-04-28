using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class GestorProyectosTests
    {
        //crearProyecto
        [TestMethod]
        public void CrearProyecto_AsignarIdCorrectamente()
        {
            Usuario adminSistema = new Usuario();
            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario>();
            Proyecto proyecto = new Proyecto("nombre", "descripcion", adminSistema, miembros);

            gestor.CrearProyecto(proyecto, adminSistema);

            Assert.AreEqual(1, proyecto.Id);
            Assert.AreEqual(proyecto, gestor.Proyectos[0]);
        }
    }
}