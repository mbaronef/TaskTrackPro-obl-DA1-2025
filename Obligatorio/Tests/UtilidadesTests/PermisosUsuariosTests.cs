using Dominio;
using Excepciones;
using Utilidades;

namespace Tests.UtilidadesTests
{
    [TestClass]
    public class PermisosUsuariosTests
    {
        private Usuario adminSistema;
        private Usuario adminProyecto;
        private Usuario usuarioComun;
        private Proyecto proyecto;

        [TestInitialize]
        public void Setup()
        {
            adminSistema = new Usuario { Id = 1, EsAdministradorSistema = true };
            adminProyecto = new Usuario { Id = 2, EsAdministradorProyecto = true };
            usuarioComun = new Usuario { Id = 3 };

            proyecto = new Proyecto("nombre", "descrip", DateTime.Now, adminProyecto,
                new List<Usuario> { adminProyecto, usuarioComun });
        }

        [TestMethod]
        public void VerificarPermisoAdminSistema_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarPermisoAdminSistema(adminSistema, "realizar una acción");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarPermisoAdminSistema_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarPermisoAdminSistema(usuarioComun, "hacer algo");
        }

        [TestMethod]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, adminProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioMiembroDelProyecto_NoLanzaExcepcion_SiEsMiembro()
        {
            PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(usuarioComun.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioMiembroDelProyecto_LanzaExcepcion_SiNoEsMiembro()
        {
            var otroUsuario = new Usuario { Id = 999 };
            PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(otroUsuario.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoAdministraOtroProyecto_LanzaExcepcion_SiAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioNoAdministraOtroProyecto_NoLanzaExcepcion_SiNoAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(usuarioComun, "usuario común");
        }

        [TestMethod]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(adminProyecto, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_LanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuarios.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_NoLanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuarios.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_NoLanzaExcepcion_SiSolicitanteEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(adminSistema, usuarioComun,
                "modificar contraseña");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(usuarioComun, adminSistema,
                "reiniciar contraseña");
        }

        [TestMethod]
        public void VerificarPermisoAdminSistemaOAdminProyecto_NoLanzaExcepcion_SiEsAdminSistema()
        {
            PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(adminSistema, "acc");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarPermisoAdminSistemaOAdminProyecto_LanzaExcepcion_SiNoEsNinguno()
        {
            PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(usuarioComun, "acc");
        }

        [TestMethod]
        public void VerificarSolicitantePuedaAutogenerarContrasena_NoLanzaExcepcion_SiTienePermisos()
        {
            PermisosUsuarios.VerificarSolicitantePuedaAutogenerarContrasena(adminSistema);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarSolicitantePuedaAutogenerarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuarios.VerificarSolicitantePuedaAutogenerarContrasena(usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioNoEsMiembroDeProyecto_NoLanzaExcepcion_SiNoTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 0 };
            PermisosUsuarios.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoEsMiembroDeProyecto_LanzaExcepcion_SiTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 2 };
            PermisosUsuarios.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }
    }
}