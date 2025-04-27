namespace Dominio;

public class GestorUsuario
{
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();
    public List<Usuario> AdministradoresSistema { get; private set; } = new List<Usuario>();

    public GestorUsuario()
    {
    }

    public void AgregarUsuario(Usuario usuario)
    {
        Usuarios.Add(usuario);
    }

}