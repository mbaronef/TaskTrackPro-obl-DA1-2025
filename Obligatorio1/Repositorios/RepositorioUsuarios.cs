using Dominio;
using Interfaces.InterfacesRepositorios;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private List<Usuario> _usuarios;
    private static int _cantidadUsuarios;

    public RepositorioUsuarios() {
        _usuarios = new List<Usuario>();
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
}
    