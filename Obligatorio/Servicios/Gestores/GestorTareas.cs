using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.CaminoCritico;
using Excepciones;
using Excepciones.MensajesError;
using Servicios.Gestores.Interfaces;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorTareas : IGestorTareas
{
    private static int _cantidadTareas;
    
    private readonly IGestorProyectos _gestorProyectos;
    private IRepositorioUsuarios _repositorioUsuarios;
    private IRepositorio<Recurso> _repositorioRecursos;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;

    public GestorTareas(IGestorProyectos gestorProyectos, IRepositorioUsuarios repositorioUsuarios, IRepositorio<Recurso> repositorioRecursos,
        INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _gestorProyectos = gestorProyectos;
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

        _cantidadTareas++;
        nuevaTarea.Id = _cantidadTareas;

        proyecto.AgregarTarea(nuevaTarea);

        _caminoCritico.CalcularCaminoCritico(proyecto);
        
        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.TareaAgregada(nuevaTarea.Id, proyecto.Nombre));
        
        nuevaTareaDTO.Id = nuevaTarea.Id;
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        proyecto.EliminarTarea(idTareaAEliminar);

        _caminoCritico.CalcularCaminoCritico(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros,
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
        
        tarea.ModificarTitulo(nuevoTitulo);
        NotificarCambio("título", idTarea, idProyecto);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);
        
        tarea.ModificarDescripcion(nuevaDescripcion);
        NotificarCambio("descripción", idTarea, idProyecto);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdminOLider(solicitante, idProyecto, idTarea);
        
        tarea.ModificarDuracion(nuevaDuracion);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _caminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _caminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        EstadoTarea nuevoEstado = (EstadoTarea)nuevoEstadoDTO;

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.CambiarEstado(nuevoEstado);

        _caminoCritico.CalcularCaminoCritico(proyecto);
        
        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.EstadoTareaModificado(idTarea, proyecto.Nombre, nuevoEstado));

        if (nuevoEstado == EstadoTarea.Completada)
        {
            ActualizarEstadosTareasDelProyecto(proyecto);
        }
    }

    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        
        Tarea tareaDependencia = ObtenerTareaDominioPorId(idProyecto, idTareaDependencia);
        Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);
        
        tarea.AgregarDependencia(dependencia);
        
        try
        {
            _caminoCritico.CalcularCaminoCritico(proyecto);
        }
        catch (ExcepcionCaminoCritico)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionTarea(MensajesErrorServicios.GeneraCiclos);
        }

        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.DependenciaAgregada(tarea.Id, proyecto.Nombre, tipoDependencia, tareaDependencia.Id));
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia,
        int idProyecto)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        
        tarea.EliminarDependencia(idTareaDependencia);
        
        _caminoCritico.CalcularCaminoCritico(proyecto);
        
        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.DependenciaEliminada(idTareaDependencia, idTarea, proyecto.Nombre));
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AsignarUsuario(nuevoMiembro);
        
        NotificarAgregar($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario miembro = ObtenerUsuarioPorDTO(miembroDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.EliminarUsuario(miembro.Id);
        NotificarEliminar($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }

    public void AsignarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso nuevoRecurso = ObtenerRecursoPorDTO(nuevoRecursoDTO);

        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);

        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AsignarRecurso(nuevoRecurso);
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
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);

        if (tarea == null)
        {
            throw new ExcepcionTarea(MensajesErrorServicios.TareaNoExistente);
        }

        return tarea;
    }

    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        
        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        return proyecto;
    }
    
    private Proyecto ObtenerProyectoValidandoAdminOLider(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminOLiderDelProyecto(proyecto, solicitante);
        
        return proyecto;
    }

    private Tarea ObtenerTareaValidandoAdmin(Usuario solicitante, int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        return ObtenerTareaDominioPorId(proyecto.Id, idTarea);
    }
    
    private Tarea ObtenerTareaValidandoAdminOLider(Usuario solicitante, int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdminOLider(idProyecto, solicitante);
        return ObtenerTareaDominioPorId(proyecto.Id, idTarea);
    }
    

    private void NotificarCambio(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.CampoTareaModificado(campo, idTarea, proyecto.Nombre));
    }

    private void NotificarEliminar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros,
            MensajesNotificacion.CampoTareaEliminado(campo, idTarea, proyecto.Nombre));
    }

    private void NotificarAgregar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _notificador.NotificarMuchos(proyecto.Miembros,
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
        proyecto.Tareas.ForEach(tarea => tarea.ActualizarEstadoBloqueadaOPendiente());
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