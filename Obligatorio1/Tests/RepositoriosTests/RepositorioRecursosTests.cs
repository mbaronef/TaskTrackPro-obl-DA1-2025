using Dominio;
using Repositorios;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioRecursosTests
{
    private RepositorioRecursos _repositorioRecursos;
    private Recurso _recurso;
    
    [TestInitialize]
    public void SetUp()
    {
        _repositorioRecursos = new RepositorioRecursos();
        _recurso = new Recurso("nombre", "tipo", "descripcion");
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
}