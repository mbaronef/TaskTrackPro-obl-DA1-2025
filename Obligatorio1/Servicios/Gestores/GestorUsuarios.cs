using Dominio;
using DTOs;
using Repositorios;
using Repositorios.Interfaces;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios
{
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public Usuario AdministradorInicial { get; private set; }
    private IRepositorioUsuarios _usuarios;

    public GestorUsuarios(IRepositorioUsuarios repositorioUsuarios)
    {
        _usuarios = repositorioUsuarios;
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
            throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar usuarios");
        }
        VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        _usuarios.Eliminar(usuario.Id);
        string mensajeNotificacion = $"Se eliminó un nuevo usuario. Nombre: {usuario.Nombre}, Apellido: {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public UsuarioDTO ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        return UsuarioDTO.DesdeEntidad(usuario);
    }
    
    public List<UsuarioListarDTO> ObtenerTodos()
    {
        return _usuarios.ObtenerTodos().Select(UsuarioListarDTO.DesdeEntidad).ToList();
    }

    public void AgregarAdministradorSistema(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        VerificarPermisoAdministradorSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void AsignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        VerificarPermisoAdministradorSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void DesasignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        VerificarPermisoAdministradorSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        VerificarUsuarioADesasignarSeaAdminProyecto(administradorProyecto);
        VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(administradorProyecto);
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        VerificarUsuarioPuedaReiniciarContrasena(solicitante, usuarioObjetivo);

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        
        Notificar(usuarioObjetivo, $"Se reinició su contraseña. La nueva contraseña es {_contrasenaPorDefecto}");
    }

    public void AutogenerarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        VerificarSolicitantePuedaAutogenerarContrasena(solicitante);
        string nuevaContrasena = UtilidadesContrasena.AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        
        Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
    }

    public void ModificarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        VerificarSolicitantePuedaModificarContrasena(solicitante, usuarioObjetivo);
        
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
        return ObtenerTodos().Except(usuarios).ToList();
    }
    
    public void ValidarUsuarioNoEsAdministradorInicial(int idUsuario)
    {
        if (idUsuario == AdministradorInicial.Id)
        {
            throw new ExcepcionServicios("No se puede eliminar al primer administrador del sistema");
        }
    }

    public void VerificarUsuarioNoEsMiembroDeProyecto(Usuario usuario)
    {
        if(usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionServicios("No puede eliminar un usuario que es miembro de un proyecto.");
        }
    }
    
    private Usuario CrearUsuario(UsuarioDTO nuevoUsuarioDTO)
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevoUsuarioDTO.Contrasena);
        return new Usuario(nuevoUsuarioDTO.Nombre, nuevoUsuarioDTO.Apellido, nuevoUsuarioDTO.FechaNacimiento, nuevoUsuarioDTO.Email, contrasenaEncriptada);
    }

    private void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        VerificarPermisoAdministradorSistema(solicitante, "crear usuarios");
        _usuarios.Agregar(usuario);
        string mensajeNotificacion =
            $"Se creó un nuevo usuario: {usuario.Nombre} {usuario.Apellido}";
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }
    
    private Usuario obtenerUsuarioDominioPorId(int idUsuario)
    {
        Usuario usuario = _usuarios.ObtenerPorId(idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionServicios("El usuario no existe");
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
        List<Usuario> administradores = _usuarios.ObtenerTodos().Where(usuario => usuario.EsAdministradorSistema && !usuario.Equals(solicitante)).ToList();
        administradores.ForEach(admin => Notificar(admin, mensajeNotificacion));
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        usuario.RecibirNotificacion(mensajeNotificacion);
    }
}