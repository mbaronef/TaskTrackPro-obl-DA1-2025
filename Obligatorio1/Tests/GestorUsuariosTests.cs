using Dominio;

namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    [TestMethod]
    public void ConstructorSinParametrosCreaGestorValido()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Assert.IsNotNull(gestorUsuarios);
        Assert.AreEqual(0, gestorUsuarios.Usuarios.Count);
    }

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario2);

        Assert.AreEqual(2, gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario1, gestorUsuarios.Usuarios[0]);
        Assert.AreSame(usuario2, gestorUsuarios.Usuarios[1]);
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIds()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario2);

        Assert.AreEqual(1, usuario1.Id);
        Assert.AreEqual(2, usuario2.Id);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario2);

        Assert.AreEqual(2, gestorUsuarios.Usuarios.Count);
        gestorUsuarios.EliminarUsuario(2);
        Assert.AreEqual(1, gestorUsuarios.Usuarios.Count);

    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario2);

        Usuario busqueda = gestorUsuarios.ObtenerUsuario(1);
        Assert.AreEqual(usuario1, busqueda);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario); // usuario con id 1

        gestorUsuarios.AgregarAdministradorSistema(1);
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeSistemaCorrectamente()
    {
        GestorUsuarios gestorUsuarios = new GestorUsuarios();
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        gestorUsuarios.AgregarUsuario(usuario); // usuario con id 1
        gestorUsuarios.AgregarAdministradorSistema(1);
        gestorUsuarios.EliminarAdministradorSistema(1);
        Assert.IsFalse(usuario.EsAdministradorSistema);
    }
}