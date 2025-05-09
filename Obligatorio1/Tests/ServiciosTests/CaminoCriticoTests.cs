using Dominio;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Tests.ServiciosTests;

[TestClass]
public class CaminoCriticoTests
{
    private Proyecto _proyecto;
    private Tarea _tarea1;
    private Tarea _tarea2;
    private Tarea _tarea3;
    private Tarea _tarea4;
    private DateTime _fechaHoy = DateTime.Today;

    [TestInitialize]
    public void SetUp()
    {
        _tarea1 = new Tarea("Tarea 1", "desc", 2, _fechaHoy);
        _tarea2 = new Tarea("Tarea 2", "desc", 3, _fechaHoy.AddDays(3));
        _tarea3 = new Tarea("Tarea 3", "desc", 4, _fechaHoy.AddDays(4));
        _tarea4 = new Tarea("Tarea 4", "desc", 1, _fechaHoy.AddDays(5));

        _tarea1.Id = 1; // por simplicidad de tests, harcodeamos ids en vez de usar el repositorio
        _tarea2.Id = 2;
        _tarea3.Id = 3;
        _tarea4.Id = 4;

        _tarea2.AgregarDependencia(new Dependencia("SS", _tarea1));
        _tarea2.AgregarDependencia(new Dependencia("FS", _tarea3));
        _tarea3.AgregarDependencia(new Dependencia("FS", _tarea1));

        _proyecto = new Proyecto("Nombre", "desc", _fechaHoy,
            new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "email@email.com", "Contra5e{a"),
            new List<Usuario>());

        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);
    }

    [TestMethod]
    public void CalculaCaminoCriticoOk()
    {
        CaminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsTrue(_tarea1.EsCritica());
        Assert.IsTrue(_tarea2.EsCritica());
        Assert.IsTrue(_tarea3.EsCritica());
        Assert.IsFalse(_tarea4.EsCritica());
        Assert.AreEqual(8, _tarea4.Holgura);

        Assert.AreEqual(_fechaHoy, _tarea1.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy.AddDays(2), _tarea3.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy.AddDays(6), _tarea2.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy, _tarea4.FechaInicioMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoSeCalculaCaminoCriticoSiHayCiclos()
    {
        _tarea3.AgregarDependencia(new Dependencia("SS", _tarea2));
        CaminoCritico.CalcularCaminoCritico(_proyecto);
    }
    
    [TestMethod]
    public void CalcularCaminoCriticoEnProyectoSinTareasNoLanzaExcepcion()
    {
        Proyecto proyectoVacio = new Proyecto("Vacío", "sin tareas", _fechaHoy,
            new Usuario("Nombre", "Apellido", new DateTime(1990, 1, 1), "a@b.com", "Clave123"), new List<Usuario>());

        CaminoCritico.CalcularCaminoCritico(proyectoVacio);
        Assert.IsTrue(true); // No se lanzó excepción
    }

    [TestMethod]
    public void VariasTareasIndependientesCaminoCriticoOk()
    {
        Tarea tarea5 = new Tarea("Tarea 5", "desc", 2, _fechaHoy) { Id = 5 };
        Tarea tarea6 = new Tarea("Tarea 6", "desc", 1, _fechaHoy) { Id = 6 };

        _proyecto.AgregarTarea(tarea5);
        _proyecto.AgregarTarea(tarea6);

        CaminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsFalse(tarea5.EsCritica());
        Assert.IsFalse(tarea6.EsCritica());
    }
    
    [TestMethod]
    public void TareaSinDependenciasFueraDelCaminoCriticoTieneHolgura()
    {
        CaminoCritico.CalcularCaminoCritico(_proyecto);
        Assert.IsTrue(_tarea4.Holgura > 0);
    }
}