namespace Dominio.Dummies;

public class Notificacion
{
    private static int _cantidadNotificaciones = 0;
    
    public int Id { get; private set; }
    public string Mensaje { get; set; }
    public DateTime Fecha { get; private set; }

    public Notificacion(string mensaje)
    {
        Mensaje = mensaje;
        Fecha = DateTime.Today;
        Id = ++_cantidadNotificaciones;
    }
}