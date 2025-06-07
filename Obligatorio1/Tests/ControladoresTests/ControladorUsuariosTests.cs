using Controladores;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorUsuariosTests
{
    private Mock<IGestorUsuarios> _mockGestorUsuarios;
    private ControladorUsuarios _controladorUsuarios;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorUsuarios = new Mock<IGestorUsuarios>();
        _controladorUsuarios = new ControladorUsuarios(_mockGestorUsuarios.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorUsuarios);
    }
    
    [TestMethod]
    public void CrearYAgregarUsuario_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        UsuarioDTO nuevoUsuario = new UsuarioDTO { Id = 2 };
        
        _mockGestorUsuarios.Setup(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario));

        _controladorTareas.CrearYAgregarUsuario(solicitante, nuevoUsuario);

        _mockGestorUsuarios.Verify(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario), Times.Once);
    }
    
    
    
    
    
    
    
}