using Dominio;
using Repositorios;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioRecursosTests
{
    private RepositorioRecursos _repositorioRecursos;
    private Recurso _recurso;
    private Proyecto _proyecto;
    private Usuario _adminProyecto;

    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(RepositorioRecursos).GetField("_cantidadRecursos",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);

        _repositorioRecursos = new RepositorioRecursos();
        _recurso = new Recurso("nombre", "tipo", "descripcion",1);
        _recurso.IncrementarCantidadDeTareasUsandolo();
        _recurso.IncrementarCantidadDeTareasUsandolo();
        _recurso.IncrementarCantidadDeTareasUsandolo(); // cantidad de tarea usandolo = 3
        _adminProyecto = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
        _adminProyecto.EsAdministradorProyecto = true;
        List<Usuario> miembros = new List<Usuario>();
        _proyecto = new Proyecto("Proyecto", "hacer algo", DateTime.Today.AddDays(10), _adminProyecto, miembros);
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioRecursos repositorioRecursos = new RepositorioRecursos();
        Recurso recurso = repositorioRecursos.ObtenerPorId(1);
        Assert.IsNull(recurso);
    }

    [TestMethod]
    public void SeAgregaRecursooOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        Assert.AreEqual(_recurso, _repositorioRecursos.ObtenerPorId(_recurso.Id));
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        Recurso recurso2 = new Recurso("Nombre", "tipo", "desc",1);
        _repositorioRecursos.Agregar(recurso2);

        Assert.AreEqual(1, _recurso.Id);
        Assert.AreEqual(2, recurso2.Id);
    }

    [TestMethod]
    public void SeEliminaRecursoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.Eliminar(_recurso.Id);
        Assert.IsNull(_repositorioRecursos.ObtenerPorId(_recurso.Id));
    }

    [TestMethod]
    public void SeObtieneLaListaDeRecursosOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        List<Recurso> recursos = _repositorioRecursos.ObtenerTodos();
        Assert.IsNotNull(recursos);
        Assert.AreEqual(1, recursos.Count);
        Assert.AreEqual(_recurso, recursos.Last());
    }
}