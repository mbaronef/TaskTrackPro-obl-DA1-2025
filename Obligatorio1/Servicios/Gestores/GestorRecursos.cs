using Dominio;
using DTOs;
using Repositorios.Interfaces;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorRecursos
{
    private IRepositorio<Recurso> _repositorioRecursos;
    private GestorProyectos _gestorProyectos;
    private IRepositorioUsuarios _repositorioUsuarios;
    private readonly INotificador _notificador;

    public GestorRecursos(
        IRepositorio<Recurso> repositorioRecursos,
        GestorProyectos gestorProyectos,
        IRepositorioUsuarios repositorioUsuarios, INotificador notificador)
    {
        _repositorioRecursos = repositorioRecursos;
        _gestorProyectos = gestorProyectos;
        _repositorioUsuarios = repositorioUsuarios;
        _notificador = notificador;
    }
    public void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO recursoDTO, bool esExclusivo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = recursoDTO.AEntidad();
        PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "agregar recursos");
        if (solicitante.EstaAdministrandoUnProyecto && esExclusivo)
        {
            AsociarRecursoAProyectoQueAdministra(solicitante, recurso);
        }
        _repositorioRecursos.Agregar(recurso);
        recursoDTO.Id = recurso.Id;
    }
    
    public void EliminarRecurso(UsuarioDTO solicitanteDTO, int idRecurso)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "eliminar un recurso");
        VerificarRecursoEnUso(recurso);
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        _repositorioRecursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public RecursoDTO ObtenerRecursoPorId(int idRecurso)
    {
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        return RecursoDTO.DesdeEntidad(recurso);
    }
    
    public List<RecursoDTO> ObtenerRecursosGenerales()
    {
        List<Recurso> recursosGenerales = _repositorioRecursos.ObtenerTodos().Where(recurso => !recurso.EsExclusivo()).ToList();
        return recursosGenerales.Select(RecursoDTO.DesdeEntidad).ToList();
    }
    
    public List<RecursoDTO> ObtenerRecursosExclusivos(int idProyecto)
    {
        List<RecursoDTO> todosLosRecursos = _repositorioRecursos.ObtenerTodos().Select(RecursoDTO.DesdeEntidad).ToList();
        return todosLosRecursos.Where(recurso => recurso.ProyectoAsociado != null && recurso.ProyectoAsociado.Id == idProyecto).ToList();
    }

    public void ModificarNombreRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoNombre)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de"); 
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        NotificarModificacion(recurso, nombreAnterior);
    }

    public void ModificarTipoRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoTipo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(solicitante,"modificar el tipo de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        recurso.ModificarTipo(nuevoTipo);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        PermisosUsuariosServicio.VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la descripción de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la descripción de");
        recurso.ModificarDescripcion(nuevaDescripcion);
        NotificarModificacion(recurso, recurso.Nombre);
    }
    public RecursoDTO ObtenerRecursoExclusivoPorId(int idProyecto, int idRecurso)
    {
        List<RecursoDTO> recursosExclusivos = ObtenerRecursosExclusivos(idProyecto);
        RecursoDTO recurso = recursosExclusivos.FirstOrDefault(recurso => recurso.Id == idRecurso);
        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesError.RecursoNoEncontrado);
        }
        return recurso;
    }

    private Recurso ObtenerRecursoDominioPorId(int idRecurso)
    {
        Recurso recurso = _repositorioRecursos.ObtenerPorId(idRecurso);
        if (recurso == null)
        {
            throw new ExcepcionRecurso(MensajesError.RecursoNoEncontrado);
        }
        return recurso;
    }
    
    private void AsociarRecursoAProyectoQueAdministra(Usuario administradorProyecto, Recurso recurso)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        recurso.AsociarAProyecto(proyecto);
    }

    private void VerificarRecursoEnUso(Recurso recurso)
    {
        if (recurso.SeEstaUsando())
        {
            throw new ExcepcionRecurso(MensajesError.RecursoEnUso); 
        }
    }
    
    private void VerificarRecursoExclusivoDelAdministradorProyecto(Usuario administradorProyecto, Recurso recurso,
        string accion)
    {
        if (administradorProyecto.EsAdministradorSistema)
        {
            return;
        }

        if (recurso.ProyectoAsociado == null)
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPara($"{accion} recursos generales."));
        }

        Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        if (!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPara( $"{accion} recursos que no son exclusivos de su proyecto"));
        }
    }

    private void NotificarEliminacion(Recurso recurso)
    {
        if (recurso.EsExclusivo())
        {
            _notificador.NotificarUno(recurso.ProyectoAsociado.Administrador, MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion));
        }
        else
        { // decidimos que si el recurso no es exclusivo, se notifica a todos los admins de todos los proyectos.
            _gestorProyectos.NotificarAdministradoresDeProyectos(_gestorProyectos.ObtenerTodosDominio(), MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion));
        }
    }

    private void NotificarModificacion(Recurso recurso, string nombreAnterior)
    {
        string mensaje = MensajesNotificacion.RecursoModificado(nombreAnterior, recurso.ToString());
        if (recurso.EsExclusivo())
        {
            _notificador.NotificarUno(recurso.ProyectoAsociado.Administrador, mensaje);
        }
        else
        { 
            NotificarAdministradoresDeProyectosQueUsanRecurso(recurso, mensaje);
        }
    }
    private void NotificarAdministradoresDeProyectosQueUsanRecurso(Recurso recurso, string mensaje)
    {
        List<Proyecto> proyectosQueUsanElRecurso = _gestorProyectos.ObtenerTodosDominio()
            .Where(proyecto => RecursosNecesariosPorProyecto(proyecto).Contains(recurso)).ToList();
        _gestorProyectos.NotificarAdministradoresDeProyectos(proyectosQueUsanElRecurso, mensaje);
    }
    private List<Recurso> RecursosNecesariosPorProyecto(Proyecto proyecto)
    {
        return proyecto.Tareas.SelectMany(tarea => tarea.RecursosNecesarios).Distinct().ToList();
    }
    
    private Usuario ObtenerUsuarioPorDTO(UsuarioDTO usuarioDTO)
    {
        var usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesError.UsuarioNoEncontrado);
        }
        return usuario;
    }
}