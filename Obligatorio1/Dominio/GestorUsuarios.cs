namespace Dominio;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();
    public List<Usuario> AdministradoresSistema { get; private set; } = new List<Usuario>();

    public GestorUsuarios()
    {
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _cantidadUsuarios++;
        usuario.Id = _cantidadUsuarios;
        Usuarios.Add(usuario);
    }

    public void EliminarUsuario(int id)
    {
        Usuarios.RemoveAll(u => u.Id == id);
    }

    public Usuario ObtenerUsuario(int idUsuario)
    {
        return Usuarios.Find(u => u.Id == idUsuario);
    }

}