using Dominio;
using Repositorios;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioTareasTests
{
    private RepositorioTareas _repositorioTareas;
    private Tarea _tarea;

    [TestInitialize]
    public void SetUp()
    {
        _repositorioTareas = new RepositorioTareas();
        _tarea = new Tarea("Título", "Descripción", 2, new DateTime(2030, 2, 2));   
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        Tarea tarea = _repositorioTareas.ObtenerPorId(1);
        Assert.IsNull(tarea);
    }

    [TestMethod]
    public void SeAgregaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Assert.AreEqual(_tarea, _repositorioTareas.ObtenerPorId(_tarea.Id));
    }

    [TestMethod]
    public void SeEliminaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.Eliminar(_tarea.Id);
        Assert.IsNull(_repositorioTareas.ObtenerPorId(_tarea.Id));
    }
    
    [TestMethod]
    public void SeObtieneLaListaDeTareasOk()
    {
        _repositorioTareas.Agregar(_tarea);
        List<Tarea> tareas = _repositorioTareas.ObtenerTodos();
        Assert.IsNotNull(tareas);
        Assert.AreEqual(1, tareas.Count);
        Assert.AreEqual(_tarea, tareas.Last());
    }

    [TestMethod]
    public void SeModificaElTituloDeLaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarTitulo(_tarea.Id, "Otro título");
        Assert.AreEqual("Otro título", _tarea.Titulo);
    }
    
    [TestMethod]
    public void SeModificaLaDescripcionDeLaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarDescripcion(_tarea.Id, "Otra descripción");
        Assert.AreEqual("Otra descripción", _tarea.Descripcion);
    }
    
    [TestMethod]
    public void SeModificaLaDuracionDeLaTareaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarDuracion(_tarea.Id, 10);
        Assert.AreEqual(10, _tarea.DuracionEnDias);
    }

    [TestMethod]
    public void SeModificaLaFechaDeInicioMasTempranaDeLaTareaOk()
    {
        DateTime fecha = new DateTime(2040, 2, 2);
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarFechaInicioMasTemprana(_tarea.Id, fecha);
        Assert.AreEqual(fecha, _tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    public void SeModificaLaFechaDeEjecucionDeLaTareaOk()
    {
        DateTime fecha = new DateTime(2200, 2, 2);
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarFechaDeEjecucion(_tarea.Id, fecha);
        Assert.AreEqual(fecha, _tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    public void SeModificaElEstadoDeLaTareaOk()
    {
        EstadoTarea estado = EstadoTarea.Bloqueada;
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarEstado(_tarea.Id, estado);
        Assert.AreEqual(estado, _tarea.Estado);
    }

    [TestMethod]
    public void SeModificaLaHolguraDeLaTareaOk()
    {
        float holgura = 2;
        _repositorioTareas.Agregar(_tarea);
        _repositorioTareas.ModificarHolgura(_tarea.Id, holgura);
        Assert.AreEqual(holgura, _tarea.Holgura);
    }
    
    [TestMethod]
    public void SeAsignaUsuarioOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Usuario miembro = new Usuario("Mateo", "Pérez", new DateTime(2003, 2, 2), "unemail@gmail.com", "UnAc0ntr4señ@");
        miembro.Id = 1; // lo maneja internamente el gestor
        _repositorioTareas.AgregarUsuario(_tarea.Id, miembro);
        Assert.AreEqual(1, _tarea.UsuariosAsignados.Count);
        Assert.AreEqual(miembro, _tarea.UsuariosAsignados.Last());
    }
    
    [TestMethod]
    public void SeEliminaMiembroOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Usuario miembro = new Usuario("Mateo", "Pérez", new DateTime(2003, 2, 2), "unemail@gmail.com", "UnAc0ntr4señ@");
        miembro.Id = 1; // lo maneja internamente el gestor
        _repositorioTareas.AgregarUsuario(_tarea.Id, miembro);
        _repositorioTareas.EliminarUsuario(_tarea.Id, miembro.Id);
        Assert.AreEqual(0,_tarea.UsuariosAsignados.Count);
    }
    
    [TestMethod]
    public void SeAsignaRecursoOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        recurso.Id = 1; // lo maneja internamente el gestor
        _repositorioTareas.AgregarRecursoNecesario(_tarea.Id, recurso);
        Assert.AreEqual(1, _tarea.RecursosNecesarios.Count);
        Assert.AreEqual(recurso, _tarea.RecursosNecesarios.Last());
    }
    
    [TestMethod]
    public void SeEliminaRecursoOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        recurso.Id = 1; // lo maneja internamente el gestor
        _repositorioTareas.AgregarRecursoNecesario(_tarea.Id, recurso);
        _repositorioTareas.EliminarRecursoNecesario(_tarea.Id, recurso.Id);
        Assert.AreEqual(0, _tarea.RecursosNecesarios.Count);
    }
    
    [TestMethod]
    public void SeAgregaDependenciaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Tarea otraTarea = new Tarea("Otro título", "Otra descripción", 2, new DateTime(2030, 2, 2));
        Dependencia dependencia = new Dependencia("FF", otraTarea);
        _repositorioTareas.AgregarDependencia(_tarea.Id, dependencia);
        Assert.AreEqual(1, _tarea.Dependencias.Count);
        Assert.AreEqual(dependencia, _tarea.Dependencias.Last());
    }

    [TestMethod]
    public void SeEliminaDependenciaOk()
    {
        _repositorioTareas.Agregar(_tarea);
        Tarea otraTarea = new Tarea("Otro título", "Otra descripción", 2, new DateTime(2030, 2, 2));
        Dependencia dependencia = new Dependencia("FF", otraTarea);
        _repositorioTareas.AgregarDependencia(_tarea.Id, dependencia);
        _repositorioTareas.EliminarDependencia(_tarea.Id,otraTarea.Id);
        Assert.AreEqual(0, _tarea.Dependencias.Count);
    }
}