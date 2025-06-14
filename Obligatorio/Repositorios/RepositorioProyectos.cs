using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioProyectos : IRepositorioProyectos
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
        Proyecto proyecto = _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
        _contexto.Proyectos.Remove(proyecto);
        _contexto.SaveChanges();
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _contexto.Proyectos.ToList();
    }
    
    public void Actualizar(Proyecto proyecto)
    {
        var proyectoContexto = _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
            .FirstOrDefault(p => p.Id == proyecto.Id);

        if (proyectoContexto != null)
        {
            proyectoContexto.ModificarNombre(proyecto.Nombre);
            proyectoContexto.ModificarDescripcion(proyecto.Descripcion);
            proyectoContexto.ModificarFechaFinMasTemprana(proyecto.FechaFinMasTemprana);
            proyectoContexto.ModificarFechaInicio(proyecto.FechaInicio);
            _contexto.SaveChanges();
        }
    }
    
    public void ActualizarTarea(Tarea tarea)
    {
        var tareaContexto = _contexto.Set<Tarea>().FirstOrDefault(t => t.Id == tarea.Id);
        if (tareaContexto != null)
        {
            tareaContexto.ModificarTitulo(tarea.Titulo);
            tareaContexto.ModificarDescripcion(tarea.Descripcion);
            tareaContexto.ModificarDuracion(tarea.DuracionEnDias);
            tareaContexto.ModificarFechaInicioMasTemprana(tarea.FechaInicioMasTemprana); 
            tareaContexto.CambiarEstado(tarea.Estado); 
            _contexto.SaveChanges();
        }
    }
}