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
}