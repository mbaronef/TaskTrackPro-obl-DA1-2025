using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private List<Usuario> _usuarios = new List<Usuario>();
    private static int _cantidadUsuarios;

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
    