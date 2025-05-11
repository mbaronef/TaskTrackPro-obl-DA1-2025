using Dominio.Excepciones;

namespace Dominio;

public class Notificacion
{
    private static int _cantidadNotificaciones = 0;
    
    public int Id { get; private set; }
    public string Mensaje { get; private set; }
    public DateTime Fecha { get; private set; }

    public Notificacion()
    {
    }

    public Notificacion(string mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ExcepcionDominio("El mensaje de la notificación no puede estar vacío o null.");

        Mensaje = mensaje;
        Fecha = DateTime.Today;
        Id = ++_cantidadNotificaciones;
    }
}