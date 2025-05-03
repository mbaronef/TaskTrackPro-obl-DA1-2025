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
    
    [TestMethod]
    public void SeObtieneLaListaDeTareasOk()
    {
        _repositorioTareas.Agregar(_tarea);
        List<Tarea> tareas = _repositorioTareas.ObtenerTodos();
        Assert.IsNotNull(tareas);
        Assert.AreEqual(1, tareas.Count);
        Assert.AreEqual(_tarea, tareas.Last());
    }

    [TestMethod]
    public void SeModificaElTituloDeLaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarTitulo(_tarea.Id, "Otro título");
        Assert.AreEqual("Otro título", _tarea.Titulo);
    }
    
    [TestMethod]
    public void SeModificaLaDescripcionDeLaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarDescripcion(_tarea.Id, "Otra descripción");
        Assert.AreEqual("Otra descripción", _tarea.Descripcion);
    }
}