using Controladores;

namespace Tests.ControladoresTests;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;


[TestClass]
public class ControladorProyectosTests
{
    private Mock<IGestorProyectos> _mockGestorProyectos;
    private ControladorProyectos _controladorProyectos;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorProyectos = new Mock<IGestorProyectos>();
        _controladorProyectos = new ControladorProyectos(_mockGestorProyectos.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorProyectos);
    }


    [TestMethod]
    public void CrearProyecto_LlamaCorrectamenteAlGestor()
    {
        var solicitante = new UsuarioDTO { Id = 1 };
        var nuevoProyecto = new ProyectoDTO { Nombre = "Proyecto Test" };

        _mockGestorProyectos.Setup(g => g.CrearProyecto(nuevoProyecto, solicitante));

        _controladorProyectos.CrearProyecto(nuevoProyecto, solicitante);

        _mockGestorProyectos.Verify(g => g.CrearProyecto(nuevoProyecto, solicitante), Times.Once);
    }
    
    [TestMethod]
    public void EliminarProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        var solicitante = new UsuarioDTO { Id = 1 };

        _mockGestorProyectos.Setup(g => g.EliminarProyecto(idProyecto, solicitante));

        _controladorProyectos.EliminarProyecto(idProyecto, solicitante);

        _mockGestorProyectos.Verify(g => g.EliminarProyecto(idProyecto, solicitante), Times.Once);
    }
}