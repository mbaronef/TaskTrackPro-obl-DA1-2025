using DTOs;
using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorUsuarios
{
    private IGestorUsuarios _gestorUsuarios;

    public ControladorUsuarios(IGestorUsuarios gestorUsuarios)
    {
        _gestorUsuarios = gestorUsuarios;
    }

    public void CrearYAgregarUsuario(UsuarioDTO solicitanteDTO, UsuarioDTO nuevoUsuarioDTO)
    {
        _gestorUsuarios.CrearYAgregarUsuario(solicitanteDTO, nuevoUsuarioDTO);
    }

    public void EliminarUsuario(UsuarioDTO solicitanteDTO, int id)
    {
        _gestorUsuarios.EliminarUsuario(solicitanteDTO, id);
    }

    public List<UsuarioListarDTO> ObtenerTodos()
    {
        return _gestorUsuarios.ObtenerTodos();
    }

    public UsuarioDTO ObtenerUsuarioPorId(int idUsuario)
    {
        return _gestorUsuarios.ObtenerUsuarioPorId(idUsuario);
    }

}