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

}