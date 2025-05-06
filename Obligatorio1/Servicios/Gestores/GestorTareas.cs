using Dominio;
namespace Servicios.Gestores;

public class GestorTareas
{
    private GestorProyectos _gestorProyectos;
    private static int _cantidadTareas;

    public GestorTareas(GestorProyectos gestorProyectos)
    {
        _gestorProyectos = gestorProyectos;
    }

    public void AgregarTareaAlProyecto(int idProyecto,  Usuario solicitante, Tarea nuevaTarea)
    {
        Proyecto proyecto =  _gestorProyectos.ObtenerProyecto(idProyecto);
        
        _gestorProyectos.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        _cantidadTareas++;
        nuevaTarea.Id = _cantidadTareas;
        
        proyecto.ValidarTareaNoDuplicada(nuevaTarea);
        proyecto.AgregarTarea(nuevaTarea);
        
        proyecto.NotificarMiembros($"Se agregó la tarea (id {nuevaTarea.Id}) al proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarTareaDelProyecto(int idProyecto, Usuario solicitante, int idTareaAEliminar)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
         
        _gestorProyectos.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.EliminarTarea(idTareaAEliminar);
        
        proyecto.NotificarMiembros($"Se eliminó la tarea (id {idTareaAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public Tarea ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);
        
        return tarea;
    }
    
}