using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.CaminoCritico;
using Excepciones;
using Excepciones.MensajesError;
using Servicios.Gestores.Interfaces;
using Servicios.Notificaciones;
using Utilidades;

namespace Servicios.Gestores;

public class GestorTareas : IGestorTareas
{
    private readonly IRepositorioProyectos _repositorioProyectos;
    private IRepositorioUsuarios _repositorioUsuarios;
    private IRepositorio<Recurso> _repositorioRecursos;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;

    public GestorTareas(IRepositorioProyectos repositorioProyectos, IRepositorioUsuarios repositorioUsuarios, IRepositorio<Recurso> repositorioRecursos,
        INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _repositorioProyectos = repositorioProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioRecursos = repositorioRecursos;
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }

    public void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea nuevaTarea = nuevaTareaDTO.AEntidad();

        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ValidarTareaIniciaDespuesDelProyecto(proyecto, nuevaTarea);

        proyecto.AgregarTarea(nuevaTarea);

        RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.TareaAgregada(nuevaTarea.Id, proyecto.Nombre));
        
        nuevaTareaDTO.Id = nuevaTarea.Id;
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);

        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        proyecto.EliminarTarea(idTareaAEliminar);

        RecalcularCaminoCriticoYActualizarProyecto(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.TareaEliminada(idTareaAEliminar, proyecto.Nombre));
    }

    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        return TareaDTO.DesdeEntidad(tarea);
    }

    public void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);

        VerificarTareaNoEsteEnProceso(tarea);
        
        tarea.ModificarTitulo(nuevoTitulo);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        
        NotificarCambio("título", idTarea, idProyecto);
    }

    private void VerificarTareaNoEsteEnProceso(Tarea tarea)
    {
        if (tarea.Estado == EstadoTarea.EnProceso)
            throw new ExcepcionTarea(MensajesErrorServicios.TareaEnProceso);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);
        
        tarea.ModificarDescripcion(nuevaDescripcion);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        
        NotificarCambio("descripción", idTarea, idProyecto);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);
        
        tarea.ModificarDuracion(nuevaDuracion);

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        
        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);
        
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        
        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        EstadoTarea nuevoEstado = (EstadoTarea)nuevoEstadoDTO;

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);
        
        VerificarProyectoHayaComenzado(proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(solicitante, tarea, proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);
        tarea.CambiarEstado(nuevoEstado);

        _caminoCritico.CalcularCaminoCritico(proyecto);

        if (nuevoEstado == EstadoTarea.Completada)
        {
            ActualizarEstadosTareasDelProyecto(proyecto); //acá se modifican fechas en base a las recalculadas. Por ello no se puede llamar a RecalcularCaminoCriticoYActualizarProyecto();
        }
        
        _repositorioProyectos.Actualizar(proyecto);
        proyecto.Tareas.ToList().ForEach(tarea => _repositorioProyectos.ActualizarTarea(tarea));
        
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.EstadoTareaModificado(idTarea, proyecto.Nombre, nuevoEstado));
    }
    
    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);

        Tarea tareaDependencia = ObtenerTareaDominioPorId(idProyecto, idTareaDependencia);
        Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);

        tarea.AgregarDependencia(dependencia);

        try
        {
            RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        }
        catch (ExcepcionCaminoCritico)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionTarea(MensajesErrorServicios.GeneraCiclos);
        }
        
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DependenciaAgregada(tarea.Id, proyecto.Nombre, tipoDependencia, tareaDependencia.Id));
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia,
        int idProyecto)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        
        tarea.EliminarDependencia(idTareaDependencia);
        
        RecalcularCaminoCriticoYActualizarProyecto(proyecto);
        
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DependenciaEliminada(idTareaDependencia, idTarea, proyecto.Nombre));
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AsignarUsuario(nuevoMiembro);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        
        NotificarAgregar($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario miembro = ObtenerUsuarioPorDTO(miembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.EliminarUsuario(miembro.Id);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        
        NotificarEliminar($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }

    public void AsignarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso nuevoRecurso = ObtenerRecursoPorDTO(nuevoRecursoDTO);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AsignarRecurso(nuevoRecurso);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        _repositorioRecursos.Actualizar(nuevoRecurso);
        
        NotificarAgregar($"recurso {nuevoRecurso.Nombre}", idTarea, idProyecto);
    }

    public void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoPorDTO(recursoDTO);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        ValidarRecursoExistente(recurso, idTarea, idProyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.EliminarRecurso(recurso.Id);
        
        _repositorioProyectos.ActualizarTarea(tarea);
        _repositorioRecursos.Actualizar(recurso);
        
        NotificarEliminar($"recurso {recurso.Nombre}", idTarea, idProyecto);
    }

    public bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto)
    {
        Usuario usuario = ObtenerUsuarioPorDTO(usuarioDTO);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        return tarea.EsMiembro(usuario);
    }

    private Tarea ObtenerTareaDominioPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);

        if (tarea == null)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaNoExistente);
        }

        return tarea;
    }

    private Proyecto ObtenerProyectoPorId(int idProyecto)
    {
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(idProyecto);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.ProyectoNoEncontrado);
        }
        return proyecto;
    }

    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        
        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        return proyecto;
    }
    
    private Proyecto ObtenerProyectoValidandoAdminOLider(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminOLiderDelProyecto(proyecto, solicitante);
        
        return proyecto;
    }
    
    private Tarea ObtenerTareaValidandoAdminOLider(Usuario solicitante, int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        return ObtenerTareaDominioPorId(proyecto.Id, idTarea);
    }

    private void RecalcularCaminoCriticoYActualizarProyecto(Proyecto proyecto)
    {
        _caminoCritico.CalcularCaminoCritico(proyecto);
        _repositorioProyectos.Actualizar(proyecto);
        proyecto.Tareas.ToList().ForEach(tarea => _repositorioProyectos.ActualizarTarea(tarea));
    }
    private void NotificarCambio(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.CampoTareaModificado(campo, idTarea, proyecto.Nombre));
    }

    private void NotificarEliminar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),

    MensajesNotificacion.CampoTareaEliminado(campo, idTarea, proyecto.Nombre));
    }

    private void NotificarAgregar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.CampoTareaAgregado(campo, idTarea, proyecto.Nombre));
    }

    private void VerificarEstadoEditablePorUsuario(EstadoTarea estado)
    {
        if (estado != EstadoTarea.EnProceso && estado != EstadoTarea.Completada)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.EstadoNoEditable);
        }
    }

    private void ValidarRecursoExistente(Recurso recurso, int idTarea, int idProyecto)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        if (!tarea.RecursosNecesarios.Contains(recurso))
        {
            throw new ExcepcionTarea(MensajesErrorServicios.RecursoNoAsignado);
        }
    }

    private void ActualizarEstadosTareasDelProyecto(Proyecto proyecto)
    {
        proyecto.Tareas.ToList().ForEach(tarea => tarea.ActualizarEstadoBloqueadaOPendiente());
    }

    private void ValidarTareaNoTieneSucesora(Proyecto proyecto, int idTarea)
    {
        if (proyecto.Tareas.Any(tarea => tarea.EsSucesoraDe(idTarea)))
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaConSucesoras);
        }
    }

    private void ValidarTareaIniciaDespuesDelProyecto(Proyecto proyecto, Tarea nuevaTarea)
    {
        if (nuevaTarea.FechaInicioMasTemprana < proyecto.FechaInicio)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.FechaInicioTarea);
        }
    }
    
    private void VerificarProyectoHayaComenzado(Proyecto proyecto)
    {
        if (proyecto.FechaInicio > DateTime.Today)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.ProyectoNoComenzado);
        }
    }

    private Usuario ObtenerUsuarioPorDTO(UsuarioDTO usuarioDTO)
    {
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.UsuarioNoEncontrado);
        }

        return usuario;
    }
    
    private Recurso ObtenerRecursoPorDTO(RecursoDTO recursoDTO)
    {
        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        
        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesErrorServicios.RecursoNoEncontrado);
        }

        return recurso;
    }
    
    
}