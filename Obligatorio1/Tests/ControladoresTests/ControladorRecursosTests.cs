using Controladores;
using DTOs;
using Servicios.Gestores.Interfaces;
using Moq;

namespace Tests.ControladoresTests;

public class ControladorRecursosTests
{
    private Mock<IGestorRecursos> _mockGestorRecursos;
    private ControladorRecursos _controladorRecursos;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorRecursos = new Mock<IGestorRecursos>();
        _controladorRecursos = new ControladorTareas(_mockGestorRecursos.Object);
    }
    
    
    
}