using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorProyectos
{
    private IGestorProyectos _gestorProyectos;

    public ControladorProyectos(IGestorProyectos gestor)
    {
        _gestorProyectos = gestor;
    }
}