using Dominio;

namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    [TestMethod]
    public void ConstructorSinParametrosCreaGestorValido()
    {
        GestorUsuarios gestorUsuariosS = new GestorUsuarios();
        Assert.IsNotNull(gestorUsuariosS);
        Assert.AreEqual(0,gestorUsuariosS.Usuarios.Count);
        Assert.AreEqual(0,gestorUsuariosS.AdministradoresSistema.Count);
    }

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        GestorUsuarios gestorUsuariosS = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario2);
        
        Assert.AreEqual(2,gestorUsuariosS.Usuarios.Count);
        Assert.AreSame(usuario1, gestorUsuariosS.Usuarios[0]);
        Assert.AreSame(usuario2, gestorUsuariosS.Usuarios[1]);
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIds()
    {
        GestorUsuarios gestorUsuariosS = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario2);
        
        Assert.AreEqual(1, usuario1.Id);
        Assert.AreEqual(2, usuario2.Id);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        GestorUsuarios gestorUsuariosS = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario1);
        Usuario usuario2 = new Usuario("Mateo", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3"); 
        gestorUsuariosS.AgregarUsuario(usuario2);
        
        Usuario busqueda = gestorUsuariosS.ObtenerUsuario(1);
        Assert.AreEqual(usuario1, busqueda);
    }


}