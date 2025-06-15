namespace Tests.DominioTests;

[TestClass]
public class RangoEnUsoTests
{
    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RangoEnUso rango = new RangoEnUso();
        Assert.IsNotNull(rango);
    }
}