using Repositorios;
using Servicios.CaminoCritico;
using Servicios.Gestores;
using Servicios.Gestores.Interfaces;
using Servicios.Notificaciones;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorTareasTests
{
    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Notificador notificador = new Notificador();
        CaminoCritico caminoCritico = new CaminoCritico();
        RepositorioUsuarios repositorioUsuarios = new RepositorioUsuarios();
        RepositorioProyectos repositorioProyectos = new RepositorioProyectos();
        GestorProyectos gestorProyectos = new GestorProyectos(repositorioUsuarios, repositorioProyectos, notificador, caminoCritico);
        IGestorTareas gestorTareas = new GestorTareas(gestorProyectos, repositorioUsuarios, notificador, caminoCritico);
        
        ControladorTareas controladorTareas = new ControladorTareas(gestorTareas);
        Assert.IsNotNull(controladorTareas);
    }
    /*métodos a probar:
     GestorTareas.EsMiembroDeTarea() 
    GestorTareas.ObtenerTareaPorId()
    GestorTareas.AgregarTareaAlProyecto()
    GestorTareas.CambiarEstadoTarea()
    GestorTareas.ModificarTituloTarea()
    GestorTareas.ModificarDuracionTarea()
    GestorTareas.ModificarDescripcionTarea()
    GestorTareas.EliminarTareaDelProyecto()
    GestorTareas.AgregarDependenciaATarea()
    GestorTareas.EliminarDependenciaDeTarea()
    GestorTareas.AgregarRecursoATarea()
    GestorTareas.EliminarRecursoDeTarea()
    GestorTareas.AgregarMiembroATarea()
    GestorTareas.EliminarMiembroDeTarea()
*/
}