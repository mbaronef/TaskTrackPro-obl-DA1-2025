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
    
    [TestMethod]
    public void ObtenerRecursoPorId_LlamaCorrectamenteAGestor()
    {
        int idRecurso = 1; 
        RecursoDTO recursoEsperado = new RecursoDTO { Id = idRecurso };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursoPorId(idRecurso)).Returns(recursoEsperado);

        RecursoDTO resultado = _controladorRecursos.ObtenerRecursoPorId(idRecurso);

        Assert.AreEqual(recursoEsperado.Id, resultado.Id);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursoPorId(idRecurso), Times.Once);
    }

    [TestMethod]
    public void ObtenerRecursosGenerales_LlamaCorrectamenteAGestor()
    {
        var listaEsperada = new List<RecursoDTO>{
            new RecursoDTO { Id = 1, Nombre = "Recurso A" },
            new RecursoDTO { Id = 2, Nombre = "Recurso B" }
        };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursosGenerales()).Returns(listaEsperada);

        List<RecursoDTO> resultado = _controladorRecursos.ObtenerRecursosGenerales();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recurso A", resultado[0].Nombre);
        Assert.AreEqual("Recurso B", resultado[1].Nombre);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursosGenerales(), Times.Once);
        
    }
}