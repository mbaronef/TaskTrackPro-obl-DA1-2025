using Controladores;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorTareasTests
{
    private Mock<IGestorTareas> _mockGestorTareas;
    private ControladorTareas _controladorTareas;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorTareas = new Mock<IGestorTareas>();
        _controladorTareas = new ControladorTareas(_mockGestorTareas.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorTareas);
    }
    
    
    /*métodos a probar:
    GestorTareas.ObtenerTareaPorId()
    GestorTareas.AgregarTareaAlProyecto()
    GestorTareas.CambiarEstadoTarea()
    GestorTareas.ModificarTituloTarea()
    GestorTareas.ModificarDuracionTarea()
    GestorTareas.ModificarDescripcionTarea()
    GestorTareas.EliminarTareaDelProyecto()
    GestorTareas.AgregarDependenciaATarea()
    GestorTareas.EliminarDependenciaDeTarea()
    GestorTareas.AgregarRecursoATarea()
    GestorTareas.EliminarRecursoDeTarea()
    GestorTareas.AgregarMiembroATarea()
    GestorTareas.EliminarMiembroDeTarea()
    */
}