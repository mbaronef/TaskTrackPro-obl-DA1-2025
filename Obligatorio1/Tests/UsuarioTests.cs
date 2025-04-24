using Dominio;

namespace Tests

{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void ConstructorConParametrosAsignaCorrectamente()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");
            
            Assert.AreEqual("Juan", usuario.Nombre);
            Assert.AreEqual("Perez", usuario.Apellido);
            Assert.AreEqual(fechaNacimiento, usuario.FechaNacimiento);
            Assert.AreEqual("unemail@gmail.com", usuario.Email);
            Assert.AreEqual("Contrase#a3", usuario.Contrasena);
        }

        [TestMethod]
        public void UsuarioSeCreaConListaNotificacionesVacia()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");
            
            Assert.IsNotNull(usuario.Notificaciones);
            Assert.AreEqual(0,usuario.Notificaciones.Count);
        }
        
        [TestMethod]
        public void UnNuevoUsuarioNoEsAdministradorDeProyectoPorDefecto()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");
            
            Assert.IsFalse(usuario.EsAdministradorProyecto);
        }

        [TestMethod]
        public void ContadorCantidadUsuarios()
        {
            DateTime fechaNacimiento1 = new DateTime(2000, 9, 1);
            DateTime fechaNacimiento2 = new DateTime(2000, 1, 9);
            Usuario usuario1 = new Usuario("Juan", "Perez", fechaNacimiento1, "unemail1@gmail.com", "Contrase#a1");
            Assert.AreEqual(Usuario.Contador, 1);
            Usuario usuario2 = new Usuario("Mateo", "Perez", fechaNacimiento2, "unemail2@gmail.com", "Contrase#a2");
            Assert.AreEqual(Usuario.Contador, 2);
        }

        [TestMethod]
        public void SeAsigaUnId()
        {
            DateTime fechaNacimiento1 = new DateTime(2000, 9, 1);
            DateTime fechaNacimiento2 = new DateTime(2000, 1, 9);
            Usuario usuario1 = new Usuario("Juan", "Perez", fechaNacimiento1, "unemail1@gmail.com", "Contrase#a1");
            Usuario usuario2 = new Usuario("Mateo", "Perez", fechaNacimiento2, "unemail2@gmail.com", "Contrase#a2");

            Assert.AreEqual(usuario1.Id, 1);
            Assert.AreEqual(usuario2.Id, 2);
        }

        [TestMethod]
        public void IdYContadorSeCorresponden()
        {
            DateTime fechaNacimiento1 = new DateTime(2000, 9, 1);
            DateTime fechaNacimiento2 = new DateTime(2000, 1, 9);
            Usuario usuario1 = new Usuario("Juan", "Perez", fechaNacimiento1, "unemail1@gmail.com", "Contrase#a1");
            Assert.AreEqual(usuario1.Id, Usuario.Contador);
            Usuario usuario2 = new Usuario("Mateo", "Perez", fechaNacimiento2, "unemail2@gmail.com", "Contrase#a2");
            Assert.AreEqual(usuario2.Id, Usuario.Contador);
        }

        /*[TestMethod]
        public void ContrasenaIncluyeAlMenosUnaMayuscula()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario1 = new Usuario("Juan", "Perez", "contrasena", "email");
            Assert.IsFalse(usuario1.contrasenaValida());
            Usuario usuario2 = new Usuario("Juan", "Perez", "Contrasena", "email");
            Assert.IsTrue(usuario2.contrasenaValida());
            Usuario usuario3 = new Usuario("Juan", "Perez", "COntrasena", "email");
            Assert.IsTrue(usuario3.contrasenaValida());
        }*/
    }
}