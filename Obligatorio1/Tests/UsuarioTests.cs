namespace Tests
{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void constructor()
        {
            // Arrange

            // Act
            var usuario = new Usuario("Juan", "Perez", "contrasena", "email"); // completar según tu constructor

            // Assert
            Assert.AreEqual(usuario.Nombre, "Juan");
            Assert.AreEqual(usuario.Apellido, "Perez");
            Assert.AreEqual(usuario.Contraseña, "contrasena");
            Assert.AreEqual(usuario.Email, "email");
        }
    }
}