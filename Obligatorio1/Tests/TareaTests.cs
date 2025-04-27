using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace Tests;
using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

[TestClass]

public class TareaTests
{
    [TestMethod]

    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        string titulo = "Tarea";
        string descripcion = "Prueba de tarea";
        int duracionEnDias = 8;
        
        Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias, fechaInicioEstimada);
        
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracionEnDias, tarea.DuracionEnDias);
        Assert.AreEqual(fechaInicioEstimada, tarea.FechaInicioMasTemprana);
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado); // Estado por defecto
        Assert.IsNotNull(tarea.UsuariosAsignados);
        Assert.IsNotNull(tarea.DependenciasFF);
        Assert.IsNotNull(tarea.DependenciasFS);
        Assert.IsNotNull(tarea.RecursosNecesarios);

    }

    [TestMethod]
    public void Constructor_SinFechaInicioEsNull()
    {
        string titulo = "Tarea";
        string descripcion = "Prueba de tarea";
        int duracionEnDias = 8;
        
        Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias);
        
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracionEnDias, tarea.DuracionEnDias);
        Assert.IsNull(tarea.FechaInicioMasTemprana); 
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
        Assert.IsNotNull(tarea.UsuariosAsignados);
        Assert.IsNotNull(tarea.RecursosNecesarios);
        Assert.IsNotNull(tarea.DependenciasFF);
        Assert.IsNotNull(tarea.DependenciasFS);
        
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsVacio()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("", "Descripción válida", 8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea(null, "Descripción válida",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsEspacio()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea(" ", "Descripción válida",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsNegativa()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", -3, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsCero()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", 0, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", "", 8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", null,  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsEspacio()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", " ",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiFechaInicioEsMenorQueHoy()
    {
        DateTime fechaInicioAnterior = DateTime.Today.AddDays(-1); 

        Tarea tarea = new Tarea("Titulo", "Descripción válida", 5, fechaInicioAnterior);
    }
    
    [TestMethod]
    public void CambiarEstadoDePendienteAFinalizada()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        
        Assert.AreEqual(EstadoTarea.Completada, tarea.Estado);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAPendienteDesdeCompletada()
    {
        var tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    public void EsCriticaDevuelveTrueCuandoHolguraEsCero()
    {
        Tarea tarea = new Tarea("Tarea Crítica", "Descripción", 5);
        
        tarea.Holgura = 0;
        bool esCritica = tarea.EsCritica();
        
        Assert.IsTrue(esCritica);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveTrueSiElUsuarioEstaAsignado()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuario = new Usuario("nombre", "apellido", "mail@ejemplo.com", "password");
        tarea.UsuariosAsignados.Add(usuario);
        
        bool resultado = tarea.EsMiembro(usuario);
        
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveFalseSiElUsuarioNoEstaAsignado()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuarioNoAsignado = new Usuario("nombre", "apellido", "mail@ejemplo.com", "password");
        
        bool resultado = tarea.EsMiembro(usuarioNoAsignado);
        
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EliminarUsuarioAsignadoEliminaUsuarioDeLaLista()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuario = new Usuario("hola", "hola", "hola", "hola") { Id = 1 };
        tarea.UsuariosAsignados.Add(usuario);
        
        tarea.EliminarUsuarioAsignado(usuario.Id);
        
        Assert.IsFalse(tarea.UsuariosAsignados.Contains(usuario));
        }
}
    


