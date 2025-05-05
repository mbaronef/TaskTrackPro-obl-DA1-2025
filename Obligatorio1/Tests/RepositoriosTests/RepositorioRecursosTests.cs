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
        _repositorioRecursos = new RepositorioRecursos();
        _recurso = new Recurso("nombre", "tipo", "descripcion");
        _adminProyecto = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
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
    
    [TestMethod]
    public void SeModificaElNombreDeRecursoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.ModificarNombre(_recurso.Id, "Nuevo");
        Recurso recurso = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual("Nuevo", recurso.Nombre);
    }
    
    [TestMethod]
    public void SeModificaElTipoDeRecursoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.ModificarTipo(_recurso.Id, "Nuevo");
        Recurso recurso = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual("Nuevo", recurso.Tipo);
    }
    
    [TestMethod]
    public void SeModificaLaDescripcionDeRecursoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.ModificarDescripcion(_recurso.Id, "Nuevo");
        Recurso recurso = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual("Nuevo", recurso.Descripcion);
    }

    [TestMethod]
    public void SeModificaProyectoAsociadoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.ModificarProyectoAsociado(_recurso.Id, _proyecto);
        Recurso recurso = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual(_proyecto, recurso.ProyectoAsociado);
    }

    [TestMethod]
    public void SeModificaTareasUsandoloOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.ModificarCantidadDeTareasUsandolo(_recurso.Id, 1000);
        Recurso recurso = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual(1000, recurso.CantidadDeTareasUsandolo);
    }
}