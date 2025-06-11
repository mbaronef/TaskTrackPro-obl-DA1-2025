using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioProyectos : IRepositorio<Proyecto>
{
    private SqlContext _contexto;

    public RepositorioProyectos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Proyecto objeto)
    {
        _contexto.Proyectos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Proyecto ObtenerPorId(int id)
    {
        return _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
    }

    public void Eliminar(int id)
    {
        Proyecto proyectoAEliminar = _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
        _contexto.Proyectos.Remove(proyectoAEliminar);
        _contexto.SaveChanges();
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _contexto.Proyectos.ToList();
    }
}