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

    public void AgregarAdministradorSistema(UsuarioDTO solicitante, int id)
    {
        _gestorUsuarios.AgregarAdministradorSistema(solicitante, id);
    }
    
    public void AsignarAdministradorProyecto(UsuarioDTO solicitante, int id)
    {
        _gestorUsuarios.AsignarAdministradorProyecto(solicitante, id);
    }
    
    public void DesasignarAdministradorProyecto(UsuarioDTO solicitante, int id)
    {
        _gestorUsuarios.DesasignarAdministradorProyecto(solicitante, id);
    }

    public void ReiniciarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        _gestorUsuarios.ReiniciarContrasena(solicitanteDTO, idUsuarioObjetivo);
    }
    
    public void AutogenerarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        _gestorUsuarios.AutogenerarContrasena(solicitanteDTO, idUsuarioObjetivo);
    }
    
    public void ModificarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo, string nuevaPass)
    {
        _gestorUsuarios.ModificarContrasena(solicitanteDTO, idUsuarioObjetivo, nuevaPass);
    }

}