namespace Dominio;

public class Notificacion
{
    public string Mensaje { get; set; }

    public Notificacion(string mensaje)
    {
        Mensaje = mensaje;
    }
}