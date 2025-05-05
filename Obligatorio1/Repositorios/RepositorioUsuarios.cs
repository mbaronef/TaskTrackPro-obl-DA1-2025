using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private Usuario _admin;
    private List<Usuario> _usuarios;
    private static int _cantidadUsuarios;

    public RepositorioUsuarios()
    {
        _usuarios = new List<Usuario>();
        _admin = new Usuario("Admin", "Admin", new DateTime(1999, 01, 01), "admin@sistema.com", "TaskTrackPro@2025");
        _admin.EsAdministradorSistema = true;
        _usuarios.Add(_admin);
    }

    public void Agregar(Usuario objeto)
    {
        objeto.Id = ++_cantidadUsuarios;
        _usuarios.Add(objeto);
    }

    public Usuario ObtenerPorId(int id)
    {
        return _usuarios.Find(usuario => usuario.Id == id);

    }

    public void Eliminar(int id)
    {
        _usuarios.Remove(_usuarios.Find(usuario => usuario.Id == id));
    }

    public List<Usuario> ObtenerTodos()
    {
        return _usuarios;
    }

    public Usuario ObtenerUsuarioPorEmail(string email)
    {
        return _usuarios.Find(usuario => usuario.Email == email);
    }

    public void ActualizarContrasena(int idUsuario, string contrasenaEncriptada)
    {
        Usuario usuario = ObtenerPorId(idUsuario);
        usuario.EstablecerContrasenaEncriptada(contrasenaEncriptada);
    }
}
    