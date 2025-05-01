using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioProyectosTest
{
    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioProyectos repositorioProyectos = new RepositorioProyectos();
        Proyecto proyecto = repositorioProyectos.ObtenerPorId(1);
        Assert.IsNull(proyecto);
    }

    [TestMethod]
    public void SeAgregaProyectoOk()
    {
        RepositorioProyectos repositorioProyectos = new RepositorioProyectos(); 
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");;
        List<Usuario> lista = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Proyecto", "hacer algo", usuario, lista);
        repositorioProyectos.Agregar(proyecto);
        Assert.AreEqual(proyecto, repositorioProyectos.ObtenerPorId(proyecto.Id));
    }

    [TestMethod]
    public void SeEliminaProyectoOk()
    {
        RepositorioProyectos repositorioProyectos = new RepositorioProyectos(); 
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");;
        List<Usuario> lista = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Proyecto", "hacer algo", usuario, lista);
        repositorioProyectos.Agregar(proyecto);
        repositorioProyectos.Eliminar(proyecto.Id);
        Assert.IsNull(repositorioProyectos.ObtenerPorId(proyecto.Id));
    }
}
