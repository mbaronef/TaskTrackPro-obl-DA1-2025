using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorTareas
{
    private IGestorTareas _gestorTareas;
    public ControladorTareas(IGestorTareas gestorTareas)
    {
        _gestorTareas = gestorTareas;
    }
}