namespace Dominio.Dummies;

public class GestorUsuarios
{
    private static int cantidadUsuarios;

    public void agregarUsuario(Usuario u)
    {
        cantidadUsuarios++;
        u.Id = cantidadUsuarios;
    }
}