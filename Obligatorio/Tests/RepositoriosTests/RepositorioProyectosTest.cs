using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorios;
using Tests.Contexto;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioProyectosTest
{
    private RepositorioProyectos _repositorioProyectos;
    private SqlContext _contexto;
    private Usuario _usuario;
    private Proyecto _proyecto;

    [TestInitialize]
    public void SetUp()
    {
        _contexto = SqlContextFactory.CrearContextoEnMemoria();

        _repositorioProyectos = new RepositorioProyectos(_contexto);

        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
        _usuario.EsAdministradorProyecto = true;

        List<Usuario> miembros = new();
        _proyecto = new Proyecto("Proyecto", "hacer algo", DateTime.Today.AddDays(10), _usuario, miembros);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(1);
        Assert.IsNull(proyecto);
    }

    [TestMethod]
    public void SeAgregaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Assert.AreEqual(_proyecto, _repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Proyecto proyecto2 = new Proyecto("Proyecto2", "hacer algo2", DateTime.Today.AddDays(10), _usuario, new List<Usuario>());
        _repositorioProyectos.Agregar(proyecto2);

        Assert.AreEqual(1, _proyecto.Id);
        Assert.AreEqual(2, proyecto2.Id);
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
        var proyectos = _repositorioProyectos.ObtenerTodos();
        Assert.IsNotNull(proyectos);
        Assert.AreEqual(1, proyectos.Count);
        Assert.AreEqual(_proyecto, proyectos.Last());
    }
}
