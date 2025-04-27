using Dominio.Excepciones;

namespace Dominio;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();

    public GestorUsuarios()
    {
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _cantidadUsuarios++;
        usuario.Id = _cantidadUsuarios;
        Usuarios.Add(usuario);
    }

    public void EliminarUsuario(int id)
    {
        Usuarios.RemoveAll(u => u.Id == id);
    }

    public Usuario ObtenerUsuario(int idUsuario)
    {
        return Usuarios.Find(u => u.Id == idUsuario);
    }

    public void AgregarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = Usuarios.Find(u => u.Id == idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void EliminarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = Usuarios.Find(u => u.Id == idUsuario);
        usuario.EsAdministradorSistema = false;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, Usuario nuevoAdministradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para asignar administradores de proyectos.");
        }
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void EliminarAdministradorProyecto(Usuario solicitante, Usuario administradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para eliminar administradores de proyectos.");
        }

        if (!administradorProyecto.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("El usuario a eliminar no es administrador de proyectos.");
        }
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario administrador, Usuario usuarioObjetivo)
    {
        if (!administrador.EsAdministradorSistema && !administrador.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para reiniciar la contraseña del usuario.");
        }
        usuarioObjetivo.CambiarContrasena("TaskTrackPro@2025");
    }
}