using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorProyectos
{
    private IRepositorio<Proyecto> _proyectos;
    private IRepositorioUsuarios _repositorioUsuarios;

    public GestorProyectos(IRepositorioUsuarios repositorioUsuarios, IRepositorio<Proyecto> repositorioProyectos)
    {
        _proyectos = repositorioProyectos;
        _repositorioUsuarios = repositorioUsuarios;
    }

    public void CrearProyecto(ProyectoDTO proyectoDTO, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto = proyectoDTO.AEntidad(solicitante);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyecto.Nombre);
        
        _proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;

        proyecto.NotificarMiembros($"Se creó el proyecto '{proyecto.Nombre}'.");
        
        proyectoDTO.Id = proyecto.Id;
    }

    public void EliminarProyecto(int idProyecto, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto =  ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        solicitante.EstaAdministrandoUnProyecto = false;
        _proyectos.Eliminar(proyecto.Id);
        foreach (Usuario miembro in proyecto.Miembros)
        {
            miembro.CantidadProyectosAsignados--;
        }

        proyecto.NotificarMiembros($"Se eliminó el proyecto '{proyecto.Nombre}'.");
    }
    
    public List<Proyecto> ObtenerTodos()
    {
        return _proyectos.ObtenerTodos();
    }
    public List<ProyectoDTO> ObtenerTodosDTO()
    {
        List<Proyecto> proyectos = _proyectos.ObtenerTodos();
        return proyectos.Select(ProyectoDTO.DesdeEntidad).ToList();
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarNombreNoRepetido(nuevoNombre);
        
        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);

        proyecto.NotificarMiembros($"Se cambió el nombre del proyecto '{nombreAnterior}' a '{proyecto.Nombre}'.");
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto =  ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarDescripcion(descripcion);

        proyecto.NotificarMiembros($"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{proyecto.Descripcion}'.");
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto =  ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaInicio(nuevaFecha);
        
        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros($"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.");
    }

    public void CambiarAdministradorDeProyecto(UsuarioDTO solicitanteDTO, int idProyecto, int idNuevoAdmin)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        VerificarUsuarioEsAdminSistema(solicitante);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);
        
        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");
        
        proyecto.Administrador.EstaAdministrandoUnProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoUnProyecto = true;
        
        proyecto.NotificarMiembros($"Se cambió el administrador del proyecto '{proyecto.Nombre}'. El nuevo administrador es '{nuevoAdmin}'.");
    }

    public void AgregarMiembroAProyecto(int idProyecto, UsuarioDTO solicitanteDTO, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);
        
        Proyecto proyecto =  ObtenerProyectoDominioPorId(idProyecto);

        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AsignarMiembro(nuevoMiembro);

        proyecto.NotificarMiembros($"Se agregó a un nuevo miembro (id {nuevoMiembro.Id}) al proyecto '{proyecto.Nombre}'.");

    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idMiembroAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        Proyecto proyecto =  ObtenerProyectoDominioPorId(idProyecto);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);

        VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembroAEliminar);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        proyecto.NotificarMiembros($"Se eliminó a el miembro (id {idMiembroAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }

    public List<ProyectoDTO> ObtenerProyectosPorUsuario(int idUsuario)
    {
        List<Proyecto> proyectosDelUsuario = _proyectos.ObtenerTodos().Where(proyecto => proyecto.EsMiembro(idUsuario)).ToList();
        return proyectosDelUsuario.Select(ProyectoDTO.DesdeEntidad).ToList();
    }
    
    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = _proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);

        if (proyecto == null)
        {
            throw new ExcepcionServicios("No se encontró un proyecto administrado por ese usuario.");
        }

        return proyecto;
    }
    
    public ProyectoDTO ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(id);
        return ProyectoDTO.DesdeEntidad(proyecto);
    }
    
    public void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
        {
            throw new ExcepcionServicios("Solo el administrador del proyecto puede realizar esta acción.");
        } 
    }

    public void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, string tipoUsuario)
    {
        if (!solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios($"El {tipoUsuario} no tiene los permisos de administrador de proyecto.");
        }
    }

    public void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);

        if (usuario is null)
        {
            throw new ExcepcionServicios("El usuario no es miembro del proyecto.");
        }
    }
    
    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        if (proyecto.Tareas.Any(tarea => tarea.EsMiembro(miembroAEliminar)))
        {
            throw new ExcepcionServicios("El usuario tiene tareas asignadas");
        }
    }

    public Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }
    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        proyectos.ForEach(proyecto => proyecto.NotificarAdministrador(mensaje));
    }

    public ProyectoDTO CalcularCaminoCriticoYActualizarDTO(ProyectoDTO proyectoDTO)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(proyectoDTO.Id);
        CaminoCritico.CalcularCaminoCritico(proyecto);
        return ProyectoDTO.DesdeEntidad(proyecto);
    }

    public bool EsAdministradorDeProyecto(UsuarioDTO usuarioDTO, int idProyecto)
    {
        Usuario usuario = ObtenerUsuarioPorDTO(usuarioDTO);
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        return proyecto.EsAdministrador(usuario);
    }
    
    public bool EsMiembroDeProyecto(int idUsuario, int idProyecto)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        return proyecto.EsMiembro(idUsuario);
    }
    public Proyecto ObtenerProyectoDominioPorId(int id)
    {
        Proyecto proyecto = _proyectos.ObtenerPorId(id);
        
        if(proyecto is null)
        {
            throw new ExcepcionServicios("El proyecto no existe.");
        }

        return proyecto;
    }
    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = _proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
        {
            throw new ExcepcionServicios($"Ya existe un proyecto con el nombre '{nuevoNombre}'.");
        }
    }

    private void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios("El usuario ya está administrando un proyecto.");
        }
    }

    private void VerificarUsuarioEsAdminSistema(Usuario usuario)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionServicios("El solicitante no es administrador de sistema.");
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