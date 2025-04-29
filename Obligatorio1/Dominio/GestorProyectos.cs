using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class GestorProyectos
{
    private static int _cantidadProyectos;
    public List<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

    public void CrearProyecto(Proyecto proyecto, Usuario solicitante)
    {
        if (!solicitante.EsAdministradorProyecto)
            throw new ExcepcionDominio("El usuario no tiene permiso para crear un proyecto.");

        _cantidadProyectos++;
        proyecto.AsignarId(_cantidadProyectos);
        Proyectos.Add(proyecto);
    }
    
    
}