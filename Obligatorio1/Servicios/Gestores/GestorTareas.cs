using Dominio;
using Dominio.Excepciones;
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

    public void ModificarTituloTarea(Usuario solicitante, int idTarea, int idProyecto, string nuevoTitulo)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarTitulo(nuevoTitulo);
        proyecto.NotificarMiembros($"Se cambió el título de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public void ModificarDescripcionTarea(Usuario solicitante, int idTarea, int idProyecto, string nuevaDescripcion)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarDescripcion(nuevaDescripcion);
        proyecto.NotificarMiembros($"Se cambió la descripción de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public void ModificarDuracionTarea(Usuario solicitante, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarDuracion(nuevaDuracion);
        proyecto.NotificarMiembros($"Se cambió la duración de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public void ModificarFechaInicioTarea(Usuario solicitante, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);
        proyecto.NotificarMiembros($"Se cambió la fecha de inicio de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public void CambiarEstadoTarea(Usuario solicitante, int idTarea, int idProyecto, EstadoTarea nuevoEstado)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto );
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.CambiarEstado(nuevoEstado);
        proyecto.NotificarMiembros($"Se cambió el estado de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}' a {nuevoEstado}.");
    }

    public void AgregarDependenciaATarea(Usuario solicitante, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        Tarea tareaDependencia = ObtenerTareaPorId(proyecto.Id, idTareaDependencia);
        try
        {
            Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);
            tarea.AgregarDependencia(dependencia);
        }
        catch (ExcepcionDominio ex)
        {
            throw new ExcepcionServicios("Error al agregar dependencia");
        }
        proyecto.NotificarMiembros($"Se agregó una dependencia a la tarea id {idTarea} del proyecto '{proyecto.Nombre}' del tipo {tipoDependencia} con la tarea id {tareaDependencia.Id}.");
    }
    
    
    
    public void EliminarDependenciaDeTarea(Usuario solicitante, int idTarea, int idTareaDependencia, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyecto(idProyecto);
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(proyecto.Id, idTarea);
        tarea.EliminarDependencia(idTareaDependencia);
        proyecto.NotificarMiembros($"Se elimino una la dependencia de la tarea id {idTareaDependencia} con la tarea id {idTarea} del proyecto '{proyecto.Nombre}'.");
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