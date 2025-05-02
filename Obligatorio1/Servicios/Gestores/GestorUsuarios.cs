using Dominio;
using Dominio.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public List<Usuario> Usuarios { get; } = new List<Usuario>();

    public GestorUsuarios(Usuario adminSistema)
    {
        Usuarios.Add(adminSistema);
        adminSistema.EsAdministradorSistema = true;
        //No se manejan ids, el primer administrador tiene id 0
    }

    public void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "crear usuarios");
        usuario.Id = ++_cantidadUsuarios;
        Usuarios.Add(usuario);
        string mensajeNotificacion =
            $"Se creó un nuevo usuario: {usuario.Nombre} {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public void EliminarUsuario(Usuario solicitante, int id)
    {
        if (id == 0)
        {
            throw new ExcepcionServicios("No se puede eliminar al primer administrador del sistema");
        }
        Usuario usuario = ObtenerUsuarioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar usuarios");
        }
        Usuarios.Remove(usuario);
        string mensajeNotificacion = $"Se eliminó un nuevo usuario. Nombre: {usuario.Nombre}, Apellido: {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = Usuarios.SingleOrDefault(u => u.Id == idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionServicios("El usuario no existe");
        }
        return usuario;
    }

    public void AgregarAdministradorSistema(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = ObtenerUsuarioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = ObtenerUsuarioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void DesasignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = ObtenerUsuarioPorId(idUsuario);
        if (!administradorProyecto.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("El usuario a desasignar no es administrador de proyectos.");
        }
        if (administradorProyecto.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios("No se puede quitar permisos de proyecto a un usuario que tiene un proyecto a su cargo.");
        }
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para reiniciar la contraseña del usuario");
        }

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.EncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        
        Notificar(usuarioObjetivo, $"Se reinició su contraseña. La nueva contraseña es {_contrasenaPorDefecto}");
    }

    public void AutogenerarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para autogenerar la contraseña del usuario");
        }
        
        string nuevaContrasena = UtilidadesContrasena.AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.EncriptarContrasena(nuevaContrasena);
        
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
    }

    public void ModificarContrasena(Usuario solicitante, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para modificar la contraseña del usuario");
        }
        UtilidadesContrasena.ValidarFormatoContrasena(nuevaContrasena);
        
        string nuevaContrasenaEncriptada = UtilidadesContrasena.EncriptarContrasena(nuevaContrasena);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
        }
    }
    
    public Usuario LogIn(string email, string contrasena)
    {
        Usuario usuario = Usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario == null)
        {
            throw new ExcepcionServicios("Correo electrónico no registrado.");
        }
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionServicios("La contraseña ingresada es incorrecta.");
        }
        return usuario;
    }
    
    private void VerificarPermisoAdministradorSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion}");
        }
    }
    
    private void NotificarAdministradoresSistema(Usuario solicitante, string mensajeNotificacion)
    {
        List<Usuario> administradores = Usuarios.Where(u => u.EsAdministradorSistema && !u.Equals(solicitante)).ToList();
        foreach (Usuario admin in administradores)
        {
            Notificar(admin, mensajeNotificacion);
        }
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        usuario.RecibirNotificacion(mensajeNotificacion);
    }
}