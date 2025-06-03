using Dominio;
using Servicios.Excepciones;

namespace Servicios.Utilidades;

public static class PermisosUsuariosServicio
{
    public static void VerificarPermisoAdminSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionServicios($"No tiene permisos para {accion}");
        }
    }
    
    public static void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
        {
            throw new ExcepcionServicios("Solo el administrador del proyecto puede realizar esta acción.");
        } 
    }
    
    public static void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);

        if (usuario is null)
        {
            throw new ExcepcionServicios("El usuario no es miembro del proyecto.");
        }
    }
    
    public static Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }

    public static void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios("El usuario está administrando un proyecto.");
        }
    }

    public static void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, string tipoUsuario)
    {
        if (!solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios($"El {tipoUsuario} no tiene los permisos de administrador de proyecto.");
        }
    }
    
    public static void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios("No se puede quitar permisos de proyecto a un usuario que tiene un proyecto a su cargo.");
        }
    }

    public static void VerificarUsuarioPuedaReiniciarOModificarContrasena(Usuario solicitante, Usuario usuario, string accion)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto && !solicitante.Equals(usuario))
        {
            throw new ExcepcionServicios("{accion}");
        }
    }
    
    public static void VerificarPermisoAdminSistemaOAdminProyecto(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema && !usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion}");
        }
    }
    
    public static void VerificarSolicitantePuedaAutogenerarContrasena(Usuario solicitante)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para autogenerar la contraseña del usuario");
        }
    }
    
    public static void VerificarUsuarioNoEsMiembroDeProyecto(Usuario usuario)
    {
        if(usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionServicios("No puede eliminar un usuario que es miembro de un proyecto.");
        }
    }
    
    
}
