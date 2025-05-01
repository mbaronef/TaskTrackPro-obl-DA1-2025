using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioProyectosTest
{
    private RepositorioProyectos _repositorioProyectos;
    private Usuario _usuario;
    
    [TestInitialize]
    public void setUp()
    {
        _repositorioProyectos = new RepositorioProyectos(); 
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");;

    }

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
        List<Usuario> lista = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Proyecto", "hacer algo", _usuario, lista);
        _repositorioProyectos.Agregar(proyecto);
        Assert.AreEqual(proyecto, _repositorioProyectos.ObtenerPorId(proyecto.Id));
    }

    [TestMethod]
    public void SeEliminaProyectoOk()
    {
        List<Usuario> lista = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Proyecto", "hacer algo", _usuario, lista);
        _repositorioProyectos.Agregar(proyecto);
        _repositorioProyectos.Eliminar(proyecto.Id);
        Assert.IsNull(_repositorioProyectos.ObtenerPorId(proyecto.Id));
    }
}
