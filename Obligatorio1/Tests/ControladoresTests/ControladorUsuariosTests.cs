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
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        UsuarioDTO nuevoUsuario = new UsuarioDTO { Id = 2 };

        _mockGestorUsuarios.Setup(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario));

        _controladorUsuarios.CrearYAgregarUsuario(solicitante, nuevoUsuario);

        _mockGestorUsuarios.Verify(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario), Times.Once);
    }

    [TestMethod]
    public void EliminarUsuario_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int id = 1;

        _mockGestorUsuarios.Setup(g => g.EliminarUsuario(solicitante, id));

        _controladorUsuarios.EliminarUsuario(solicitante, id);

        _mockGestorUsuarios.Verify(g => g.EliminarUsuario(solicitante, id), Times.Once);
    }

    [TestMethod]
    public void ObtenerTodos_LlamaCorrectamenteAGestor()
    {
        var listaEsperada = new List<UsuarioListarDTO>
        {
            new UsuarioListarDTO { Id = 1, Nombre = "Usuario A" },
            new UsuarioListarDTO { Id = 2, Nombre = "Usuario B" }
        };

        _mockGestorUsuarios.Setup(g => g.ObtenerTodos()).Returns(listaEsperada);

        List<UsuarioListarDTO> resultado = _controladorUsuarios.ObtenerTodos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Usuario A", resultado[0].Nombre);
        Assert.AreEqual("Usuario B", resultado[1].Nombre);
        _mockGestorUsuarios.Verify(g => g.ObtenerTodos(), Times.Once);

    }

    [TestMethod]
    public void ObtenerUsuarioPorId_LlamaCorrectamenteAGestor()
    {
        int idUsuario = 1;
        UsuarioDTO usuarioEsperado = new UsuarioDTO { Id = idUsuario };

        _mockGestorUsuarios.Setup(g => g.ObtenerUsuarioPorId(idUsuario)).Returns(usuarioEsperado);

        UsuarioDTO resultado = _controladorUsuarios.ObtenerUsuarioPorId(idUsuario);

        Assert.AreEqual(usuarioEsperado.Id, resultado.Id);
        _mockGestorUsuarios.Verify(g => g.ObtenerUsuarioPorId(idUsuario), Times.Once);
    }

    [TestMethod]
    public void AgregarAdministradorSistema_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;
        
        _mockGestorUsuarios.Setup(g => g.AgregarAdministradorSistema(solicitante, nuevoAdmin));

        _controladorUsuarios.AgregarAdministradorSistema(solicitante, nuevoAdmin);
        
        _mockGestorUsuarios.Verify(g => g.AgregarAdministradorSistema(solicitante, nuevoAdmin), Times.Once);
    }
    
    [TestMethod]
    public void AsignarAdministradorProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;
        
        _mockGestorUsuarios.Setup(g => g.AsignarAdministradorProyecto(solicitante, nuevoAdmin));

        _controladorUsuarios.AsignarAdministradorProyecto(solicitante, nuevoAdmin);
        
        _mockGestorUsuarios.Verify(g => g.AsignarAdministradorProyecto(solicitante, nuevoAdmin), Times.Once);
    }

    [TestMethod]
    public void DesasignarAdministradorProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;
        
        _mockGestorUsuarios.Setup(g => g.DesasignarAdministradorProyecto(solicitante, nuevoAdmin));

        _controladorUsuarios.DesasignarAdministradorProyecto(solicitante, nuevoAdmin);
        
        _mockGestorUsuarios.Verify(g => g.DesasignarAdministradorProyecto(solicitante, nuevoAdmin), Times.Once);
    }

    




}