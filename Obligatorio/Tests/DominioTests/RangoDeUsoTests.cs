using Dominio;

namespace Tests.DominioTests;

[TestClass]
public class RangoDeUsoTests
{
    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RangoDeUso rango = new RangoDeUso();
        Assert.IsNotNull(rango);
    }
}