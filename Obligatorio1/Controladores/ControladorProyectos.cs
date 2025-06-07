using Dominio;
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
    
    public ProyectoDTO ObtenerProyectoPorId(int idProyecto)
    {
        return _gestorProyectos.ObtenerProyectoPorId(idProyecto);
    }
    
    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, UsuarioDTO solicitante)
    {
        _gestorProyectos.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante);
    }
    
    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, UsuarioDTO solicitante)
    {
        _gestorProyectos.ModificarDescripcionDelProyecto(idProyecto, descripcion, solicitante);
    }
    
    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, UsuarioDTO solicitante)
    {
        _gestorProyectos.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante);
    }
    
    public void CambiarAdministradorDeProyecto(UsuarioDTO solicitante, int idProyecto, int idNuevoAdmin)
    {
        _gestorProyectos.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin);
    }
    
    public void AgregarMiembroAProyecto(int idProyecto, UsuarioDTO solicitante, UsuarioDTO nuevoMiembro)
    {
        _gestorProyectos.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro);
    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, UsuarioDTO solicitante, int idMiembro)
    {
        _gestorProyectos.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro);
    }
    
    public List<ProyectoDTO> ObtenerProyectosPorUsuario(int idUsuario)
    {
        return _gestorProyectos.ObtenerProyectosPorUsuario(idUsuario);
    }
    
    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        return _gestorProyectos.ObtenerProyectoDelAdministrador(idAdministrador);
    }
    
    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembro)
    {
        _gestorProyectos.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembro);
    }
    
    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        _gestorProyectos.NotificarAdministradoresDeProyectos(proyectos, mensaje);
    }
    
    public void CalcularCaminoCritico(ProyectoDTO proyecto)
    {
        _gestorProyectos.CalcularCaminoCritico(proyecto);
    }
    
    public bool EsAdministradorDeProyecto(UsuarioDTO usuario, int idProyecto)
    {
        return _gestorProyectos.EsAdministradorDeProyecto(usuario, idProyecto);
    }
    
    public bool EsMiembroDeProyecto(int idUsuario, int idProyecto)
    {
        return _gestorProyectos.EsMiembroDeProyecto(idUsuario, idProyecto);
    }
}