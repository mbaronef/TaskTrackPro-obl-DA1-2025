using Dominio;
using Dominio.Excepciones;

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
            Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
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
            string contrasenaEncriptada = Usuario.EncriptarContrasena(unaContrasena);
            Assert.AreNotEqual(unaContrasena, contrasenaEncriptada);
        }

        [TestMethod]
        public void ContrasenasIgualesEncriptanIgual()
        {
            string unaContrasena = "Contrase#a3";
            string contrasenaEncriptada1 = Usuario.EncriptarContrasena(unaContrasena);
            string contrasenaEncriptada2 = Usuario.EncriptarContrasena(unaContrasena);

            Assert.AreEqual(contrasenaEncriptada1, contrasenaEncriptada2);
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioOk()
        {
            string contrasenaIngresada = "Contrase#a3";

            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");

            Assert.IsTrue(usuario.Autenticar(contrasenaIngresada));
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioIncorrecto()
        {
            string contrasenaIngresada = "OtraContraseña";

            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "Contrase#a3");

            Assert.IsFalse(usuario.Autenticar(contrasenaIngresada));
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaMuyCorta()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "P3e.");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinMayusculas()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "minuscula1@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinMinusculas()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "MAYUSCULA1@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinNumeros()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "CoNtRaSeN@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinCaracterEspecial()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@gmail.com", "CoNtRaSeN14");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeEmailInvalido()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "emailinvalido", "Contrase#a3");
        }

        [TestMethod]
        public void SeCambiaContrasenaCorrectamente()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@adinet.com", "Contrase#a3");

            string nuevaContrasena = "CoNtRaSeN@";
            usuario.CambiarContrasena(nuevaContrasena);
            Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
        }

        [TestMethod]
        public void SeRecibeUnaNotificacionCorrectamente()
        {
            DateTime fechaNacimiento = new DateTime(2000, 9, 1);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@hotmail.com", "6onTrase}a3");
            Notificacion notificacion = new Notificacion("un mensaje de notificación");
            
            usuario.RecibirNotificacion(notificacion);

            Assert.AreEqual(1, usuario.Notificaciones.Count);
            Assert.AreSame(notificacion, usuario.Notificaciones.ElementAt(0));
        }
    }
}