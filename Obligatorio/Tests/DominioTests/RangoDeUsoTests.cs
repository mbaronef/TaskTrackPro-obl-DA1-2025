using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RangoDeUsoTests
{
    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RangoDeUso rango = new RangoDeUso();
        Assert.IsNotNull(rango);
    }
    
    [TestMethod]
    public void ConstructorConParametros_CreaRangoCorrectamente()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        int capacidadEnUso = 10;

        Tarea tareaQueHaceUso = new Tarea();
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, capacidadEnUso, tareaQueHaceUso);
        Assert.AreEqual(0, rango.Id);
        Assert.AreEqual(fechaInicio, rango.FechaInicio);
        Assert.AreEqual(fechaFin, rango.FechaFin);
        Assert.AreEqual(capacidadEnUso, rango.CantidadDeUsos);
        Assert.AreEqual(tareaQueHaceUso, rango.Tarea);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiFechaInicioEsMayorQueFechaFin()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(10);
        DateTime fechaFin = DateTime.Today;
        Tarea tareaQueHaceUso = new Tarea();
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 10, tareaQueHaceUso);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiCantidadDeUsosEs0()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        Tarea tareaQueHaceUso = new Tarea();
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 0, tareaQueHaceUso);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiCantidadDeUsosEsNegativa()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        Tarea tareaQueHaceUso = new Tarea();
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, -2, tareaQueHaceUso);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiTareaEsNull()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        Tarea tareaQueHaceUso = null;
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 2, tareaQueHaceUso);
    }
}