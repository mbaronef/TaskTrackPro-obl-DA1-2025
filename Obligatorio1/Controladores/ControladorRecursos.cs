using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorRecursos
{
    private IGestorRecursos _gestorRecursos;
    
    public ControladorRecursos(IGestorRecursos gestorRecursos)
    {
        _gestorRecursos = gestorRecursos;
    }
}