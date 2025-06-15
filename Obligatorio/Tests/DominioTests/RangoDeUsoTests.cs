using Dominio;

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
        Assert.AreEqual(fechaInicio, rango.FechaInicio);
        Assert.AreEqual(fechaFin, rango.FechaFin);
        Assert.AreEqual(capacidadEnUso, rango.CantidadDeUsos);
        Assert.AreEqual(tareaQueHaceUso, rango.TareaQueHaceUso);
    }
}