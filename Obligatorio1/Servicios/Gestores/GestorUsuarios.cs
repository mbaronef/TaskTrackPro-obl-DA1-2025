using Dominio;
using InterfacesServicios;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios
{
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    
    public Usuario AdministradorInicial { get; private set; }
    public RepositorioUsuarios Usuarios { get; } = new RepositorioUsuarios();
    
    private readonly INotificador _notificador;


    public GestorUsuarios(INotificador notificador)
    {
        _notificador = notificador;
        AdministradorInicial = CrearUsuario("Admin", "Admin", new DateTime(1999, 01, 01), "admin@sistema.com", _contrasenaPorDefecto);
        AdministradorInicial.EsAdministradorSistema = true; 
        Usuarios.Agregar(AdministradorInicial);
    }
    
    public Usuario CrearUsuario(string nombre, string apellido, DateTime fechaNacimiento, string email, string contrasena)
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(contrasena);
        return new Usuario(nombre, apellido, fechaNacimiento, email, contrasenaEncriptada);
    }

    public void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "crear usuarios");
        Usuarios.Agregar(usuario);
        string mensajeNotificacion = MensajesNotificacion.UsuarioCreado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public void EliminarUsuario(Usuario solicitante, int id)
    {
        ValidarUsuarioNoEsAdministradorInicial(id);
        Usuario usuario = ObtenerUsuarioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegado);
        }
        PermisosUsuariosServicio.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        Usuarios.Eliminar(usuario.Id);
        string mensajeNotificacion = MensajesNotificacion.UsuarioEliminado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = Usuarios.ObtenerPorId(idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesError.UsuarioNoEncontrado);
        }
        return usuario;
    }

    public void AgregarAdministradorSistema(Usuario solicitante, int idUsuario)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = ObtenerUsuarioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = ObtenerUsuarioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void DesasignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = ObtenerUsuarioPorId(idUsuario);
        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(administradorProyecto, "solicitante");
        PermisosUsuariosServicio.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(administradorProyecto);
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo, "reiniciar la contraseña del usuario");

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaReiniciada(_contrasenaPorDefecto));
    }

    public void AutogenerarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        PermisosUsuariosServicio.VerificarSolicitantePuedaAutogenerarContrasena(solicitante);
        string nuevaContrasena = UtilidadesContrasena.AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaModificada(nuevaContrasena));
    }

    public void ModificarContrasena(Usuario solicitante, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo, "modificar la contraseña del usuario");
        
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
    
    public void ValidarUsuarioNoEsAdministradorInicial(int idUsuario)
    {
        if (idUsuario == AdministradorInicial.Id)
        {
            throw new ExcepcionPermisos(MensajesError.PrimerAdminSistema);
        }
    }

    public void VerificarUsuarioNoEsMiembroDeProyecto(Usuario usuario)
    {
        if(usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioMiembroDeProyecto);
        }
    }
    
    private void NotificarUsuarioModificacionSiNoEsElMismo(Usuario solicitante, Usuario usuarioObjetivo, String nuevaContrasena)
    {
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaModificada(nuevaContrasena));
        }
    }
    
    private void VerificarUsuarioRegistrado(Usuario usuario)
    {
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesError.UsuarioNoEncontrado);
        }
    }

    private void VerificarContrasenaCorrecta(Usuario usuario, string contrasena)
    {
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionUsuario(MensajesError.ContrasenaIncorrecta);
        }
    }
    
    private void NotificarAdministradoresSistema(Usuario solicitante, string mensajeNotificacion)
    {
        List<Usuario> administradores = Usuarios.ObtenerTodos().Where(usuario => usuario.EsAdministradorSistema && !usuario.Equals(solicitante)).ToList();
        administradores.ForEach(admin => Notificar(admin, mensajeNotificacion));
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        _notificador.NotificarUno(usuario, mensajeNotificacion);
    }
}