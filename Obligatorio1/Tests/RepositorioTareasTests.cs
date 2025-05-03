using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioTareasTests
{
    private RepositorioTareas _repositorioTareas;
    private Tarea _tarea;

    [TestInitialize]
    public void SetUp()
    {
        _repositorioTareas = new RepositorioTareas();
        _tarea = new Tarea("Título", "Descripción", 2, new DateTime(2030, 2, 2));   
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioTareas repositorioTareas = new RepositorioTareas();
        Tarea tarea = repositorioTareas.ObtenerPorId(1);
        Assert.IsNull(tarea);
    }

    [TestMethod]
    public void SeAgregaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Assert.AreEqual(_tarea, _repositorioTareas.ObtenerPorId(_tarea.Id));
    }

    [TestMethod]
    public void SeEliminaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.Eliminar(_tarea.Id);
        Assert.IsNull(_repositorioTareas.ObtenerPorId(_tarea.Id));
    }
}