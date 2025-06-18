using Dominio;
using Repositorios;
using Microsoft.EntityFrameworkCore;
using Tests.Contexto;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioRecursosTests
{
    private RepositorioRecursos _repositorioRecursos;
    private SqlContext _contexto;
    private Recurso _recurso;
    private Proyecto _proyecto;
    private Usuario _adminProyecto;

    [TestInitialize]
    public void SetUp()
    {
        _contexto = SqlContextFactory.CrearContextoEnMemoria();

        _repositorioRecursos = new RepositorioRecursos(_contexto);
        
        _recurso = new Recurso("nombre", "tipo", "descripcion", 1);
        _recurso.IncrementarCantidadDeTareasUsandolo();
        _recurso.IncrementarCantidadDeTareasUsandolo();
        _recurso.IncrementarCantidadDeTareasUsandolo(); // cantidad de tarea usandolo = 3
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    public void InicializarProyecto()
    {
        _adminProyecto = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
        _adminProyecto.EsAdministradorProyecto = true;
        
        List<Usuario> miembros = new List<Usuario>();
        _proyecto = new Proyecto("Proyecto", "hacer algo", DateTime.Today.AddDays(10), _adminProyecto, miembros);
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        Assert.IsNotNull(_repositorioRecursos);
    }

    [TestMethod]
    public void SeAgregaRecursooOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        Assert.AreEqual(_recurso, _repositorioRecursos.ObtenerPorId(_recurso.Id));
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        Recurso recurso2 = new Recurso("Nombre", "tipo", "desc",1);
        _repositorioRecursos.Agregar(recurso2);

        Assert.AreEqual(1, _recurso.Id);
        Assert.AreEqual(2, recurso2.Id);
    }

    [TestMethod]
    public void SeEliminaRecursoOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        _repositorioRecursos.Eliminar(_recurso.Id);
        Assert.IsNull(_repositorioRecursos.ObtenerPorId(_recurso.Id));
    }

    [TestMethod]
    public void SeObtieneLaListaDeRecursosOk()
    {
        _repositorioRecursos.Agregar(_recurso);
        List<Recurso> recursos = _repositorioRecursos.ObtenerTodos();
        Assert.IsNotNull(recursos);
        Assert.AreEqual(1, recursos.Count);
        Assert.AreEqual(_recurso, recursos.Last());
    }

    [TestMethod]
    public void SeActualizaUnRecursoSinProyectoAsociadoOk()
    {
        _repositorioRecursos.Agregar(_recurso);

        Recurso recursoCambios = new Recurso("Recurso Actualizado", "Nuevo Tipo", "Descripción actualizada", 5)
        {
            CantidadDeTareasUsandolo = 10,
            ProyectoAsociado = null
        };
        recursoCambios.Id = _recurso.Id;
        
        _repositorioRecursos.Actualizar(recursoCambios);
        
        Recurso recursoActualizado = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual("Recurso Actualizado", recursoActualizado.Nombre);
        Assert.AreEqual("Nuevo Tipo", recursoActualizado.Tipo);
        Assert.AreEqual("Descripción actualizada", recursoActualizado.Descripcion);
        Assert.AreEqual(10, recursoActualizado.CantidadDeTareasUsandolo);
        Assert.AreEqual(5, recursoActualizado.Capacidad);
        Assert.IsNull(recursoActualizado.ProyectoAsociado);
    }


    [TestMethod]
    public void SeActualizaUnRecursoConProyectoAsociadoOk()
    {
        InicializarProyecto();
        _contexto.Proyectos.Add(_proyecto);
        
        _repositorioRecursos.Agregar(_recurso);
        
        Recurso recursoCambios = new Recurso("Recurso Actualizado", "Nuevo Tipo", "Descripción actualizada", 10)
        {
            CantidadDeTareasUsandolo = 10,
            ProyectoAsociado = _proyecto
        };
        recursoCambios.Id = _recurso.Id;
        
        _repositorioRecursos.Actualizar(recursoCambios);
        
        Recurso recursoActualizado = _repositorioRecursos.ObtenerPorId(_recurso.Id);
        Assert.AreEqual("Recurso Actualizado", recursoActualizado.Nombre);
        Assert.AreEqual("Nuevo Tipo", recursoActualizado.Tipo);
        Assert.AreEqual("Descripción actualizada", recursoActualizado.Descripcion);
        Assert.AreEqual(10, recursoActualizado.CantidadDeTareasUsandolo);
        Assert.AreEqual(10, recursoActualizado.Capacidad);
        Assert.AreEqual(recursoActualizado.ProyectoAsociado, _proyecto);
    }
}