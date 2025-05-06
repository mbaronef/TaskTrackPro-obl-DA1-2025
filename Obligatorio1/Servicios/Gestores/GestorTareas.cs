using Dominio;
namespace Servicios.Gestores;

public class GestorTareas
{
    private GestorProyectos _gestorProyectos;

    public GestorTareas(GestorProyectos gestorProyectos)
    {
        _gestorProyectos = gestorProyectos;
    }
}