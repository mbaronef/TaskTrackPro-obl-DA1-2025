using Controladores;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;

namespace Tests.ControladoresTests;

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
    
}