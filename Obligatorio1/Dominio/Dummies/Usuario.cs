namespace Dominio.Dummies;

public class Usuario
{
    public int Id { get; set; }
    public List<Notificacion>  Notificaciones { get; set; }  = new List<Notificacion>();

    public void RecibirNotificacion(Notificacion notificacion)
    {
        Notificaciones.Add(notificacion);
    }
    
    public Usuario(string nombre, string apellido, string mail, string password)
    { }

    public Usuario(){}
}