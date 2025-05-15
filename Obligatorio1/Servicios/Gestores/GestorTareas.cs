using Dominio;
using Dominio.Excepciones;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorTareas
{
    private GestorProyectos _gestorProyectos;
    private static int _cantidadTareas;

    public GestorTareas(GestorProyectos gestorProyectos)
    {
        _gestorProyectos = gestorProyectos;
    }
    
    public void AgregarTareaAlProyecto(int idProyecto, Usuario solicitante, Tarea nuevaTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ValidarTareaIniciaDespuesDelProyecto(proyecto, nuevaTarea);

        _cantidadTareas++;
        nuevaTarea.Id = _cantidadTareas;
        
        proyecto.AgregarTarea(nuevaTarea);

        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros("Se agregó la tarea (id {nuevaTarea.Id}) al proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarTareaDelProyecto(int idProyecto, Usuario solicitante, int idTareaAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        proyecto.EliminarTarea(idTareaAEliminar);

        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros($"Se eliminó la tarea (id {idTareaAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public Tarea ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);
        if (tarea == null)
            throw new ExcepcionServicios("Tarea no existente");
        return tarea;
    }

    public void ModificarTituloTarea(Usuario solicitante, int idTarea, int idProyecto, string nuevoTitulo)
    {
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarTitulo(nuevoTitulo);
        NotificarCambio("título", idTarea, idProyecto);
    }

    public void ModificarDescripcionTarea(Usuario solicitante, int idTarea, int idProyecto, string nuevaDescripcion)
    {
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarDescripcion(nuevaDescripcion);
        NotificarCambio("descripción", idTarea, idProyecto);
    }

    public void ModificarDuracionTarea(Usuario solicitante, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarDuracion(nuevaDuracion);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        CaminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(Usuario solicitante, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        CaminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(Usuario solicitante, int idTarea, int idProyecto, EstadoTarea nuevoEstado)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.CambiarEstado(nuevoEstado);
        
        CaminoCritico.CalcularCaminoCritico(proyecto);
        
        proyecto.NotificarMiembros($"Se cambió el estado de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}' a {nuevoEstado}.");

        if (nuevoEstado == EstadoTarea.Completada)
        {
            ActualizarEstadosTareasDelProyecto(proyecto);
        }
    }

    public void AgregarDependenciaATarea(Usuario solicitante, int idTarea, int idTareaDependencia, int idProyecto, string tipoDependencia)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        Tarea tareaDependencia = ObtenerTareaPorId(idProyecto, idTareaDependencia);
        Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);
        tarea.AgregarDependencia(dependencia);
        try
        {
            CaminoCritico.CalcularCaminoCritico(proyecto);
        }
        catch (ExcepcionServicios ex)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionServicios(ex.Message);
        }

        proyecto.NotificarMiembros($"Se agregó una dependencia a la tarea id {idTarea} del proyecto '{proyecto.Nombre}' del tipo {tipoDependencia} con la tarea id {tareaDependencia.Id}.");
    }

    public void EliminarDependenciaDeTarea(Usuario solicitante, int idTarea, int idTareaDependencia, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarDependencia(idTareaDependencia);
        CaminoCritico.CalcularCaminoCritico(proyecto);
        proyecto.NotificarMiembros($"Se eliminó la dependencia de la tarea id {idTareaDependencia} con la tarea id {idTarea} del proyecto '{proyecto.Nombre}'.");
    }

    public void AgregarMiembroATarea(Usuario solicitante, int idTarea, int idProyecto, Usuario nuevoMiembro)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.AsignarUsuario(nuevoMiembro);
        NotificarCambio($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(Usuario solicitante, int idTarea, int idProyecto, Usuario miembro)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarUsuario(miembro.Id);
        NotificarCambio($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }
    
    public void AgregarRecursoATarea(Usuario solicitante, int idTarea, int idProyecto, Recurso nuevoRecurso)
    {
        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.AgregarRecurso(nuevoRecurso);
        NotificarCambio($"recurso {nuevoRecurso.Nombre}", idTarea, idProyecto);
    }

    public void EliminarRecursoDeTarea(Usuario solicitante, int idTarea, int idProyecto, Recurso recurso)
    {
        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        ValidarRecursoExistente(recurso, idTarea, idProyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarRecurso(recurso.Id);
        NotificarCambio($"recurso {recurso.Nombre}", idTarea, idProyecto);
    }
    
    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        _gestorProyectos.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        return proyecto;
    }

    private Tarea ObtenerTareaValidandoAdmin(Usuario solicitante, int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        return ObtenerTareaPorId(proyecto.Id, idTarea);
    }

    private void NotificarCambio(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        proyecto.NotificarMiembros($"Se cambió el {campo} de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    private void VerificarEstadoEditablePorUsuario(EstadoTarea estado)
    {
        if (estado != EstadoTarea.EnProceso && estado != EstadoTarea.Completada)
            throw new ExcepcionServicios("No se puede cambiar manualmente a un estado distinto de 'En Proceso' o 'Completada'.");
    }

    private void ValidarRecursoExistente(Recurso recurso, int idTarea, int idProyecto)
    {
        Tarea tarea = ObtenerTareaPorId(idTarea, idProyecto);
        if (!tarea.RecursosNecesarios.Contains(recurso))
            throw new ExcepcionServicios("El recurso no está asignado a la tarea.");
    }

    private void ActualizarEstadosTareasDelProyecto(Proyecto proyecto)
    {
        proyecto.Tareas.ForEach(tarea => tarea.ActualizarEstadoBloqueadaOPendiente());
    }
    
    private void ValidarTareaNoTieneSucesora(Proyecto proyecto, int idTarea)
    {
        if (proyecto.Tareas.Any(tarea => tarea.EsSucesoraDe(idTarea)))
        {
            throw new ExcepcionServicios("No se puede eliminar la tarea porque tiene tareas sucesoras.");
        }
    }

    private void ValidarTareaIniciaDespuesDelProyecto(Proyecto proyecto, Tarea nuevaTarea)
    {
        if (nuevaTarea.FechaInicioMasTemprana < proyecto.FechaInicio)
        {
            throw new ExcepcionServicios("La fecha de inicio de la tarea no puede ser anterior a la fecha de inicio del proyecto.");
        }
    }
}
