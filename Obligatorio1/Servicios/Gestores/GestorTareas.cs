using Dominio;
using Dominio.Excepciones;
using DTOs;
using Repositorios.Interfaces;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorTareas
{
    private GestorProyectos _gestorProyectos;
    private static int _cantidadTareas;
    private IRepositorioUsuarios _repositorioUsuarios;

    public GestorTareas(GestorProyectos gestorProyectos, IRepositorioUsuarios repositorioUsuarios)
    {
        _gestorProyectos = gestorProyectos;
        _repositorioUsuarios = repositorioUsuarios;
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

        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros($"Se agregó la tarea (id {nuevaTarea.Id}) al proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        ValidarTareaNoTieneSucesora(proyecto, idTareaAEliminar);

        proyecto.EliminarTarea(idTareaAEliminar);

        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros($"Se eliminó la tarea (id {idTareaAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }
    
    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        return TareaDTO.DesdeEntidad(tarea);
    }

    public void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarTitulo(nuevoTitulo);
        NotificarCambio("título", idTarea, idProyecto);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarDescripcion(nuevaDescripcion);
        NotificarCambio("descripción", idTarea, idProyecto);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarDuracion(nuevaDuracion);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        CaminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("duración", idTarea, idProyecto);
    }

    public void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Tarea tarea = ObtenerTareaValidandoAdmin(solicitante, idProyecto, idTarea);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        CaminoCritico.CalcularCaminoCritico(proyecto);

        NotificarCambio("fecha de inicio", idTarea, idProyecto);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, EstadoTareaDTO nuevoEstadoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        EstadoTarea nuevoEstado = (EstadoTarea)nuevoEstadoDTO;
        
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(solicitante.Id, proyecto);
        VerificarEstadoEditablePorUsuario(nuevoEstado);
        
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.CambiarEstado(nuevoEstado);
        
        CaminoCritico.CalcularCaminoCritico(proyecto);
        
        proyecto.NotificarMiembros($"Se cambió el estado de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}' a {nuevoEstado}.");

        if (nuevoEstado == EstadoTarea.Completada)
        {
            ActualizarEstadosTareasDelProyecto(proyecto);
        }
    }

    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto, string tipoDependencia)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        Tarea tareaDependencia = ObtenerTareaDominioPorId(idProyecto, idTareaDependencia);
        Dependencia dependencia = new Dependencia(tipoDependencia, tareaDependencia);
        tarea.AgregarDependencia(dependencia);
        try
        {
            CaminoCritico.CalcularCaminoCritico(proyecto);
        }
        catch (ExcepcionServicios ex)
        {
            tarea.EliminarDependencia(dependencia.Tarea.Id);
            throw new ExcepcionServicios("No se puede  agregar la dependencia de la tarea ya que se generarían dependencias cíclicas.");
        }

        proyecto.NotificarMiembros($"Se agregó una dependencia a la tarea id {idTarea} del proyecto '{proyecto.Nombre}' del tipo {tipoDependencia} con la tarea id {tareaDependencia.Id}.");
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.EliminarDependencia(idTareaDependencia);
        CaminoCritico.CalcularCaminoCritico(proyecto);
        proyecto.NotificarMiembros($"Se eliminó la dependencia de la tarea id {idTareaDependencia} con la tarea id {idTarea} del proyecto '{proyecto.Nombre}'.");
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);
        
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(nuevoMiembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AsignarUsuario(nuevoMiembro);
        NotificarAgregar($"miembro {nuevoMiembro.ToString()}", idTarea, idProyecto);
    }

    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Usuario miembro = ObtenerUsuarioPorDTO(miembroDTO);
        
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        _gestorProyectos.VerificarUsuarioMiembroDelProyecto(miembro.Id, proyecto);
        
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.EliminarUsuario(miembro.Id);
        NotificarEliminar($"miembro {miembro.ToString()}", idTarea, idProyecto);
    }
    
    public void AgregarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso nuevoRecurso = nuevoRecursoDTO.AEntidad();
        
        ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
        tarea.AgregarRecurso(nuevoRecurso);
        NotificarAgregar($"recurso {nuevoRecurso.Nombre}", idTarea, idProyecto);
    }

    public void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = recursoDTO.AEntidad();
        
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
        //return tarea.UsuariosAsignados.Any(u => u.Id == usuarioDTO.Id);
    }
    
    public Tarea ObtenerTareaDominioPorId(int idProyecto, int idTarea)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        Tarea tarea = proyecto.Tareas.FirstOrDefault(t => t.Id == idTarea);
        if (tarea == null)
            throw new ExcepcionServicios("Tarea no existente");
        return tarea;
    }
    private Proyecto ObtenerProyectoValidandoAdmin(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        _gestorProyectos.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        _gestorProyectos.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        return proyecto;
    }
    private Tarea ObtenerTareaValidandoAdmin(Usuario solicitante, int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerProyectoValidandoAdmin(idProyecto, solicitante);
        return ObtenerTareaDominioPorId(proyecto.Id, idTarea);
    }

    private void NotificarCambio(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        proyecto.NotificarMiembros($"Se cambió el {campo} de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }

    private void NotificarEliminar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        proyecto.NotificarMiembros($"Se eliminó el {campo} de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }
    
    private void NotificarAgregar(string campo, int idTarea, int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDominioPorId(idProyecto);
        proyecto.NotificarMiembros($"Se agregó el {campo} de la tarea (id {idTarea}) del proyecto '{proyecto.Nombre}'.");
    }

    private void VerificarEstadoEditablePorUsuario(EstadoTarea estado)
    {
        if (estado != EstadoTarea.EnProceso && estado != EstadoTarea.Completada)
            throw new ExcepcionServicios("No se puede cambiar manualmente a un estado distinto de 'En Proceso' o 'Completada'.");
    }

    private void ValidarRecursoExistente(Recurso recurso, int idTarea, int idProyecto)
    {
        Tarea tarea = ObtenerTareaDominioPorId(idProyecto, idTarea);
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
    
    private Usuario ObtenerUsuarioPorDTO(UsuarioDTO usuarioDTO)
    {
        var usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        if (usuario == null)
        {
            throw new ExcepcionServicios($"Usuario no encontrado.");
        }
        return usuario;
    }
}
