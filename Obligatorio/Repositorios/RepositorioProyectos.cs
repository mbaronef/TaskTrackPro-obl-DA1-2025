using Dominio;
using Microsoft.EntityFrameworkCore;
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
        //var proyecto = _contexto.Proyectos
          //  .Include(p => p.Administrador)
          //  .Include(p => p.Miembros)
          //  .Include(p => p.Tareas)
          //  .FirstOrDefault(p => p.Id == id);
          
            Proyecto proyecto = _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
            _contexto.Proyectos.Remove(proyecto);
            
            // debug
            Console.WriteLine("Estado del proyecto antes del SaveChanges:");
            Console.WriteLine($"- ID: {proyecto.Id}");
            Console.WriteLine($"- Admin: {proyecto.Administrador?.Id}, Trackeado: {_contexto.Entry(proyecto.Administrador!).State}");
            Console.WriteLine($"- Miembros: {string.Join(", ", proyecto.Miembros.Select(m => $"{m.Id} ({_contexto.Entry(m).State})"))}");
            Console.WriteLine($"- Tareas: {string.Join(", ", proyecto.Tareas.Select(t => $"{t.Id} ({_contexto.Entry(t).State})"))}");
            
            _contexto.SaveChanges();
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _contexto.Proyectos.ToList();
    }
    
    public void Update(Proyecto proyecto)
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
    
    public void UpdateTarea(Tarea tarea)
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