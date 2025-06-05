using Dominio;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Tests.Servicios
{
    [TestClass]
    public class PermisosUsuariosServicioTests
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
            PermisosUsuariosServicio.VerificarPermisoAdminSistema(adminSistema, "realizar una acción");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarPermisoAdminSistema_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuariosServicio.VerificarPermisoAdminSistema(usuarioComun, "hacer algo");
        }

        [TestMethod]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, adminProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioMiembroDelProyecto_NoLanzaExcepcion_SiEsMiembro()
        {
            PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(usuarioComun.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void VerificarUsuarioMiembroDelProyecto_LanzaExcepcion_SiNoEsMiembro()
        {
            var otroUsuario = new Usuario { Id = 999 };
            PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(otroUsuario.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoAdministraOtroProyecto_LanzaExcepcion_SiAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuariosServicio.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioNoAdministraOtroProyecto_NoLanzaExcepcion_SiNoAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuariosServicio.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(usuarioComun, "usuario común");
        }

        [TestMethod]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(adminProyecto, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_LanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuariosServicio.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_NoLanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuariosServicio.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_NoLanzaExcepcion_SiSolicitanteEsAdmin()
        {
            PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(adminSistema, usuarioComun, "modificar contraseña");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(usuarioComun, adminSistema, "reiniciar contraseña");
        }

        [TestMethod]
        public void VerificarPermisoAdminSistemaOAdminProyecto_NoLanzaExcepcion_SiEsAdminSistema()
        {
            PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(adminSistema, "acc");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void VerificarPermisoAdminSistemaOAdminProyecto_LanzaExcepcion_SiNoEsNinguno()
        {
            PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(usuarioComun, "acc");
        }

        [TestMethod]
        public void VerificarSolicitantePuedaAutogenerarContrasena_NoLanzaExcepcion_SiTienePermisos()
        {
            PermisosUsuariosServicio.VerificarSolicitantePuedaAutogenerarContrasena(adminSistema);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void VerificarSolicitantePuedaAutogenerarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuariosServicio.VerificarSolicitantePuedaAutogenerarContrasena(usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioNoEsMiembroDeProyecto_NoLanzaExcepcion_SiNoTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 0 };
            PermisosUsuariosServicio.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoEsMiembroDeProyecto_LanzaExcepcion_SiTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 2 };
            PermisosUsuariosServicio.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }
    }
}