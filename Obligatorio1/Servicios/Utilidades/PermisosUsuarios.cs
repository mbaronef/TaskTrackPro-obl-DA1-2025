using Dominio;
using Servicios.Excepciones;

namespace Servicios.Utilidades;

public static class PermisosUsuariosServicio
{
    public static void VerificarPermisoAdminSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioNoAdminSistema);
        }
    }
    
    public static void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
        {
            throw new ExcepcionPermisos(MensajesError.NoEsAdminDelProyecto);
        } 
    }
    
    public static void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);

        if (usuario is null)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioNoMiembroDelProyecto);
        }
    }
    
    private static Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }

    public static void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioAdministrandoProyecto);
        }
    }

    public static void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, string tipoUsuario)
    {
        if (!solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPorTipo(tipoUsuario));
        }
    }
    
    public static void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioAdministrandoProyecto);
        }
    }

    public static void VerificarUsuarioPuedaReiniciarOModificarContrasena(Usuario solicitante, Usuario usuario, string accion)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto && !solicitante.Equals(usuario))
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPara($"{accion}"));
        }
    }
    
    public static void VerificarPermisoAdminSistemaOAdminProyecto(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema && !usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPara(accion));
        }
    }
    
    public static void VerificarSolicitantePuedaAutogenerarContrasena(Usuario solicitante)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPara("autogenerar la contraseña del usuario"));
        }
    }
    
    public static void VerificarUsuarioNoEsMiembroDeProyecto(Usuario usuario)
    {
        if(usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioNoMiembroDelProyecto);
        }
    }
}
