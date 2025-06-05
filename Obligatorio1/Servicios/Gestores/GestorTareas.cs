using Dominio;
using Dominio.Excepciones;
using Interfaces.InterfacesServicios;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorTareas
{
    private GestorProyectos _gestorProyectos;
    private static int _cantidadTareas;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;


    public GestorTareas(GestorProyectos gestorProyectos, INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _gestorProyectos = gestorProyectos;
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }
    
    public void AgregarTareaAlProyecto(int idProyecto, Usuario solicitante, Tarea nuevaTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ValidarTareaIniciaDespuesDelProyecto(proyecto, nuevaTarea);

        _cantidadTareas++;
        nuevaTarea.Id = _cantidadTareas;
        
        proyecto.AgregarTarea(nuevaTarea);

        _caminoCritico.CalcularCaminoCritico(proyecto);
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.TareaAgregada(nuevaTarea.Id, proyecto.Nombre));
    }

    public void EliminarTareaDelProyecto(int idProyecto, Usuario solicitante, int idTareaAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        proyecto.EliminarTarea(idTareaAEliminar);

        _caminoCritico.CalcularCaminoCritico(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.TareaEliminada(idTareaAEliminar, proyecto.Nombre));
    }
    
    public Tarea ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);
        if (tarea == null)
            throw new ExcepcionTarea(MensajesError.TareaNoExistente);
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
        _caminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(Usuario solicitante, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        _caminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(Usuario solicitante, int idTarea, int idProyecto, EstadoTarea nuevoEstado)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.CambiarEstado(nuevoEstado);
        
        _caminoCritico.CalcularCaminoCritico(proyecto);
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.EstadoTareaModificado(idTarea, proyecto.Nombre, nuevoEstado));

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
            _caminoCritico.CalcularCaminoCritico(proyecto);
        }
        catch (ExcepcionServicios ex)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionTarea(MensajesError.GeneraCiclos);
        }
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.DependenciaAgregada(tarea.Id, proyecto.Nombre, tipoDependencia, tareaDependencia.Id));
    }

    public void EliminarDependenciaDeTarea(Usuario solicitante, int idTarea, int idTareaDependencia, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarDependencia(idTareaDependencia);
        _caminoCritico.CalcularCaminoCritico(proyecto);
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.DependenciaEliminada(idTareaDependencia, idTarea, proyecto.Nombre));
    }

    public void AgregarMiembroATarea(Usuario solicitante, int idTarea, int idProyecto, Usuario nuevoMiembro)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.AsignarUsuario(nuevoMiembro);
        NotificarAgregar($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(Usuario solicitante, int idTarea, int idProyecto, Usuario miembro)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarUsuario(miembro.Id);
        NotificarEliminar($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }
    
    public void AgregarRecursoATarea(Usuario solicitante, int idTarea, int idProyecto, Recurso nuevoRecurso)
    {
        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.AgregarRecurso(nuevoRecurso);
        NotificarAgregar($"recurso {nuevoRecurso.Nombre}", idTarea, idProyecto);
    }

    public void EliminarRecursoDeTarea(Usuario solicitante, int idTarea, int idProyecto, Recurso recurso)
    {
        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        ValidarRecursoExistente(recurso, idTarea, idProyecto);
        
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        tarea.EliminarRecurso(recurso.Id);
        NotificarEliminar($"recurso {recurso.Nombre}", idTarea, idProyecto);
    }
    
    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
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
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.CampoTareaModificado(campo, idTarea, proyecto.Nombre));
    }

    private void NotificarEliminar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.CampoTareaEliminado(campo, idTarea, proyecto.Nombre));
    }
    
    private void NotificarAgregar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros,MensajesNotificacion.CampoTareaAgregado(campo, idTarea, proyecto.Nombre));
    }

    private void VerificarEstadoEditablePorUsuario(EstadoTarea estado)
    {
        if (estado != EstadoTarea.EnProceso && estado != EstadoTarea.Completada)
            throw new ExcepcionTarea(MensajesError.EstadoNoEditable);
    }

    private void ValidarRecursoExistente(Recurso recurso, int idTarea, int idProyecto)
    {
        Tarea tarea = ObtenerTareaPorId(idProyecto, idTarea);
        if (!tarea.RecursosNecesarios.Contains(recurso))
            throw new ExcepcionTarea(MensajesError.RecursoNoAsignado);
    }

    private void ActualizarEstadosTareasDelProyecto(Proyecto proyecto)
    {
        proyecto.Tareas.ForEach(tarea => tarea.ActualizarEstadoBloqueadaOPendiente());
    }
    
    private void ValidarTareaNoTieneSucesora(Proyecto proyecto, int idTarea)
    {
        if (proyecto.Tareas.Any(tarea => tarea.EsSucesoraDe(idTarea)))
        {
            throw new ExcepcionTarea(MensajesError.TareaConSucesoras);
        }
    }

    private void ValidarTareaIniciaDespuesDelProyecto(Proyecto proyecto, Tarea nuevaTarea)
    {
        if (nuevaTarea.FechaInicioMasTemprana < proyecto.FechaInicio)
        {
            throw new ExcepcionTarea(MensajesError.FechaInicioTarea);
        }
    }
}
