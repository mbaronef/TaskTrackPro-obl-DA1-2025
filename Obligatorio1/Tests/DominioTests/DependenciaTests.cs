using Dominio;
using Dominio.Excepciones;

namespace Tests;

[TestClass]
public class DependenciaTests
{
    private Tarea _tarea;

    [TestInitialize]
    public void Setup()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        _tarea = new Tarea("titulo", "descripcion", 6, fechaInicioEstimada);
    }
    [TestMethod]
    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        string tipo = "FF";
        
        Dependencia dependencia = new Dependencia(tipo, _tarea);
        
        Assert.AreEqual(tipo, dependencia.Tipo);
        Assert.AreEqual(_tarea, dependencia.Tarea);

    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsVacio()
    { 
        Dependencia dependencia = new Dependencia("", _tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsNull()
    {
        Dependencia dependencia = new Dependencia(null, _tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsEspacio()
    {
        Dependencia dependencia = new Dependencia("   ", _tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTareaEsNull()
    { 
        Dependencia dependencia = new Dependencia("FF", null);
    }
    
    [TestMethod]
    public void Constructor_AceptaTipoFF()
    {
        Dependencia dependencia = new Dependencia("FF", _tarea);
        Assert.AreEqual("FF", dependencia.Tipo);
    }

    [TestMethod]
    public void Constructor_AceptaTipoFS()
    {
        Dependencia dependencia = new Dependencia("FS", _tarea);
        Assert.AreEqual("FS", dependencia.Tipo);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoNoEsFFNiFS()
    {
        Dependencia dependencia = new Dependencia("SS", _tarea);
    }
    

}