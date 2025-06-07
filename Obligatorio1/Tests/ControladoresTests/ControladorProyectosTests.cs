using System.Runtime.InteropServices.JavaScript;
using Controladores;
using Dominio;

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
    
    [TestMethod]
    public void AgregarMiembroAProyecto_LlamaCorrectamenteAGestor()
    {
        var solicitante = new UsuarioDTO { Id = 1 };
        var nuevoMiembro = new UsuarioDTO { Id = 2 };
        int idProyecto = 5;

        _mockGestorProyectos.Setup(g => g.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro));

        _controladorProyectos.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro);

        _mockGestorProyectos.Verify(g => g.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro), Times.Once);
    }
    
    [TestMethod]
    public void EliminarMiembroDelProyecto_LlamaCorrectamenteAGestor()
    {
        var solicitante = new UsuarioDTO { Id = 1 };
        int idProyecto = 3;
        int idMiembro = 4;

        _mockGestorProyectos.Setup(g => g.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro));

        _controladorProyectos.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro);

        _mockGestorProyectos.Verify(g => g.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro), Times.Once);
    }
    
    [TestMethod]
    public void ObtenerProyectosPorUsuario_LlamaCorrectamenteAGestorYDevuelveLista()
    {
        int idUsuario = 1;
        var proyectosEsperados = new List<ProyectoDTO>
        {
            new ProyectoDTO { Id = 1, Nombre = "P1" },
            new ProyectoDTO { Id = 2, Nombre = "P2" }
        };

        _mockGestorProyectos.Setup(g => g.ObtenerProyectosPorUsuario(idUsuario)).Returns(proyectosEsperados);

        var resultado = _controladorProyectos.ObtenerProyectosPorUsuario(idUsuario);

        Assert.AreEqual(2, resultado.Count);
        _mockGestorProyectos.Verify(g => g.ObtenerProyectosPorUsuario(idUsuario), Times.Once);
    }
    
    [TestMethod]
    public void ObtenerProyectoDelAdministrador_LlamaCorrectamenteAGestorYDevuelveProyecto()
    {
        Usuario admin = new Usuario();
        Proyecto proyectoEsperado = new Proyecto("P1", "descripcion", DateTime.Today.AddYears(1),admin,  new List<Usuario>());

        _mockGestorProyectos.Setup(g => g.ObtenerProyectoDelAdministrador(admin.Id)).Returns(proyectoEsperado);

        var resultado = _controladorProyectos.ObtenerProyectoDelAdministrador(admin.Id);

        Assert.AreEqual(admin.Id, resultado.Id);
        Assert.AreEqual("P1", resultado.Nombre);
        _mockGestorProyectos.Verify(g => g.ObtenerProyectoDelAdministrador(admin.Id), Times.Once);
    }
    
    [TestMethod]
    public void VerificarUsuarioNoTieneTareasAsignadas_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        int idUsuario = 2;

        _mockGestorProyectos.Setup(g => g.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario));

        _controladorProyectos.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario);

        _mockGestorProyectos.Verify(g => g.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario), Times.Once);
    }
    
    [TestMethod]
    public void NotificarAdministradoresDeProyectos_LlamaCorrectamenteAGestor()
    {
        Usuario admin = new Usuario();
        Proyecto proyecto = new Proyecto("P1", "descripcion", DateTime.Today.AddYears(1),admin,  new List<Usuario>());
        List<Proyecto> proyectos = new List<Proyecto> {proyecto};
        string mensaje = "Mensaje test";

        _mockGestorProyectos.Setup(g => g.NotificarAdministradoresDeProyectos(proyectos, mensaje));

        _controladorProyectos.NotificarAdministradoresDeProyectos(proyectos, mensaje);

        _mockGestorProyectos.Verify(g => g.NotificarAdministradoresDeProyectos(proyectos, mensaje), Times.Once);
    }
    
    [TestMethod]
    public void CalcularCaminoCritico_LlamaCorrectamenteAGestor()
    {
        var proyecto = new ProyectoDTO { Id = 1 };

        _mockGestorProyectos.Setup(g => g.CalcularCaminoCritico(proyecto));

        _controladorProyectos.CalcularCaminoCritico(proyecto);

        _mockGestorProyectos.Verify(g => g.CalcularCaminoCritico(proyecto), Times.Once);
    }
}