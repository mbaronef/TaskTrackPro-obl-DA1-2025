using Dominio;
using Repositorios;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioRecursosTests
{
    private RepositorioRecursos _repositorioRecursos;
    private Recurso _recurso;
    
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
}