using Dominio;

namespace Servicios.Notificaciones;

public class Notificador : INotificador
{
    public void NotificarUno(Usuario usuario, string mensaje)
    {
        usuario.RecibirNotificacion(mensaje);
    }

    public void NotificarMuchos(List<Usuario> usuarios, string mensaje)
    {
        usuarios.ForEach(u => u.RecibirNotificacion(mensaje));
    }
}