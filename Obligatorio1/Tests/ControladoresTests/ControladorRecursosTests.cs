using Controladores;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorRecursosTests
{
    private Mock<IGestorRecursos> _mockGestorRecursos;
    private ControladorRecursos _controladorRecursos;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorRecursos = new Mock<IGestorRecursos>();
        _controladorRecursos = new ControladorRecursos(_mockGestorRecursos.Object);
    }
    
    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorRecursos);
    }
    
    [TestMethod]
    public void AgregarRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        RecursoDTO nuevoRecurso = new RecursoDTO { Id = 2 };

        _mockGestorRecursos.Setup(g => g.AgregarRecurso(usuario, nuevoRecurso, false));

        _controladorRecursos.AgregarRecurso(usuario, nuevoRecurso, false);

        _mockGestorRecursos.Verify(g => g.AgregarRecurso(usuario, nuevoRecurso, false), Times.Once);
    }
    
    [TestMethod]
    public void EliminarRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecursoAEliminar = 1;

        _mockGestorRecursos.Setup(g => g.EliminarRecurso(usuario, idRecursoAEliminar));

        _controladorRecursos.EliminarRecurso(usuario, idRecursoAEliminar);

        _mockGestorRecursos.Verify(g => g.EliminarRecurso(usuario, idRecursoAEliminar), Times.Once);
    }
    
}