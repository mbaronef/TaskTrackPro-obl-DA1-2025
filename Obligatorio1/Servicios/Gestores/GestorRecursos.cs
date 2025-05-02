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
        return Recursos.FirstOrDefault(recurso => recurso.Id == idRecurso);
    }

}