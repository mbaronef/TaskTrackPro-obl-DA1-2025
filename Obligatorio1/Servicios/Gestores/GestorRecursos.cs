using Dominio;
using Repositorios;
using Servicios.Excepciones;

namespace Servicios.Gestores;

public class GestorRecursos
{
    private static int _cantidadRecursos;
    public RepositorioRecursos Recursos { get; } = new RepositorioRecursos();
    private GestorProyectos _gestorProyectos;

    public GestorRecursos(GestorProyectos gestorProyectos)
    {
        _gestorProyectos = gestorProyectos;
    }

    public void AgregarRecurso(Usuario solicitante, Recurso recurso)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "agregar recursos");
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            AsociarProyectoQueAdministraARecurso(solicitante, recurso);
        }
        ++_cantidadRecursos;
        recurso.Id = _cantidadRecursos;
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
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "eliminar");
        }
        Recursos.Eliminar(recurso.Id);
        NotificarEliminacion(recurso);
    }

    public void ModificarNombreRecurso(Usuario solicitante, int idRecurso, string nuevoNombre)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de");
        }
        string nombreAnterior = recurso.Nombre;
        recurso.ModificarNombre(nuevoNombre);
        NotificarModificacion(recurso, nombreAnterior);
    }

    public void ModificarTipoRecurso(Usuario solicitante, int idRecurso, string nuevoTipo)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el tipo de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el tipo de");
        }
        recurso.ModificarTipo(nuevoTipo);
        NotificarModificacion(recurso, recurso.Nombre);
    }

    public void ModificarDescripcionRecurso(Usuario solicitante, int idRecurso, string nuevaDescripcion)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar la descripción de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar la descripción de");
        }
        recurso.ModificarDescripcion(nuevaDescripcion);
        NotificarModificacion(recurso, recurso.Nombre);
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
        Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoDelAdministrador(administradorProyecto.Id);
        if (!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
        {
            throw new ExcepcionServicios(
                $"No tiene los permisos necesarios para {accion} recursos que no son exclusivos de su proyecto");
        }
    }

    private void NotificarEliminacion(Recurso recurso)
    {
        if (recurso.EsExclusivo())
        {
            Usuario adminProyecto = recurso.ProyectoAsociado.Administrador;
            adminProyecto.RecibirNotificacion($"Se eliminó el recurso {recurso.Nombre} de tipo {recurso.Tipo} - {recurso.Descripcion}");
        }
    }

    private void NotificarModificacion(Recurso recurso, string nombreAnterior)
    {
        string mensaje =
            $"El recurso '{nombreAnterior}' ha sido modificado. Nuevos valores: Nombre: '{recurso.Nombre}', Tipo: '{recurso.Tipo}', Descripción: '{recurso.Descripcion}'.";
        if (recurso.EsExclusivo())
        {
            Usuario adminProyecto = recurso.ProyectoAsociado.Administrador;
            adminProyecto.RecibirNotificacion(mensaje);
        }
    }
}
