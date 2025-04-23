namespace Tests;

[TestClass]
public class RolTests
{
    [TestMethod]
    public void Constructor()
    {
        TipoRol tipo = TipoRol.AdminSistema;
        Rol rol = new Rol(tipo);
        Assert.AreEqual(tipo, rol.Tipo);
    }
}