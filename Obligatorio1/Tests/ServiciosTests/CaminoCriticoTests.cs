using Dominio;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class CaminoCriticoTests
{
    private Tarea _tarea1;
    private Tarea _tarea2;
    private Tarea _tarea3;
    
    [TestInitialize]
    public void SetUp()
    {
        _tarea1 = new Tarea("Tarea 1", "desc", 2, DateTime.Today);
        _tarea2 = new Tarea("Tarea 2", "desc", 3, DateTime.Today.AddDays(3));
        _tarea3 = new Tarea("Tarea 3", "desc", 4, DateTime.Today.AddDays(4));
        
        _tarea1.Id = 1; // por simplicidad de tests, harcodeamos ids en vez de usar el repositorio
        _tarea2.Id = 2;
        _tarea3.Id = 3;

        _tarea2.AgregarDependencia(new Dependencia("SS", _tarea1));
        _tarea2.AgregarDependencia(new Dependencia("SS", _tarea3));
        _tarea3.AgregarDependencia(new Dependencia("FS", _tarea1));
    }

    [TestMethod]
    public void OrdenTopologicoOrdenaOk()
    {
        List<Tarea> tareas = new List<Tarea> { _tarea1, _tarea2, _tarea3 };
        
        List<Tarea> resultado = CaminoCritico.OrdenarTopologicamente(tareas);
        
        Assert.AreEqual(tareas.Count, resultado.Count);
        Assert.AreEqual(_tarea1.Titulo, resultado.ElementAt(0).Titulo);
        Assert.AreEqual(_tarea3.Titulo, resultado.ElementAt(1).Titulo);
        Assert.AreEqual(_tarea2.Titulo, resultado.ElementAt(2).Titulo);
    }

    [TestMethod]
    public void SeObtienenSucesorasPorTarea()
    {
        List<Tarea> tareas = new List<Tarea> { _tarea1, _tarea2, _tarea3 };

        Dictionary<Tarea, List<Tarea>> sucesoras = CaminoCritico.ObtenerSucesorasPorTarea(tareas);
        
        Assert.AreEqual(tareas.Count, sucesoras.Count);
        Assert.AreEqual(2, sucesoras[_tarea1].Count);
        Assert.IsTrue(sucesoras[_tarea1].Contains(_tarea2));
        Assert.IsTrue(sucesoras[_tarea1].Contains(_tarea3));
    }

    [TestMethod]
    public void CalculaCorrectamenteFechasDeUnaTarea()
    {
        List<Tarea> tareas = new List<Tarea> { _tarea1, _tarea2, _tarea3 };
        
        CaminoCritico.CalcularFechasMasTempranas(tareas);
        
        Assert.AreEqual(_tarea1.FechaInicioMasTemprana, DateTime.Today);
        Assert.AreEqual(_tarea3.FechaInicioMasTemprana, DateTime.Today.AddDays(2));
        
        Assert.AreEqual(_tarea2.FechaInicioMasTemprana, DateTime.Today.AddDays(6));
    }
}