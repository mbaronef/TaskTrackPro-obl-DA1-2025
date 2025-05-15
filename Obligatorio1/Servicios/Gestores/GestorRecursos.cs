using Dominio;
using Repositorios;
using Servicios.Excepciones;

namespace Servicios.Gestores;

public class GestorRecursos
{
    public RepositorioRecursos Recursos { get; } = new RepositorioRecursos();
    private GestorProyectos _gestorProyectos;

    public GestorRecursos(GestorProyectos gestorProyectos)
    {
        _gestorProyectos = gestorProyectos;
    }

    public void AgregarRecurso(Usuario solicitante, Recurso recurso, bool esExclusivo)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "agregar recursos");
        if (solicitante.EstaAdministrandoUnProyecto && esExclusivo)
        {
            AsociarProyectoQueAdministraARecurso(solicitante, recurso);
        }
        Recursos.Agregar(recurso);
    }

    public Recurso ObtenerRecursoPorId(int idRecurso)
    {
        Recurso recurso = Recursos.ObtenerPorId(idRecurso);
        if (recurso == null)
        {
            throw new ExcepcionServicios("Recurso no existente");
        }
        return recurso;
    }

    public void EliminarRecurso(Usuario solicitante, int idRecurso)
    {
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "eliminar un recurso");
        VerificarRecursoEnUso(recurso, "eliminar");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        Recursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public void ModificarNombreRecurso(Usuario solicitante, int idRecurso, string nuevoNombre)
    {
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de"); 
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        NotificarModificacion(recurso, nombreAnterior);
    }

    public void ModificarTipoRecurso(Usuario solicitante, int idRecurso, string nuevoTipo)
    {
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante,"modificar el tipo de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        recurso.ModificarTipo(nuevoTipo);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(Usuario solicitante, int idRecurso, string nuevaDescripcion)
    {
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la descripción de un recurso");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la descripción de");
        recurso.ModificarDescripcion(nuevaDescripcion);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public List<Recurso> ObtenerRecursosGenerales()
    {
        return Recursos.ObtenerTodos().Where(recurso => !recurso.EsExclusivo()).ToList();
    }
    
    public List<Recurso> ObtenerRecursosExclusivos(int idProyecto)
    {
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorId(idProyecto);
        return Recursos.ObtenerTodos()
            .Where(recurso => recurso.ProyectoAsociado != null && recurso.ProyectoAsociado.Id == idProyecto).ToList();
    }

    private void VerificarPermisoAdminSistemaOAdminProyecto(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema && !usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion}");
        }
    }

    private void AsociarProyectoQueAdministraARecurso(Usuario administradorProyecto, Recurso recurso)
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
            _gestorProyectos.NotificarAdministradoresDeProyectos(_gestorProyectos.Proyectos.ObtenerTodos(), mensaje);
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
        List<Proyecto> proyectosQueUsanElRecurso = _gestorProyectos.Proyectos.ObtenerTodos()
            .Where(proyecto => RecursosNecesariosPorProyecto(proyecto).Contains(recurso)).ToList();
        _gestorProyectos.NotificarAdministradoresDeProyectos(proyectosQueUsanElRecurso, mensaje);
    }
    private List<Recurso> RecursosNecesariosPorProyecto(Proyecto proyecto)
    {
        return proyecto.Tareas.SelectMany(tarea => tarea.RecursosNecesarios).Distinct().ToList();
    }
}
