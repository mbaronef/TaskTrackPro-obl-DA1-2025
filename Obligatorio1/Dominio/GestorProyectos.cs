using Dominio.Dummies;

namespace Dominio;

public class GestorProyectos
{
    private static int _cantidadProyectos;
    public List<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

    public void CrearProyecto(Proyecto proyecto, Usuario solicitante) // no se si no es mejor un metodo en proyecto que sea asignar id en vez del constructor
    {
        _cantidadProyectos++;
        Proyecto proyectoConId = new Proyecto(_cantidadProyectos, proyecto.Nombre, proyecto.Descripcion, proyecto.Administrador, proyecto.Miembros);
        Proyectos.Add(proyectoConId);
    }
}