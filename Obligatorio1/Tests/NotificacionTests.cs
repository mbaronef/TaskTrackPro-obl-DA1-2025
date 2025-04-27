namespace Tests;

using Dominio;
using Dominio.Excepciones;


[TestClass]
[DoNotParallelize]
public class NotificacionTests
{
    private Notificacion _nuevaNotificacion;
    
    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(Notificacion).GetField("_cantidadNotificaciones", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        
        _nuevaNotificacion = new Notificacion("Mensaje de notificación");
    }
    
    [TestMethod]
    public void Constructor_asigna_mensaje_ok()
    {
        Assert.AreEqual("Mensaje de notificación", _nuevaNotificacion.Mensaje);
    }

    [TestMethod]
    public void Se_asigna_fecha_ok()
    {
        Assert.AreEqual(DateTime.Today, _nuevaNotificacion.Fecha);
    }

    [TestMethod]
    public void Se_asigna_ID_ok()
    {
        Assert.AreEqual(1,_nuevaNotificacion.Id);
    }
    
    [TestMethod]
    public void Se_asignan_varios_IDs_ok()
    {
        Notificacion segundaNotificacion = new Notificacion("Mensaje de una segunda notificación");
        Assert.AreEqual(2,segundaNotificacion.Id);
        Notificacion terceraNotificacion = new Notificacion("Mensaje de una tercera notificación");
        Assert.AreEqual(3,terceraNotificacion.Id);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiMensajeEsNull()
    {
        Notificacion notificacion = new Notificacion(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiMensajeEsVacio()
    {
        Notificacion notificacion = new Notificacion("");
    }
    
}
