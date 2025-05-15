using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioProyectos : IRepositorio<Proyecto>
{
    private List<Proyecto> _proyectos;
    private static int _cantidadProyectos;

    public RepositorioProyectos()
    {
        _proyectos = new List<Proyecto>();
    }

    public void Agregar(Proyecto objeto)
    {
        objeto.Id = ++_cantidadProyectos;
        _proyectos.Add(objeto);
    }

    public Proyecto ObtenerPorId(int id)
    {
        return _proyectos.Find(proyecto=>proyecto.Id == id);
    }

    public void Eliminar(int id)
    {
        _proyectos.Remove(_proyectos.Find(proyecto => proyecto.Id == id));
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _proyectos;
    }
}