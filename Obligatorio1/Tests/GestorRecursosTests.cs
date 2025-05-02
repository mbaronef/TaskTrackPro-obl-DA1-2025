using Dominio;
using Dominio.Dummies;
using Servicios.Gestores;

namespace Tests;

[TestClass]
public class GestorRecursosTests
{
    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;
    
    [TestInitialize]
    public void SetUp()
    {
        _gestorProyectos = new GestorProyectos();
        _gestorRecursos = new GestorRecursos(_gestorProyectos);
    }
    
    private Usuario CrearAdministradorSistema()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000,01,01), "unemail@gmail.com", contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        return admin;
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count);
    }
    
    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        Usuario adminSistema = CrearAdministradorSistema();
        
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(adminSistema, recurso2);
        
        Assert.AreEqual(2, _gestorRecursos.Recursos.Count);
        Assert.AreEqual(recurso1, _gestorRecursos.Recursos.ElementAt(0));
        Assert.AreEqual(recurso2, _gestorRecursos.Recursos.ElementAt(1));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNoAgregaRecurso()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000,01,01), "unemail@gmail.com", contrasenaEncriptada);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(usuario, recurso);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000,01,01), "unemail@gmail.com", contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIdsIncrementales()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);

        Assert.AreEqual(1, recurso1.Id);
        Assert.AreEqual(2, recurso2.Id);
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);
        Assert.AreEqual(recurso1, _gestorRecursos.ObtenerRecursoPorId(1));
        Assert.AreEqual(recurso2, _gestorRecursos.ObtenerRecursoPorId(2));
    }
}