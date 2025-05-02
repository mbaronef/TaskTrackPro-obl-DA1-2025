using Servicios.Gestores;

namespace Tests;

[TestClass]
public class GestorRecursosTests
{
    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        GestorRecursos gestorRecursos = new GestorRecursos();
        Assert.IsNotNull(gestorRecursos);
        Assert.AreEqual(0, gestorRecursos.Recursos.Count);
    }
}