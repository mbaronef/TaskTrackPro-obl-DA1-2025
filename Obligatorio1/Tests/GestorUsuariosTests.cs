using Dominio;

namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    [TestMethod]
    public void ConstructorSinParametrosCreaGestorValido()
    {
        GestorUsuario gestorUsuarios = new GestorUsuario();
        Assert.IsNotNull(gestorUsuarios);
        Assert.AreEqual(0,gestorUsuarios.Usuarios.Count);
        Assert.AreEqual(0,gestorUsuarios.AdministradoresSistema.Count);
    }

}