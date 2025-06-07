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
    
    [TestMethod]
    public void ObtenerTodos_LlamaCorrectamenteAGestorYDevuelveLista()
    {
        var listaEsperada = new List<ProyectoDTO>
        {
            new ProyectoDTO { Id = 1, Nombre = "Proyecto A" },
            new ProyectoDTO { Id = 2, Nombre = "Proyecto B" }
        };

        _mockGestorProyectos.Setup(g => g.ObtenerTodos()).Returns(listaEsperada);

        var resultado = _controladorProyectos.ObtenerTodos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Proyecto A", resultado[0].Nombre);
        Assert.AreEqual("Proyecto B", resultado[1].Nombre);
        _mockGestorProyectos.Verify(g => g.ObtenerTodos(), Times.Once);
    }
    
    [TestMethod]
    public void ObtenerProyectoPorId_LlamaCorrectamenteAGestorYDevuelveProyecto()
    {
        int idProyecto = 1;
        var proyectoEsperado = new ProyectoDTO { Id = idProyecto, Nombre = "Proyecto Test" };

        _mockGestorProyectos.Setup(g => g.ObtenerProyectoPorId(idProyecto)).Returns(proyectoEsperado);

        var resultado = _controladorProyectos.ObtenerProyectoPorId(idProyecto);

        Assert.AreEqual(idProyecto, resultado.Id);
        Assert.AreEqual("Proyecto Test", resultado.Nombre);
        _mockGestorProyectos.Verify(g => g.ObtenerProyectoPorId(idProyecto), Times.Once);
    }
    
    [TestMethod]
    public void ModificarNombreDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        var solicitante = new UsuarioDTO { Id = 1 };
        string nuevoNombre = "Nuevo Proyecto";

        _mockGestorProyectos.Setup(g => g.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante));

        _controladorProyectos.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante), Times.Once);
    }
    
    [TestMethod]
    public void ModificarDescripcionDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        var solicitante = new UsuarioDTO { Id = 1 };
        string nuevaDescripcion = "Descripción actualizada";

        _mockGestorProyectos.Setup(g => g.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante));

        _controladorProyectos.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante), Times.Once);
    }
    
    [TestMethod]
    public void ModificarFechaDeInicioDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        var solicitante = new UsuarioDTO { Id = 1 };
        DateTime nuevaFecha = DateTime.Today.AddDays(5);

        _mockGestorProyectos.Setup(g => g.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante));

        _controladorProyectos.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante), Times.Once);
    }
    
    [TestMethod]
    public void CambiarAdministradorDeProyecto_LlamaCorrectamenteAGestor()
    {
        var solicitante = new UsuarioDTO { Id = 1 };
        int idProyecto = 10;
        int idNuevoAdmin = 99;

        _mockGestorProyectos.Setup(g => g.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin));

        _controladorProyectos.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin);

        _mockGestorProyectos.Verify(g => g.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin), Times.Once);
    }
}