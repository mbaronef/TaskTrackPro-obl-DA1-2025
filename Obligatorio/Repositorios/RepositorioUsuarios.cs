using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private SqlContext _contexto;

    public RepositorioUsuarios(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Usuario objeto)
    {
        _contexto.Usuarios.Add(objeto);
        _contexto.SaveChanges();
    }

    public Usuario ObtenerPorId(int id)
    {
        return _contexto.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
    }

    public void Eliminar(int id)
    {
        Usuario usuarioAEliminar = _contexto.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
        _contexto.Usuarios.Remove(usuarioAEliminar);
        _contexto.SaveChanges();
    }

    public List<Usuario> ObtenerTodos()
    {
        return _contexto.Usuarios.ToList();
    }

    public Usuario ObtenerUsuarioPorEmail(string email)
    {
        return _contexto.Usuarios.FirstOrDefault(usuario => usuario.Email == email);
    }
}