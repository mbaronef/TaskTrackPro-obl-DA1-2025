using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioRecursos : IRepositorio<Recurso>
{
    public List<Recurso> _recursos;
    private static int _cantidadRecursos;

    public RepositorioRecursos()
    {
        _recursos = new List<Recurso>();
    }

    public void Agregar(Recurso objeto)
    {
        objeto.Id = ++_cantidadRecursos;
        _recursos.Add(objeto);
    }

    public Recurso ObtenerPorId(int id)
    {
        return _recursos.Find(recurso => recurso.Id == id);
    }

    public void Eliminar(int id)
    {
        _recursos.Remove(_recursos.Find(recurso => recurso.Id == id));
    }

    public List<Recurso> ObtenerTodos()
    {
        return _recursos;
    }
}