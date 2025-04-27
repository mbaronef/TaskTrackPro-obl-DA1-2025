using Dominio;
using Dominio.Excepciones;

namespace Tests

{
    [TestClass]
    public class UsuarioTests
    {
        private DateTime _fechaNacimientoValida = new DateTime(2000, 9, 1);
        private Usuario CrearUsuarioValido()
        {
            return new Usuario("Juan", "Perez", _fechaNacimientoValida,  "unemail@gmail.com", "Contrase#a3");
        }
        
        [TestMethod]
        public void ConstructorConParametrosAsignaCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();

            Assert.AreEqual("Juan", usuario.Nombre);
            Assert.AreEqual("Perez", usuario.Apellido);
            Assert.AreEqual(_fechaNacimientoValida, usuario.FechaNacimiento);
            Assert.AreEqual("unemail@gmail.com", usuario.Email);
            Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
        }

        [TestMethod]
        public void UsuarioSeCreaConListaNotificacionesVacia()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsNotNull(usuario.Notificaciones);
            Assert.AreEqual(0, usuario.Notificaciones.Count);
        }

        [TestMethod]
        public void UnNuevoUsuarioNoEsAdministradorDeSistemaPorDefecto()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.EsAdministradorSistema);
        }

        [TestMethod]
        public void UnNuevoUsuarioNoAdministraProyectosPorDefecto()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.AreEqual(0, usuario.CantidadProyectosAdministra);
            Assert.IsFalse(usuario.EsAdministradorProyecto());
        }

        [TestMethod]
        public void ContrasenaEncriptadaEsDistintaALaOriginal()
        {
            string unaContrasena = "Contrase#a3";
            string contrasenaEncriptada = Usuario.EncriptarContrasena(unaContrasena);
            Assert.AreNotEqual(unaContrasena, contrasenaEncriptada);
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioOk()
        {
            string contrasenaIngresada = "Contrase#a3";
            Usuario usuario = CrearUsuarioValido();
            Assert.IsTrue(usuario.Autenticar(contrasenaIngresada));
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioIncorrecto()
        {
            string contrasenaIngresada = "OtraContraseña";
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.Autenticar(contrasenaIngresada));
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaMuyCorta()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "P3e.");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinMayusculas()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "minuscula1@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinMinusculas()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "MAYUSCULA1@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinNumeros()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "CoNtRaSeN@");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeContrasenaSinCaracterEspecial()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "CoNtRaSeN14");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeEmailInvalido()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "emailinvalido", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void DaErrorSiElUsuarioEsMenorDeEdad()
        {
            DateTime fechaNacimiento = new DateTime(2020, 1, 6);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@hotmail.com", "6onTrase}a3");
        }
        
        [TestMethod]
        public void SeCambiaContrasenaCorrectamente()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@adinet.com", "Contrase#a3");
            string nuevaContrasena = "CoNtRaSeN1@";
            usuario.CambiarContrasena(nuevaContrasena);
            Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
        }
        
        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void DaErrorSiSeCambiaContrasenaInvalida()
        {
            Usuario usuario = CrearUsuarioValido();
            string nuevaContrasena = "c1.A";
            usuario.CambiarContrasena(nuevaContrasena);
        }

        [TestMethod]
        public void NoSeCambiaContrasenaInvalida()
        {
            Usuario usuario = CrearUsuarioValido();
            string nuevaContrasena = "c1.A";
            Assert.IsFalse(usuario.Autenticar("c1.A"));
        }

        [TestMethod]
        public void SeRecibeUnaNotificacionCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Notificacion notificacion = new Notificacion("un mensaje de notificación");
            
            usuario.RecibirNotificacion(notificacion);
            
            Assert.AreEqual(1, usuario.Notificaciones.Count);
            Assert.AreSame(notificacion, usuario.Notificaciones.ElementAt(0));
        }
        
        [TestMethod]
        public void SeBorraUnaNotificacionCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Notificacion notificacion = new Notificacion("un mensaje de notificación");
            int id = notificacion.Id;
            usuario.RecibirNotificacion(notificacion);

            usuario.BorrarNotificacion(id);
            Assert.AreEqual(0, usuario.Notificaciones.Count);
        }
        
        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void BorrarNotificacionInexistenteDaError()
        {
            Usuario usuario = CrearUsuarioValido();
            Notificacion notificacion = new Notificacion("un mensaje de notificación");
            usuario.RecibirNotificacion(notificacion); // se agrega notificación con ID 1
            usuario.BorrarNotificacion(2); 
        }
        
        [TestMethod]
        public void EqualsRetornaTrueSiLosIdsSonIguales()
        {
            GestorUsuarios gestor = new GestorUsuarios();
            Usuario usuario1 = CrearUsuarioValido();
            gestor.AgregarUsuario(usuario1);
            Usuario usuario2 = gestor.ObtenerUsuario(usuario1.Id);
            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsTrue(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiLosIdsNoSonIguales()
        {
            GestorUsuarios gestor = new GestorUsuarios();
            Usuario usuario1 = CrearUsuarioValido();
            gestor.AgregarUsuario(usuario1);
            Usuario usuario2 = CrearUsuarioValido();
            gestor.AgregarUsuario(usuario2);
            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsFalse(sonIguales);
        }
    }
}