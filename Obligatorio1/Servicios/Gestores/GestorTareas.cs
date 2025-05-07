using Dominio;
using Servicios.Excepciones;
//using servicios.utilidades;

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
        // caminoCritico.CalcularCaminoCritico(proyecto);
        
        proyecto.NotificarMiembros($"Se agregó la tarea (id {nuevaTarea.Id}) al proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarTareaDelProyecto(int idProyecto, Usuario solicitante, int idTareaAEliminar)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
         
        _gestorProyectos.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.EliminarTarea(idTareaAEliminar);
        // caminoCritico.CalcularCaminoCritico(proyecto);
        
        proyecto.NotificarMiembros($"Se eliminó la tarea (id {idTareaAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public Tarea ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);
        if (tarea == null)
        {
            throw new ExcepcionServicios("Recurso no existente");
        }
        return tarea;
    }

    public void ModificarTituloTarea(Usuario solicitante, int idTarea, string nuevoTitulo)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idTarea);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarTitulo(nuevoTitulo);
        proyecto.NotificarMiembros($"Se cambió el título la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public void ModificarDescripcionTarea(Usuario solicitante, int idTarea, string nuevaDescripcion)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idTarea);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarTitulo(nuevaDescripcion);
    }
    
    //EN AGREGAR DEPENDENCIA:
    //try{
    //    Dependencias.add(dependencia);
    //    calcularCaminoCritico();
    //}
    //catch {
    //    throw new exception ... no se pudo agregar la tarea xq forma un ciclo.
    //}
    
    // EN ELIMINAR DEPENDENCIA:
    // caminoCritico.CalcularCaminoCritico(proyecto);
    
    // EN MODIFICAR ESTADO, FECHA INICIO Y DURACIÓN:
    // caminoCritico.CalcularCaminoCritico(proyecto);
    
}