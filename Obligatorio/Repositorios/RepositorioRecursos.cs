using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioRecursos : IRepositorio<Recurso>
{
    private SqlContext _contexto;

    public RepositorioRecursos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Recurso objeto)
    {
        _contexto.Recursos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Recurso ObtenerPorId(int id)
    {
        return _contexto.Recursos.FirstOrDefault(recurso => recurso.Id == id);
    }

    public void Eliminar(int id)
    {
        Recurso recursoAEliminar = _contexto.Recursos.FirstOrDefault(recurso => recurso.Id == id);
        _contexto.Remove(recursoAEliminar);
        _contexto.SaveChanges();
    }

    public List<Recurso> ObtenerTodos()
    {
        return _contexto.Recursos.ToList();
    }
}