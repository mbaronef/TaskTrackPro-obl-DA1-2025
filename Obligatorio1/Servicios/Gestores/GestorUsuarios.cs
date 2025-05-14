using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios
{
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    
    public Usuario AdministradorInicial { get; private set; }
    public RepositorioUsuarios Usuarios { get; } = new RepositorioUsuarios();

    public GestorUsuarios()
    {
        AdministradorInicial = CrearUsuario("Admin", "Admin", new DateTime(1999, 01, 01), "admin@sistema.com", _contrasenaPorDefecto);
        AdministradorInicial.EsAdministradorSistema = true; // primer administrador (con id 0)
        Usuarios.Agregar(AdministradorInicial);
    }
    
    public Usuario CrearUsuario(string nombre, string apellido, DateTime fechaNacimiento, string email, string contrasena)
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(contrasena);
        return new Usuario(nombre, apellido, fechaNacimiento, email, contrasenaEncriptada);
    }

    public void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "crear usuarios");
        Usuarios.Agregar(usuario);
        string mensajeNotificacion =
            $"Se creó un nuevo usuario: {usuario.Nombre} {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public void EliminarUsuario(Usuario solicitante, int id)
    {
        ValidarUsuarioNoEsPrimerAdmin(id);
        Usuario usuario = ObtenerUsuarioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar usuarios");
        }
        VerificarUsuarioNoEsMiembroDeProyectos(usuario);
        Usuarios.Eliminar(usuario.Id);
        string mensajeNotificacion = $"Se eliminó un nuevo usuario. Nombre: {usuario.Nombre}, Apellido: {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = Usuarios.ObtenerPorId(idUsuario);
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
        VerificarUsuarioADesasignarSeaAdminProyecto(administradorProyecto);
        VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(administradorProyecto);
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        VerificarUsuarioPuedaReiniciarContrasena(solicitante, usuarioObjetivo);

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        
        Notificar(usuarioObjetivo, $"Se reinició su contraseña. La nueva contraseña es {_contrasenaPorDefecto}");
    }

    public void AutogenerarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        VerificarSolicitantePuedaAutogenerarContrasena(solicitante);
        string nuevaContrasena = UtilidadesContrasena.AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
    }

    public void ModificarContrasena(Usuario solicitante, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        VerificarSolicitantePuedaModificarContrasena(solicitante, usuarioObjetivo);
        
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        NotificarUsuarioModificacionSiNoEsElMismo(solicitante, usuarioObjetivo,  nuevaContrasena);
    }

    public Usuario LogIn(string email, string contrasena)
    {
        Usuario usuario = Usuarios.ObtenerUsuarioPorEmail(email);
        VerificarUsuarioRegistrado(usuario);
        VerificarContrasenaCorrecta(usuario, contrasena);
        return usuario;
    }

    public List<Usuario> ObtenerUsuariosDiferentes(List<Usuario> usuarios)
    {
        return Usuarios.ObtenerTodos().Except(usuarios).ToList();
    }

    private void VerificarPermisoAdministradorSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion}");
        }
    }
    
    private void ValidarUsuarioNoEsPrimerAdmin(int id)
    {
        if (id == 0)
        {
            throw new ExcepcionServicios("No se puede eliminar al primer administrador del sistema");
        }
    }

    public void VerificarUsuarioNoEsMiembroDeProyectos(Usuario usuario)
    {
        if(usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionServicios("No puede eliminar un usuario que es miembro de un proyecto.");
        }
    }
    
    private void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios("No se puede quitar permisos de proyecto a un usuario que tiene un proyecto a su cargo.");
        }
    }

    private void VerificarUsuarioADesasignarSeaAdminProyecto(Usuario usuario)
    {
        if (!usuario.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("El usuario a desasignar no es administrador de proyectos.");
        }
        
    }
    
    private void VerificarSolicitantePuedaAutogenerarContrasena(Usuario solicitante)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para autogenerar la contraseña del usuario");
        }
    }
    
    private void VerificarUsuarioPuedaReiniciarContrasena(Usuario solicitante, Usuario usuario)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuario))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para reiniciar la contraseña del usuario");
        }
    }
    
    private void VerificarSolicitantePuedaModificarContrasena(Usuario solicitante, Usuario usuario)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuario))
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para modificar la contraseña del usuario");
        }
        
    }
    
    private void NotificarUsuarioModificacionSiNoEsElMismo(Usuario solicitante, Usuario usuarioObjetivo, String nuevaContrasena)
    {
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
        }
    }
    
    private void VerificarUsuarioRegistrado(Usuario usuario)
    {
        if (usuario == null)
        {
            throw new ExcepcionServicios("Correo electrónico no registrado.");
        }
    }

    private void VerificarContrasenaCorrecta(Usuario usuario, string contrasena)
    {
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionServicios("La contraseña ingresada es incorrecta.");
        }
    }
    
    private void NotificarAdministradoresSistema(Usuario solicitante, string mensajeNotificacion)
    {
        List<Usuario> administradores = Usuarios.ObtenerTodos().Where(usuario => usuario.EsAdministradorSistema && !usuario.Equals(solicitante)).ToList();
        administradores.ForEach(admin => Notificar(admin, mensajeNotificacion));
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        usuario.RecibirNotificacion(mensajeNotificacion);
    }
}