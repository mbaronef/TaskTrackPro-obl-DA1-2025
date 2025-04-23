using Dominio;
namespace Tests


{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void Constructor()
        {
            Usuario usuario = new Usuario("Juan", "Perez", "contrasena", "email");
            
            Assert.AreEqual(usuario.Nombre, "Juan");
            Assert.AreEqual(usuario.Apellido, "Perez");
            Assert.AreEqual(usuario.Contrasena, "contrasena");
            Assert.AreEqual(usuario.Email, "email");
        }
        
        [TestMethod]
        public void ContadorId()
        {
            Usuario usuario = new Usuario("Juan", "Perez", "contrasena", "email");
            
            Assert.AreEqual(usuario.Id, 1);
        }
    }
}