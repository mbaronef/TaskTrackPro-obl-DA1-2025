using Dominio;
using Dominio.Dummies;
using Dominio.Excepciones;

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
        if (solicitante.EsAdministradorProyecto)
        {
            if (!recurso.EsExclusivo())
            {
                throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar recursos compartidos");
            }
            else if (recurso.ProyectoAsociado.Equals(_gestorProyectos.ObtenerProyectoPorAdministrador(solicitante.Id)))
            {
                throw new ExcepcionServicios("No tiene los permisos necesarios para eliminar recursos exclusivos de otros proyectos");
            }
        }

        Recursos.Remove(recurso);
    }

    public void ModificarNombreRecurso(Usuario solicitante, int idRecurso, string nuevoNombre)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para modificar el nombre de un recurso");
        }
        Recurso recurso = ObtenerRecursoPorId(idRecurso);
        if (solicitante.EsAdministradorProyecto)
        {
            Proyecto proyectoQueAdministra = _gestorProyectos.ObtenerProyectoPorAdministrador(solicitante.Id);
            if(!recurso.EsExclusivo() || !recurso.ProyectoAsociado.Equals(proyectoQueAdministra))
            {
                throw new ExcepcionServicios("No tiene los permisos necesarios para modificar recursos que no son exclusivos de su proyecto");
            }
        }
        recurso.ModificarNombre(nuevoNombre);
    }

}
