namespace Dominio.Dummies;

public class Usuario
{
    private static int _contadorId = 0;
    public int Id { get; set; }
    public bool  EsAdministradorProyecto { get; set; }
    
    public bool EsAdministradorSistema { get; set; }
    
    public bool EstaAdministrandoProyecto { get; set; }
    public List<Notificacion>  Notificaciones { get; }  = new List<Notificacion>();

    public void RecibirNotificacion(string mensaje)
    {
        Notificacion notificacion = new Notificacion(mensaje);
        Notificaciones.Add(notificacion);
    }

    public Usuario()
    {
        Id = _contadorId++;
    }
}