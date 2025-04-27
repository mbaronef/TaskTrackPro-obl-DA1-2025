namespace Dominio.Dummies;

public class GestorUsuarios
{
    private static int cantidadUsuarios;
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();

    public void AgregarUsuario(Usuario u)
    {
        cantidadUsuarios++;
        u.Id = cantidadUsuarios;
        Usuarios.Add(u);
    }

    public Usuario ObtenerUsuario(int idUsuario)
    {
        return Usuarios.Find(u => u.Id == idUsuario);
    }
}
