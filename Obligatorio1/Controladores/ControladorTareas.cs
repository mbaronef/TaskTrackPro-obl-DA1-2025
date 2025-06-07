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
    
    public bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto)
    {
        return _gestorTareas.EsMiembroDeTarea(usuarioDTO, idTarea, idProyecto);
    }

    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        return _gestorTareas.ObtenerTareaPorId(idProyecto, idTarea);
    }

    public void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO)
    {
        _gestorTareas.AgregarTareaAlProyecto(idProyecto, solicitanteDTO, nuevaTareaDTO);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        _gestorTareas.CambiarEstadoTarea(solicitanteDTO, idTarea, idProyecto, nuevoEstadoDTO);
    }
}