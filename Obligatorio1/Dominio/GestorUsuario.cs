namespace Dominio;

public class GestorUsuario
{
    private static int _cantidadUsuarios;
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();
    public List<Usuario> AdministradoresSistema { get; private set; } = new List<Usuario>();

    public GestorUsuario()
    {
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _cantidadUsuarios++;
        usuario.Id = _cantidadUsuarios;
        Usuarios.Add(usuario);
    }
    
    public Usuario ObtenerUsuario(int idUsuario)
    {
        return Usuarios.Find(u => u.Id == idUsuario);
    }

}