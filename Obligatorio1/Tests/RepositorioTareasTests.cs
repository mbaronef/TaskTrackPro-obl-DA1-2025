using Dominio.Dummies;

namespace Tests;

[TestClass]
public class RepositorioTareasTests
{
    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioTareas repositorioTareas = new RepositorioTareas();
        Tarea tarea = repositorioTareas.ObtenerPorId(1);
        Assert.IsNull(tarea);
    }
}