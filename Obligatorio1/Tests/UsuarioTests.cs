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
            Assert.AreEqual(usuario.Contrasena, Usuario.encriptarContrasena("Contrase#a3"));
        }

        [TestMethod]
        public void UsuarioSeCreaConListaNotificacionesVacia()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);

            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");

            Assert.IsNotNull(usuario.Notificaciones);
            Assert.AreEqual(0, usuario.Notificaciones.Count);
        }

        [TestMethod]
        public void UnNuevoUsuarioNoEsAdministradorDeProyectoPorDefecto()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);

            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");

            Assert.IsFalse(usuario.EsAdministradorProyecto);
        }

        [TestMethod]
        public void ContrasenaEncriptadaEsDistintaALaOriginal()
        {
            string unaContrasena = "Contrase#a3";
            string contrasenaEncriptada = Usuario.encriptarContrasena(unaContrasena);
            Assert.AreNotEqual(unaContrasena, contrasenaEncriptada);
        }

        [TestMethod]
        public void ContrasenasIgualesEncriptanIgual()
        {
            string unaContrasena = "Contrase#a3";
            string contrasenaEncriptada1 = Usuario.encriptarContrasena(unaContrasena);
            string contrasenaEncriptada2 = Usuario.encriptarContrasena(unaContrasena);
            
            Assert.AreEqual(contrasenaEncriptada1, contrasenaEncriptada2);
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