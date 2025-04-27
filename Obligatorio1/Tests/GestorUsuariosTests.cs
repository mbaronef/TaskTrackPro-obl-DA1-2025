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

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        GestorUsuario gestorUsuarios = new GestorUsuario();
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuarios.AgregarUsuario(usuario);
        Assert.AreEqual(1,gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario, gestorUsuarios.Usuarios[0]);
    }

}