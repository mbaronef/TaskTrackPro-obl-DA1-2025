using Dominio;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class CaminoCriticoTests
{
    [TestMethod]
    public void OrdenTopologicoOrdenaOk()
    {
        Tarea tarea1 = new Tarea("Tarea 1", "desc", 2, DateTime.Today);
        Tarea tarea2 = new Tarea("Tarea 2", "desc", 3, DateTime.Today);
        Tarea tarea3 = new Tarea("Tarea 3", "desc", 4, DateTime.Today);

        tarea2.AgregarDependencia(new Dependencia("SS", tarea1));
        tarea3.AgregarDependencia(new Dependencia("SS", tarea1));
        tarea3.AgregarDependencia(new Dependencia("FS", tarea2));

        List<Tarea> tareas = new List<Tarea> { tarea1, tarea2, tarea3 };
        
        List<Tarea> resultado = CaminoCritico.OrdenarTopologicamente(tareas);
        
        Assert.AreEqual(tareas.Count, resultado.Count);
        Assert.AreEqual(tarea1, resultado.ElementAt(0));
        Assert.AreEqual(tarea3, resultado.ElementAt(1));
        Assert.AreEqual(tarea2, resultado.ElementAt(2));
    }
}