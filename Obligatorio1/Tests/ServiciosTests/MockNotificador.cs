using Dominio;
using InterfacesServicios;

public class MockNotificador : INotificador
{
    public List<(Usuario, string)> Notificaciones = new();

    public void NotificarUno(Usuario usuario, string mensaje)
    {
        Notificaciones.Add((usuario, mensaje));
        usuario.RecibirNotificacion(mensaje);
    }

    public void NotificarMuchos(List<Usuario> usuarios, string mensaje)
    {
        usuarios.ForEach(u =>
            Notificaciones.Add((u, mensaje)));
        usuarios.ForEach(u => u.RecibirNotificacion(mensaje));
    }
}