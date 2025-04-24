namespace Dominio;

public class Notificacion
{
    public string Mensaje { get; set; }
    public DateTime Fecha { get; private set; }

    public Notificacion(string mensaje)
    {
        Mensaje = mensaje;
        Fecha = DateTime.Today;
    }
}