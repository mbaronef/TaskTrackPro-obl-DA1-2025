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
        public void ContadorCantidadUsuarios()
        {
            Usuario usuario1 = new Usuario("Juan", "Perez", "contrasena", "email");
            
            Assert.AreEqual(Usuario.Contador, 1);
            
            Usuario usuario2 = new Usuario("Mateo", "Perez", "contrasena", "email");
            
            Assert.AreEqual(Usuario.Contador, 2);
        }
        
        [TestMethod]
        public void SeAsigaUnId()
        {
            Usuario usuario1 = new Usuario("Juan", "Perez", "contrasena", "email");
            
            Assert.AreEqual(usuario1.Id, 1);
        }
        
        
        
    }
}