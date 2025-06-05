using Dominio;
using DTOs;
using Repositorios;
using Repositorios.Interfaces;
using Servicios.Excepciones;

namespace Servicios.Gestores;

public class GestorRecursos
{
    private IRepositorio<Recurso> _repositorioRecursos;
    private GestorProyectos _gestorProyectos;
    private IRepositorioUsuarios _repositorioUsuarios;

    public GestorRecursos(
        IRepositorio<Recurso> repositorioRecursos,
        GestorProyectos gestorProyectos,
        IRepositorioUsuarios repositorioUsuarios)
    {
        _repositorioRecursos = repositorioRecursos;
        _gestorProyectos = gestorProyectos;
        _repositorioUsuarios = repositorioUsuarios;
    }
    public void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO recursoDTO, bool esExclusivo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = recursoDTO.AEntidad();
        
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "agregar recursos");
        if (solicitante.EstaAdministrandoUnProyecto && esExclusivo)
        {
            AsociarRecursoAProyectoQueAdministra(solicitante, recurso);
        }
        _repositorioRecursos.Agregar(recurso);
        recursoDTO.Id = recurso.Id;
    }

    public RecursoDTO ObtenerRecursoPorId(int idRecurso)
    {
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        return RecursoDTO.DesdeEntidad(recurso);
    }

    public void EliminarRecurso(UsuarioDTO solicitanteDTO, int idRecurso)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "eliminar un recurso");
        VerificarRecursoEnUso(recurso, "eliminar");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        _repositorioRecursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public void ModificarNombreRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoNombre)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de"); 
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        NotificarModificacion(recurso, nombreAnterior);
    }

    public void ModificarTipoRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevoTipo)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante,"modificar el tipo de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        recurso.ModificarTipo(nuevoTipo);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(UsuarioDTO solicitanteDTO, int idRecurso, string nuevaDescripcion)
    {
        Usuario solicitante = ObtenerUsuarioPorDTO(solicitanteDTO);
        Recurso recurso = ObtenerRecursoDominioPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la descripción de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la descripción de");
        recurso.ModificarDescripcion(nuevaDescripcion);
        NotificarModificacion(recurso, recurso.Nombre);
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

    public RecursoDTO ObtenerRecursoExclusivoPorId(int idProyecto, int idRecurso)
    {
        List<RecursoDTO> recursosExclusivos = ObtenerRecursosExclusivos(idProyecto);
        return recursosExclusivos.FirstOrDefault(recurso => recurso.Id == idRecurso);
    }

    private Recurso ObtenerRecursoDominioPorId(int idRecurso)
    {
        Recurso recurso = _repositorioRecursos.ObtenerPorId(idRecurso);
        if (recurso == null)
        {
            throw new ExcepcionServicios("Recurso no existente");
        }
        return recurso;
    }
    private void VerificarPermisoAdminSistemaOAdminProyecto(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema && !usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion}");
        }
    }

    private void AsociarRecursoAProyectoQueAdministra(Usuario administradorProyecto, Recurso recurso)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        recurso.AsociarAProyecto(proyecto);
    }

    private void VerificarRecursoEnUso(Recurso recurso, string accion)
    {
        if (recurso.SeEstaUsando())
        {
            throw new ExcepcionServicios($"No se puede {accion} un recurso que está en uso");
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
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion} recursos generales.");
        }

        Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        if (!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
        {
            throw new ExcepcionServicios(
                $"No tiene los permisos necesarios para {accion} recursos que no son exclusivos de su proyecto");
        }
    }

    private void NotificarEliminacion(Recurso recurso)
    {
        string mensaje = $"Se eliminó el recurso {recurso.Nombre} de tipo {recurso.Tipo} - {recurso.Descripcion}";
        if (recurso.EsExclusivo())
        {
            recurso.ProyectoAsociado.NotificarAdministrador(mensaje);
        }
        else
        { // decidimos que si el recurso no es exclusivo, se notifica a todos los admins de todos los proyectos.
            _gestorProyectos.NotificarAdministradoresDeProyectos(_gestorProyectos.ObtenerTodos(), mensaje);
        }
    }

    private void NotificarModificacion(Recurso recurso, string nombreAnterior)
    {
        string mensaje =
            $"El recurso '{nombreAnterior}' ha sido modificado. Nuevos valores: {recurso.ToString()}";
        if (recurso.EsExclusivo())
        {
            recurso.ProyectoAsociado.NotificarAdministrador(mensaje);
        }
        else
        { 
            NotificarAdministradoresDeProyectosQueUsanRecurso(recurso, mensaje);
        }
    }
    private void NotificarAdministradoresDeProyectosQueUsanRecurso(Recurso recurso, string mensaje)
    {
        List<Proyecto> proyectosQueUsanElRecurso = _gestorProyectos.ObtenerTodos()
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
            throw new ExcepcionServicios($"Usuario no encontrado.");
        }
        return usuario;
    }
}
