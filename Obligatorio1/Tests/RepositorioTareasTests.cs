using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioTareasTests
{
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
        RepositorioTareas repositorioTareas = new RepositorioTareas();
        Tarea tarea = new Tarea("Título", "Descripción", 2, new DateTime(2030, 2, 2));
        repositorioTareas.Agregar(tarea);
        Assert.AreEqual(tarea, repositorioTareas.ObtenerPorId(tarea.Id));
    }

    [TestMethod]
    public void SeEliminaTareaOk()
    {
        RepositorioTareas repositorioTareas = new RepositorioTareas();
        Tarea tarea = new Tarea("Título", "Descripción", 2, new DateTime(2030, 2, 2));
        repositorioTareas.Agregar(tarea);
        repositorioTareas.Eliminar(tarea.Id);
        Assert.IsNull(repositorioTareas.ObtenerPorId(tarea.Id));
    }
}