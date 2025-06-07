using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios
{
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public Usuario AdministradorInicial { get; private set; }
    private IRepositorioUsuarios _usuarios;
    private readonly INotificador _notificador;

    public GestorUsuarios(IRepositorioUsuarios repositorioUsuarios, INotificador notificador)
    {
        _usuarios = repositorioUsuarios;
        _notificador = notificador;
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        AdministradorInicial = new Usuario("Admin", "Admin", new DateTime(1999, 01, 01), "admin@sistema.com", contrasenaEncriptada);
        AdministradorInicial.EsAdministradorSistema = true; 
        _usuarios.Agregar(AdministradorInicial);
    }

    public void CrearYAgregarUsuario(UsuarioDTO solicitanteDTO, UsuarioDTO nuevoUsuarioDTO)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario nuevoUsuario = CrearUsuario(nuevoUsuarioDTO);
        AgregarUsuario(solicitante, nuevoUsuario);
        nuevoUsuarioDTO.Id = nuevoUsuario.Id;
    }

    public void EliminarUsuario(UsuarioDTO solicitanteDTO, int id)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        ValidarUsuarioNoEsAdministradorInicial(id);
        Usuario usuario = obtenerUsuarioDominioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegado);
        }
        PermisosUsuariosServicio.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        _usuarios.Eliminar(usuario.Id);
        string mensajeNotificacion = MensajesNotificacion.UsuarioEliminado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }
    
    public List<UsuarioListarDTO> ObtenerTodos()
    {
        return _usuarios.ObtenerTodos().Select(UsuarioListarDTO.DesdeEntidad).ToList();
    }

    public UsuarioDTO ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        return UsuarioDTO.DesdeEntidad(usuario);
    }

    public void AgregarAdministradorSistema(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void AsignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void DesasignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(administradorProyecto, "solicitante");
        PermisosUsuariosServicio.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(administradorProyecto);
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo, "reiniciar la contraseña del usuario");

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaReiniciada(_contrasenaPorDefecto));
    }

    public void AutogenerarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuariosServicio.VerificarSolicitantePuedaAutogenerarContrasena(solicitante);
        string nuevaContrasena = UtilidadesContrasena.AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaModificada(nuevaContrasena));
    }

    public void ModificarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        PermisosUsuariosServicio.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo, "modificar la contraseña del usuario");
        
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        NotificarUsuarioModificacionSiNoEsElMismo(solicitante, usuarioObjetivo,  nuevaContrasena);
    }
    
    public void BorrarNotificacion(int idUsuario, int idNotificacion)
    {
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        usuario.BorrarNotificacion(idNotificacion);
    }

    public UsuarioDTO LogIn(string email, string contrasena)
    {
        Usuario usuario = _usuarios.ObtenerUsuarioPorEmail(email);
        VerificarUsuarioRegistrado(usuario);
        VerificarContrasenaCorrecta(usuario, contrasena);
        return UsuarioDTO.DesdeEntidad(usuario);
    }

    public List<UsuarioListarDTO> ObtenerUsuariosDiferentes(List<UsuarioListarDTO> usuarios)
    {
        List<int> idsAExcluir = usuarios.Select(u => u.Id).ToList();
        return ObtenerTodos().Where(u => !idsAExcluir.Contains(u.Id)).ToList();
    }
    
    public void ValidarUsuarioNoEsAdministradorInicial(int idUsuario)
    {
        if (idUsuario == AdministradorInicial.Id)
        {
            throw new ExcepcionPermisos(MensajesError.PrimerAdminSistema);
        }
    }
    
    private Usuario CrearUsuario(UsuarioDTO nuevoUsuarioDTO)
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevoUsuarioDTO.Contrasena);
        return new Usuario(nuevoUsuarioDTO.Nombre, nuevoUsuarioDTO.Apellido, nuevoUsuarioDTO.FechaNacimiento, nuevoUsuarioDTO.Email, contrasenaEncriptada);
    }

    private void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "crear usuarios");
        _usuarios.Agregar(usuario);
        string mensajeNotificacion = MensajesNotificacion.UsuarioCreado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }
    
    private Usuario obtenerUsuarioDominioPorId(int idUsuario)
    {
        Usuario usuario = _usuarios.ObtenerPorId(idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesError.UsuarioNoEncontrado);
        }
        return usuario;
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
        List<Usuario> administradores = _usuarios.ObtenerTodos().Where(usuario => usuario.EsAdministradorSistema && !usuario.Equals(solicitante)).ToList();
        administradores.ForEach(admin => Notificar(admin, mensajeNotificacion));
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        _notificador.NotificarUno(usuario, mensajeNotificacion);
    }
}