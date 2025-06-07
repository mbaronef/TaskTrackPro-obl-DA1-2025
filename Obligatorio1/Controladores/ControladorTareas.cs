using DTOs;
using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorTareas
{
    private IGestorTareas _gestorTareas;
    public ControladorTareas(IGestorTareas gestorTareas)
    {
        _gestorTareas = gestorTareas;
    }
    
    public void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO)
    {
        _gestorTareas.AgregarTareaAlProyecto(idProyecto, solicitanteDTO, nuevaTareaDTO);
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        _gestorTareas.EliminarTareaDelProyecto(idProyecto, solicitanteDTO, idTareaAEliminar);
    }

    public bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto)
    {
        return _gestorTareas.EsMiembroDeTarea(usuarioDTO, idTarea, idProyecto);
    }

    public void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo)
    {
        _gestorTareas.ModificarTituloTarea(solicitanteDTO, idTarea, idProyecto, nuevoTitulo);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        string nuevaDescripcion)
    {
        _gestorTareas.ModificarDescripcionTarea(solicitanteDTO, idTarea, idProyecto, nuevaDescripcion);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        _gestorTareas.ModificarDuracionTarea(solicitanteDTO, idTarea, idProyecto, nuevaDuracion);
    }
    
    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        _gestorTareas.CambiarEstadoTarea(solicitanteDTO, idTarea, idProyecto, nuevoEstadoDTO);
    }

    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        _gestorTareas.AgregarDependenciaATarea(solicitanteDTO, idTarea, idTareaDependencia, idProyecto, tipoDependencia);
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia,
        int idProyecto)
    {
        _gestorTareas.EliminarDependenciaDeTarea(solicitanteDTO, idTarea, idTareaDependencia, idProyecto);
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        _gestorTareas.AgregarMiembroATarea(solicitanteDTO, idTarea, idProyecto, nuevoMiembroDTO);
    }
    
    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        _gestorTareas.EliminarMiembroDeTarea(solicitanteDTO, idTarea, idProyecto, miembroDTO);
    }
    
    public void AgregarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO)
    {
        _gestorTareas.AgregarRecursoATarea(solicitanteDTO, idTarea, idProyecto, nuevoRecursoDTO);
    }
    
    public void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO)
    {
        _gestorTareas.EliminarRecursoDeTarea(solicitanteDTO, idTarea, idProyecto, recursoDTO);
    }
    
    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        return _gestorTareas.ObtenerTareaPorId(idProyecto, idTarea);
    }
}