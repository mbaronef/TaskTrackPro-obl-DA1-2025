namespace Tests;

using Dominio;

[TestClass]
public class NotificacionTests
{
    [TestMethod]
    public void Constructor_asigna_mensaje_ok()
    {
        Notificacion nuevaNotificacion = new Notificacion("Mensaje de notificación");
        Assert.AreEqual("Mensaje de notificación", nuevaNotificacion.Mensaje);
    }

    [TestMethod]
    public void Se_asigna_fecha_ok()
    {
        Notificacion nuevaNotificacion = new Notificacion("Mensaje de notificación");
        Assert.AreEqual(DateTime.Today, nuevaNotificacion.Fecha);
    }

    [TestMethod]
    public void Se_asigna_ID_ok()
    {
        Notificacion nuevaNotificacion = new Notificacion("Mensaje de notificación");
        Assert.AreEqual(1,nuevaNotificacion.Id);
    }
    
    [TestMethod]
    public void Se_asignan_varios_IDs_ok()
    {
        Notificacion nuevaNotificacion = new Notificacion("Mensaje de notificación");
        Assert.AreEqual(1,nuevaNotificacion.Id);
        Notificacion segundaNotificacion = new Notificacion("Mensaje de una segunda notificación");
        Assert.AreEqual(2,segundaNotificacion.Id);
        Notificacion terceraNotificacion = new Notificacion("Mensaje de una tercera notificación");
        Assert.AreEqual(3,terceraNotificacion.Id);
    }
}
