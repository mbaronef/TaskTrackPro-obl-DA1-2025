using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.CaminoCritico;
using Excepciones;
using Excepciones.MensajesError;
using Repositorios;
using Servicios.Gestores.Interfaces;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorProyectos : IGestorProyectos
{
    private readonly IRepositorio<Proyecto> _proyectos;
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly INotificador _notificador;
    private readonly ICalculadorCaminoCritico _caminoCritico;
    private readonly SqlContext _context;

    public GestorProyectos(IRepositorioUsuarios repositorioUsuarios, IRepositorio<Proyecto> repositorioProyectos,
        INotificador notificador, ICalculadorCaminoCritico caminoCritico)
    {
        _proyectos = repositorioProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _notificador = notificador;
        _caminoCritico = caminoCritico;
    }

    public void CrearProyecto(ProyectoDTO proyectoDTO, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        
        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyectoDTO.Nombre);
        
        Proyecto proyecto = proyectoDTO.AEntidad(solicitante);

        _proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;
        
        _repositorioUsuarios.Update(solicitante);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(), MensajesNotificacion.ProyectoCreado(proyecto.Nombre));
        
        proyectoDTO.Id = proyecto.Id;
    }

    public void EliminarProyecto(int idProyecto, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        List<Usuario> miembros = proyecto.Miembros.ToList();
        
        _proyectos.Eliminar(idProyecto);

        solicitante.EstaAdministrandoUnProyecto = false;
        
        _repositorioUsuarios.Update(solicitante);
        
        foreach (Usuario miembro in miembros)
        {
            miembro.CantidadProyectosAsignados--;
        }

        _notificador.NotificarMuchos(miembros, MensajesNotificacion.ProyectoEliminado(proyecto.Nombre));
    }


    public List<ProyectoDTO> ObtenerTodos()
    {
        List<Proyecto> proyectos = _proyectos.ObtenerTodos();
        return proyectos.Select(ProyectoDTO.DesdeEntidad).ToList();
    }

    public List<Proyecto> ObtenerTodosDominio()
    {
        return _proyectos.ObtenerTodos();
    }

    public ProyectoDTO ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(id);
        return ProyectoDTO.DesdeEntidad(proyecto);
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        VerificarNombreNoRepetido(nuevoNombre);

        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);
        
        _proyectos.Update(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.NombreProyectoModificado(nombreAnterior, proyecto.Nombre));
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.ModificarDescripcion(descripcion);
        
        _proyectos.Update(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, proyecto.Descripcion));
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, UsuarioDTO solicitanteDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.ModificarFechaInicio(nuevaFecha);

        _caminoCritico.CalcularCaminoCritico(proyecto);
        
        _proyectos.Update(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha));
    }

    public void CambiarAdministradorDeProyecto(UsuarioDTO solicitanteDTO, int idProyecto, int idNuevoAdmin)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "cambiar el administrador del proyecto");

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);

        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");

        proyecto.Administrador.EstaAdministrandoUnProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoUnProyecto = true;
        
        _proyectos.Update(proyecto);
        _repositorioUsuarios.Update(solicitante);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre, nuevoAdmin.ToString()));
    }

    public void AgregarMiembroAProyecto(int idProyecto, UsuarioDTO solicitanteDTO, UsuarioDTO nuevoMiembroDTO)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Usuario nuevoMiembro = ObtenerUsuarioPorDTO(nuevoMiembroDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        proyecto.AsignarMiembro(nuevoMiembro);
        
        _proyectos.Update(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.MiembroAgregado(proyecto.Nombre, nuevoMiembro.Id));
    }

    public void EliminarMiembroDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idMiembroAEliminar)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);

        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);

        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);

        PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);

        VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembroAEliminar);

        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        _proyectos.Update(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros.ToList(),
            MensajesNotificacion.MiembroEliminado(proyecto.Nombre, idMiembroAEliminar));
    }

    public List<ProyectoDTO> ObtenerProyectosPorUsuario(int idUsuario)
    {
        List<Proyecto> proyectosDelUsuario =
            _proyectos.ObtenerTodos().Where(proyecto => proyecto.EsMiembro(idUsuario)).ToList();
        return proyectosDelUsuario.Select(ProyectoDTO.DesdeEntidad).ToList();
    }

    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = _proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.UsuarioNoAdministraProyectos);
        }

        return proyecto;
    }

    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(idProyecto);
        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        if (proyecto.Tareas.Any(tarea => tarea.EsMiembro(miembroAEliminar)))
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.UsuarioConTareas);
        }
    }

    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        proyectos.ForEach(proyecto => _notificador.NotificarUno(proyecto.Administrador, mensaje));
    }

    public void CalcularCaminoCritico(ProyectoDTO proyectoDTO)
    {
        Proyecto proyecto = ObtenerProyectoDominioPorId(proyectoDTO.Id);
        _caminoCritico.CalcularCaminoCritico(proyecto);
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

        if (proyecto is null)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.ProyectoNoEncontrado);
        }

        return proyecto;
    }

    private Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = _proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
        {
            throw new ExcepcionProyecto(MensajesErrorServicios.NombreRepetido);
        }
    }

    private Usuario ObtenerUsuarioPorDTO(UsuarioDTO usuarioDTO)
    {
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.UsuarioNoEncontrado);
        }

        return usuario;
    }
}