using Dominio.Dummies;

namespace Dominio.Interfaces;

public interface IGestorProyectos
{
    public void crearProyecto(Proyecto proyecto, Usuario solicitante);
    public void eliminarProyecto(int id);
    public List<Proyecto> obtenerProyectosPorUsuario(Usuario usuarioLogeado);
    public void cambiarAdministradorProyecto(Usuario solicitante, Proyecto proyecto, Usuario nuevoAdministradorProyecto);
    public void agregarMiembro(Usuario usuario, Proyecto proyecto);
    public void eliminarMiembro(int id, Proyecto proyecto);
    public void agregarTarea(Tarea tarea, Proyecto proyecto);
    public void eliminarTarea(int id,  Proyecto proyecto); 
    // se necesitaria el proyecto??? porque las tareas son unicas
    // a no ser que los ids se asignen por poryevto ej el proyecto a tiene la tarea 1 y el proyecto b tambien tiene una tarea 1
    
    
    // falta modificaciones
}