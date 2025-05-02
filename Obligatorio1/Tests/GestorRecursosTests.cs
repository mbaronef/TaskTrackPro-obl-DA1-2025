using Dominio;
using Dominio.Dummies;
using Servicios.Gestores;

namespace Tests;

[TestClass]
public class GestorRecursosTests
{
    private GestorRecursos _gestorRecursos;
    
    [TestInitialize]
    public void SetUp()
    {
        _gestorRecursos = new GestorRecursos();
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count);
    }

    private Usuario CrearAdministradorSistema()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000,01,01), "unemail@gmail.com", contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        return admin;
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

}