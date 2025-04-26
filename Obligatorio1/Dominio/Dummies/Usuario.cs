namespace Dominio.Dummies;

public class Usuario
{
    public int Id { get; set; }
    public List<Notificacion>  Notificaciones { get; set; }  = new List<Notificacion>();

    public void AgregarNotificacion(Notificacion notificacion)
    {
        Notificaciones.Add(notificacion);
    }
}