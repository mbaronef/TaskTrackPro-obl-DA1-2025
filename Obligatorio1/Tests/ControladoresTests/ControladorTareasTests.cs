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

    [TestMethod]
    public void AgregarTareaAlProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        TareaDTO nuevaTarea = new TareaDTO { Id = 2 };

        _mockGestorTareas.Setup(g => g.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea));

        _controladorTareas.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea);

        _mockGestorTareas.Verify(g => g.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea), Times.Once);
    }
    
    [TestMethod]
    public void EliminarTareaDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTareaAEliminar = 1;

        _mockGestorTareas.Setup(g => g.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar));

        _controladorTareas.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar);

        _mockGestorTareas.Verify(g => g.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar), Times.Once);
    }

    [TestMethod]
    public void ObtenerTareaPorId_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        int idTarea = 1;
        TareaDTO tareaEsperada = new TareaDTO { Id = idTarea };

        _mockGestorTareas.Setup(g => g.ObtenerTareaPorId(idProyecto, idTarea)).Returns(tareaEsperada);

        TareaDTO resultado = _controladorTareas.ObtenerTareaPorId(idProyecto, idTarea);

        Assert.AreEqual(tareaEsperada.Id, resultado.Id);
        _mockGestorTareas.Verify(g => g.ObtenerTareaPorId(idProyecto, idTarea), Times.Once);
    }
    
    [TestMethod]
    public void ModificarTituloTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        string nuevoTitulo = "Nuevo título";

        _mockGestorTareas.Setup(g => g.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo));

        _controladorTareas.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo);

        _mockGestorTareas.Verify(g => g.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo), Times.Once);
    }
    
    [TestMethod]
    public void ModificarDescripcionTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        string nuevaDescripcion = "Nueva descripción";

        _mockGestorTareas.Setup(g => g.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion));

        _controladorTareas.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion);

        _mockGestorTareas.Verify(g => g.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion), Times.Once);
    }
    
    [TestMethod]
    public void ModificarDuracionTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        int nuevaDuracion = 5;

        _mockGestorTareas.Setup(g => g.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion));

        _controladorTareas.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion);

        _mockGestorTareas.Verify(g => g.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion), Times.Once);
    }
    
    [TestMethod]
    public void CambiarEstadoTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        EstadoTareaDTO nuevoEstado = EstadoTareaDTO.EnProceso;
        
        _mockGestorTareas.Setup(g => g.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado));

        _controladorTareas.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado);

        _mockGestorTareas.Verify(g => g.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado), Times.Once);
    }
    
    [TestMethod]
    public void EsMiembroDeTarea_LLamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;

        _mockGestorTareas.Setup(g => g.EsMiembroDeTarea(usuario, idTarea, idProyecto)).Returns(true);

        bool resultado = _controladorTareas.EsMiembroDeTarea(usuario, idTarea, idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorTareas.Verify(g => g.EsMiembroDeTarea(usuario, idTarea, idProyecto), Times.Once);
    }
    
    
    
    /*métodos a probar:
    GestorTareas.EliminarTareaDelProyecto()
    GestorTareas.AgregarDependenciaATarea()
    GestorTareas.EliminarDependenciaDeTarea()
    GestorTareas.AgregarRecursoATarea()
    GestorTareas.EliminarRecursoDeTarea()
    GestorTareas.AgregarMiembroATarea()
    GestorTareas.EliminarMiembroDeTarea()
    */
}