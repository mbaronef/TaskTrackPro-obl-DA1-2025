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
        
        if (solicitante.EstaAdministrandoProyecto)
            throw new ExcepcionDominio("El usuario ya está administrando un proyecto.");
        
        if (Proyectos.Any(p => p.Nombre == proyecto.Nombre))
        {
            throw new ExcepcionDominio("Ya existe un proyecto con ese nombre.");
        }

        _cantidadProyectos++;
        proyecto.AsignarId(_cantidadProyectos);
        Proyectos.Add(proyecto);
        
        solicitante.EstaAdministrandoProyecto = true;
        
        proyecto.NotificarMiembros($"Se creó el proyecto '{proyecto.Nombre}'.");
    }
    
    
}