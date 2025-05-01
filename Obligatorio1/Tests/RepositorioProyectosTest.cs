using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioProyectosTest
{
    private RepositorioProyectos _repositorioProyectos;
    private Usuario _usuario;
    private Proyecto _proyecto;
    
    [TestInitialize]
    public void SetUp()
    {
        _repositorioProyectos = new RepositorioProyectos(); 
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
        List<Usuario> lista = new List<Usuario>();
        _proyecto = new Proyecto("Proyecto", "hacer algo", _usuario, lista);
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
        _repositorioProyectos.Agregar(_proyecto);
        Assert.AreEqual(_proyecto, _repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeEliminaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.Eliminar(_proyecto.Id);
        Assert.IsNull(_repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeObtieneLaListaDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        List<Proyecto> proyectos = _repositorioProyectos.ObtenerTodos();
        Assert.IsNotNull(proyectos);
        Assert.AreEqual(1, proyectos.Count);
        Assert.AreEqual(_proyecto, proyectos.Last());
    }

    [TestMethod]
    public void SeModificaElNombreDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarNombre(_proyecto.Id, "Nuevo");
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual("Nuevo", proyecto.Nombre);
    }
    
    [TestMethod]
    public void SeModificaLaDescripcionDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarDescripcion(_proyecto.Id, "Nuevo");
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual("Nuevo", proyecto.Descripcion);
    }
}
