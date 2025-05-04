using Dominio;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioRecursosTests
{
    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioRecursos repositorioRecursos = new RepositorioRecursos();
        Recurso recurso = repositorioRecursos.ObtenerPorId(1);
        Assert.IsNull(recurso);
    }
}