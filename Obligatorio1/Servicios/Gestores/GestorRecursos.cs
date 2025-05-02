using Dominio;
using Dominio.Dummies;

namespace Servicios.Gestores;

public class GestorRecursos
{
    private static int _cantidadRecursos;
    public List<Recurso> Recursos { get; private set; }
    private GestorProyectos _gestorProyectos;

    public GestorRecursos(GestorProyectos gestorProyectos)
    {
        Recursos = new List<Recurso>();
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
        Recursos.Add(recurso);
    }

    public Recurso ObtenerRecursoPorId(int idRecurso)
    {
        Recurso recurso = Recursos.FirstOrDefault(recurso => recurso.Id == idRecurso);
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
        Recursos.Remove(recurso);
        if (recurso.EsExclusivo())
        {
            Usuario adminProyecto = recurso.ProyectoAsociado.Administrador;
            adminProyecto.RecibirNotificacion($"Se eliminó el recurso {recurso.Nombre} de tipo {recurso.Tipo} - {recurso.Descripcion}");
        }
    }

    public void ModificarNombreRecurso(Usuario solicitante, int idRecurso, string nuevoNombre)
    {
        VerificarPermisoAdminSistemaOAdminProyecto(solicitante, "modificar el nombre de un recurso");
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (solicitante.EstaAdministrandoUnProyecto)
        {
            VerificarRecursoExclusivoDelAdministradorProyecto(solicitante, recurso, "modificar el nombre de");
        }
        recurso.ModificarNombre(nuevoNombre);
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
        Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorAdministrador(administradorProyecto.Id);
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
        Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoPorAdministrador(administradorProyecto.Id);
        if (!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
        {
            throw new ExcepcionServicios(
                $"No tiene los permisos necesarios para {accion} recursos que no son exclusivos de su proyecto");
        }
    }
}
