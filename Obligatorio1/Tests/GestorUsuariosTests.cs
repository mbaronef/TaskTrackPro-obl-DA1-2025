namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    [TestMethod]
    public void ConstructorSinParametrosCreaGestorValido()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Assert.IsNotNull(gestorUsuarios);
        Assert.AreEqual(0,gestorUsuarios.Usuarios.Count);
        Assert.AreEqual(0,gestorUsuarios.AdministradoresSistema.Count);
    }

}