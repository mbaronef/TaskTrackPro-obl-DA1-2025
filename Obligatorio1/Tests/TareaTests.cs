namespace Tests;
using Dominio;

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
    
}