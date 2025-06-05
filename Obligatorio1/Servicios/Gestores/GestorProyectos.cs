using Dominio;
using DTOS_;
using Interfaces.InterfacesServicios;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorProyectos
{
    public RepositorioProyectos Proyectos { get; } = new RepositorioProyectos();
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;

    
    public GestorProyectos(INotificador notificador, ICalculadorCaminoCritico  caminoCritico)
    {
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }

    public void CrearProyecto(Proyecto proyecto, Usuario solicitante)
    {
        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuariosServicio.VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyecto.Nombre);
        
        Proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.ProyectoCreado(proyecto.Nombre));
    }

    public void EliminarProyecto(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        solicitante.EstaAdministrandoUnProyecto = false;
        Proyectos.Eliminar(proyecto.Id);
        foreach (Usuario miembro in proyecto.Miembros)
        {
            miembro.CantidadProyectosAsignados--;
        }

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.ProyectoEliminado(proyecto.Nombre));
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarNombreNoRepetido(nuevoNombre);
        
        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.NombreProyectoModificado(nombreAnterior, proyecto.Nombre));
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarDescripcion(descripcion);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, proyecto.Descripcion));
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaInicio(nuevaFecha);
        
        _caminoCritico.CalcularCaminoCritico(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha));
    }

    public void CambiarAdministradorDeProyecto(Usuario solicitante, int idProyecto, int idNuevoAdmin)
    {
        PermisosUsuariosServicio.VerificarPermisoAdminSistema(solicitante, "cambiar el administrador del proyecto");

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);
        
        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        PermisosUsuariosServicio.VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");
        
        proyecto.Administrador.EstaAdministrandoUnProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoUnProyecto = true;
        
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre, nuevoAdmin.ToString()));
    }

    public void AgregarMiembroAProyecto(int idProyecto, Usuario solicitante, Usuario nuevoMiembro)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AsignarMiembro(nuevoMiembro);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.MiembroAgregado(proyecto.Nombre, nuevoMiembro.Id));

    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, Usuario solicitante, int idMiembroAEliminar)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);
        
        PermisosUsuariosServicio.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuariosServicio.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        PermisosUsuariosServicio.VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);

        VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembroAEliminar);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.MiembroEliminado(proyecto.Nombre, idMiembroAEliminar));
    }

    public List<Proyecto> ObtenerProyectosPorUsuario(int idUsuario)
    {
        return Proyectos.ObtenerTodos().Where(proyecto => proyecto.Miembros.Any(usuario => usuario.Id == idUsuario)).ToList();
    }
    
    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = Proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesError.UsuarioNoAdministraProyectos);
        }

        return proyecto;
    }
    
    public Proyecto ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = Proyectos.ObtenerPorId(id);
        
        if(proyecto is null)
        {
            throw new ExcepcionPermisos(MensajesError.ProyectoNoEncontrado);
        }
        return proyecto;
    }
    
    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        if (proyecto.Tareas.Any(tarea => tarea.EsMiembro(miembroAEliminar)))
        {
            throw new ExcepcionProyecto(MensajesError.UsuarioConTareas);
        }
    }

    public Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }
    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        proyectos.ForEach(proyecto => _notificador.NotificarUno(proyecto.Administrador, mensaje));
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = Proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
        {
            throw new ExcepcionProyecto(MensajesError.NombreRepetido);
        }
    }
    
    public List<ProyectoDTO> ObtenerTodosDTO()
    {
        return Proyectos.ObtenerTodos().Select(ProyectoDTO.DesdeEntidad).ToList();
    }
    
    public List<ProyectoDTO> ObtenerProyectosPorUsuarioDTO(int idUsuario)
    {
        return ObtenerProyectosPorUsuario(idUsuario).Select(ProyectoDTO.DesdeEntidad).ToList();
    }
    
    public void CrearProyectoDesdeDTO(ProyectoDTO dto, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = solicitanteDTO.AEntidad(); 
        Proyecto proyecto = dto.ANuevaEntidad(solicitante);
        CrearProyecto(proyecto, solicitante);
    }
    
    public ProyectoDTO ObtenerProyectoPorIdDTO(int id)
    {
        Proyecto proyecto = ObtenerProyectoPorId(id); 
        return ProyectoDTO.DesdeEntidad(proyecto);
    }
}