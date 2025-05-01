using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioProyectosTest
{
    private RepositorioProyectos _repositorioProyectos;
    private Usuario _usuario;
    private Proyecto _proyecto;
    
    [TestInitialize]
    public void SetUp()
    {
        _repositorioProyectos = new RepositorioProyectos(); 
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
        _usuario.EsAdministradorProyecto = true;
        List<Usuario> lista = new List<Usuario>();
        _proyecto = new Proyecto("Proyecto", "hacer algo", _usuario, lista);
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioProyectos repositorioProyectos = new RepositorioProyectos();
        Proyecto proyecto = repositorioProyectos.ObtenerPorId(1);
        Assert.IsNull(proyecto);
    }

    [TestMethod]
    public void SeAgregaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Assert.AreEqual(_proyecto, _repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeEliminaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.Eliminar(_proyecto.Id);
        Assert.IsNull(_repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeObtieneLaListaDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        List<Proyecto> proyectos = _repositorioProyectos.ObtenerTodos();
        Assert.IsNotNull(proyectos);
        Assert.AreEqual(1, proyectos.Count);
        Assert.AreEqual(_proyecto, proyectos.Last());
    }

    [TestMethod]
    public void SeModificaElNombreDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarNombre(_proyecto.Id, "Nuevo");
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual("Nuevo", proyecto.Nombre);
    }
    
    [TestMethod]
    public void SeModificaLaDescripcionDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarDescripcion(_proyecto.Id, "Nuevo");
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual("Nuevo", proyecto.Descripcion);
    }
    
    [TestMethod]
    public void SeModificaLaFechaInicioDeProyectosOk()
    {
        DateTime fechaInicio = DateTime.Today;
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarFechaInicio(_proyecto.Id, fechaInicio);
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual(fechaInicio, proyecto.FechaInicio);
    }
    
    [TestMethod]
    public void SeModificaLaFechaFinMasTempranaDeProyectosOk()
    {
        DateTime fechaInicio = new DateTime(2028, 1, 1);
        DateTime fechaFin = new DateTime(2030, 1, 1);
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarFechaInicio(_proyecto.Id, fechaInicio);
        _repositorioProyectos.ModificarFechaFinMasTemprana(_proyecto.Id, fechaFin);
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual(fechaFin, proyecto.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void SeModificaElAdministradorDeProyectoOk()
    {
        Usuario nuevo = new Usuario("Mateo", "Pérez", new DateTime(2003, 2, 2), "unemail@gmail.com", "UnAc0ntr4señ@");
        nuevo.EsAdministradorProyecto = true;
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.ModificarAdministrador(_proyecto.Id, nuevo);
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual(nuevo, proyecto.Administrador);
    }

    [TestMethod]
    public void SeAgregaTareaOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Tarea tarea = new Tarea();
        _repositorioProyectos.AgregarTarea(_proyecto.Id, tarea);
        Assert.AreEqual(1,_proyecto.Tareas.Count);
        Assert.AreEqual(tarea, _proyecto.Tareas.Last());
    }
    
    [TestMethod]
    public void SeEliminaTareaOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Tarea tarea = new Tarea();
        _repositorioProyectos.AgregarTarea(_proyecto.Id, tarea);
        _repositorioProyectos.EliminarTarea(_proyecto.Id, tarea.Id);
        Assert.AreEqual(0,_proyecto.Tareas.Count);
    }

    [TestMethod]
    public void SeAgregaMiembroOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Usuario miembro = new Usuario("Mateo", "Pérez", new DateTime(2003, 2, 2), "unemail@gmail.com", "UnAc0ntr4señ@");
        miembro.Id = 1; // lo maneja internamente el gestor
        _repositorioProyectos.AgregarMiembro(_proyecto.Id, miembro);
        Assert.AreEqual(2, _proyecto.Miembros.Count);
        Assert.AreEqual(miembro, _proyecto.Miembros.Last());
    }
    
    [TestMethod]
    public void SeEliminaMiembroOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Usuario miembro = new Usuario("Mateo", "Pérez", new DateTime(2003, 2, 2), "unemail@gmail.com", "UnAc0ntr4señ@");
        miembro.Id = 1; // lo maneja internamente el gestor
        _repositorioProyectos.AgregarMiembro(_proyecto.Id, miembro);
        _repositorioProyectos.EliminarMiembro(_proyecto.Id, miembro.Id);
        Assert.AreEqual(1,_proyecto.Miembros.Count);
    }
}
