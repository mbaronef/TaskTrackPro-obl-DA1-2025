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
    
}
