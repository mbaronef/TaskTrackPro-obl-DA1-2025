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
            return new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
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

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaNombreNoVacio()
        {
            Usuario usuario = new Usuario("", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaNombreNoNull()
        {
            Usuario usuario = new Usuario(null, "Perez", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaApellidoNoVacio()
        {
            Usuario usuario = new Usuario("Juan", "", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaApellidoNoNull()
        {
            Usuario usuario = new Usuario("Juan", null, _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
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
            try
            {
                usuario.CambiarContrasena(nuevaContrasena);
            }
            catch
            {
            } // Ignorar la excepción

            Assert.IsFalse(usuario.Autenticar(nuevaContrasena));
            Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
        }

        [TestMethod]
        public void SeCambiaEmailCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.CambiarEmail("otroEmail@fi365.ort.edu.uy");
            Assert.AreEqual("otroEmail@fi365.ort.edu.uy", usuario.Email);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void DaErrorSiSeCambiaEmailInvalido()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.CambiarEmail("email");
        }

        [TestMethod]
        public void NoSeCambiaEmailInvalido()
        {
            Usuario usuario = CrearUsuarioValido();
            string nuevoEmail = "email";
            try
            {
                usuario.CambiarEmail(nuevoEmail);
            }
            catch (ExcepcionDominio)
            {
            } // Ignorar la excepción

            Assert.AreNotEqual(nuevoEmail, usuario.Email);
            Assert.AreEqual("unemail@gmail.com", usuario.Email);
        }

        [TestMethod]
        public void SeRecibeUnaNotificacionCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensaje de notificación";
            usuario.RecibirNotificacion(mensaje);

            Assert.AreEqual(1, usuario.Notificaciones.Count);
            Assert.AreEqual("un mensaje de notificación", usuario.Notificaciones.ElementAt(0).Mensaje);
            Assert.AreEqual(DateTime.Today, usuario.Notificaciones.ElementAt(0).Fecha);
        }

        [TestMethod]
        public void SeBorranNotificacionesCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensje de notificación";
            usuario.RecibirNotificacion(mensaje);
            usuario.RecibirNotificacion(mensaje);
            int id1 = usuario.Notificaciones.ElementAt(0).Id;

            usuario.BorrarNotificacion(id1);
            Assert.AreEqual(1, usuario.Notificaciones.Count);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void BorrarNotificacionInexistenteDaError()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensje de notificación";
            usuario.RecibirNotificacion(mensaje);
            usuario.RecibirNotificacion(mensaje);
            // se agregan notificaciones con id 1 e id 2
            usuario.BorrarNotificacion(0);
        }
        
        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void BorrarNotificacionEnListaVaciaDaError()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.BorrarNotificacion(2);
        }

        [TestMethod]
        public void EqualsRetornaTrueSiLosIdsSonIguales()
        {
            Usuario adminSistema = new Usuario("admin", "admin", new DateTime(0001,01,01), "admin@admin.com", "AdminTaskTrackPro@2025");
            GestorUsuarios gestor = new GestorUsuarios(adminSistema);
            Usuario usuario1 = CrearUsuarioValido();
            gestor.AgregarUsuario(adminSistema,usuario1);
            Usuario usuario2 = gestor.ObtenerUsuarioPorId(usuario1.Id);
            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsTrue(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiLosIdsNoSonIguales()
        {
            Usuario adminSistema = new Usuario("admin", "admin", new DateTime(0001,01,01), "admin@admin.com", "AdminTaskTrackPro@2025");
            GestorUsuarios gestor = new GestorUsuarios(adminSistema);
            Usuario usuario1 = CrearUsuarioValido();
            gestor.AgregarUsuario(adminSistema, usuario1);
            Usuario usuario2 = CrearUsuarioValido();
            gestor.AgregarUsuario(adminSistema, usuario2);
            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiUnObjetoEsNull()
        {
            Usuario usuario1 = CrearUsuarioValido();
            bool sonIguales = usuario1.Equals(null);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiUnObjetoNoEsUsuario()
        {
            Usuario usuario1 = CrearUsuarioValido();
            int otro = 0;
            bool sonIguales = usuario1.Equals(otro);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void GetHashCodeFuncionaOk()
        {
            Usuario usuario1 = CrearUsuarioValido();
            Usuario usuario2 = CrearUsuarioValido();
            //ambos tienen mismo id ya que no hay un gestor que maneje ids
            Usuario usuario3 = CrearUsuarioValido();
            usuario3.Id = 3; // usuario con id distinto a los otros 2 (se hardcodea en vez de llamar al gestor por simplicidad)
            Assert.AreEqual(usuario1.GetHashCode(), usuario2.GetHashCode());
            Assert.AreNotEqual(usuario3.GetHashCode(), usuario1.GetHashCode());
        }
    }
}