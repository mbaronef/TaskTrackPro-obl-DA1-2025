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
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "eliminar un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarRecursoEnUso(recurso, "eliminar");
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        Recursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public void ModificarNombreRecurso(Usuario solicitante, int idRecurso, string nuevoNombre)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de");
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        NotificarModificacion(recurso, nombreAnterior);
    }

    public void ModificarTipoRecurso(Usuario solicitante, int idRecurso, string nuevoTipo)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el tipo de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        recurso.ModificarTipo(nuevoTipo);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(Usuario solicitante, int idRecurso, string nuevaDescripcion)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la descripción de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
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
        if (recurso.ProyectoAsociado == null)
        {
            // Es un recurso general
            if (!administradorProyecto.EsAdministradorSistema)
            {
                throw new ExcepcionServicios($"No tiene los permisos necesarios para {accion} recursos generales.");
            }
            return;
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
            NotificarAdministradoresDeTodosLosProyectos(mensaje);
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
    
    private void NotificarAdministradoresDeTodosLosProyectos(string mensaje)
    {
        _gestorProyectos.Proyectos.ObtenerTodos().ForEach(proyecto => proyecto.NotificarAdministrador(mensaje));
    }
    private void NotificarAdministradoresDeProyectosQueUsanRecurso(Recurso recurso, string mensaje)
    {
        List<Proyecto> proyectosQueUsanElRecurso = _gestorProyectos.Proyectos.ObtenerTodos()
            .Where(proyecto => RecursosNecesariosPorProyecto(proyecto).Contains(recurso)).ToList();
        proyectosQueUsanElRecurso.ForEach(proyecto => proyecto.NotificarAdministrador(mensaje));
    }

    private List<Recurso> RecursosNecesariosPorProyecto(Proyecto proyecto)
    {
        return proyecto.Tareas.SelectMany(tarea => tarea.RecursosNecesarios).Distinct().ToList();
    }
}
