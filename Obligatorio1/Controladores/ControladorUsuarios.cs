using Servicios.Gestores;

namespace Controladores;

public class ControladorUsuarios
{
    private GestorUsuarios _gestorUsuarios;
    public ControladorUsuarios(GestorUsuarios gestor)
    {
        _gestorUsuarios = gestor;
    }
}