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
        Recursos =  new List<Recurso>();
        _gestorProyectos = gestorProyectos;
    }

    public void AgregarRecurso(Usuario solicitante, Recurso recurso)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para agregar recursos");
        }

        if (solicitante.EsAdministradorProyecto)
        {
            Proyecto proyecto = _gestorProyectos.ObtenerProyectoPorAdministrador(solicitante.Id);
            recurso.AsociarAProyecto(proyecto);
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
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar un recurso");
        }
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (recurso.SeEstaUsando())
        {
            throw new ExcepcionServicios("No se puede eliminar un recurso que está en uso");
        }
        if (solicitante.EsAdministradorProyecto && !recurso.EsExclusivo())
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar recursos compartidos");
        }

        Recursos.Remove(recurso);
    }

}