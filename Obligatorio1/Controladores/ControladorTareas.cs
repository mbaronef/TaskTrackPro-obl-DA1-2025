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
}