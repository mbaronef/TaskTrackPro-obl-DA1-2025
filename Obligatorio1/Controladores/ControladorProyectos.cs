using DTOs;
using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorProyectos
{
    private IGestorProyectos _gestorProyectos;

    public ControladorProyectos(IGestorProyectos gestor)
    {
        _gestorProyectos = gestor;
    }
    
    public void CrearProyecto(ProyectoDTO nuevoProyecto, UsuarioDTO solicitante)
    {
        _gestorProyectos.CrearProyecto(nuevoProyecto, solicitante);
    }
    
    public void EliminarProyecto(int idProyecto, UsuarioDTO solicitante)
    {
        _gestorProyectos.EliminarProyecto(idProyecto, solicitante);
    }
    
    public List<ProyectoDTO> ObtenerTodos()
    {
        return _gestorProyectos.ObtenerTodos();
    }
}