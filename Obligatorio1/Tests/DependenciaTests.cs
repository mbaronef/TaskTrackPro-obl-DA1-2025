namespace Tests;
using Dominio;
using Dominio.Excepciones;

[TestClass]

public class DependenciaTests
{
    [TestMethod]
    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        string tipo = "tipo";
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        
        Dependencia dependencia = new Dependencia(tipo, tarea);
        
        Assert.AreEqual(tipo, dependencia.Tipo);
        Assert.AreEqual(tarea, dependencia.Tarea);

    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsVacio()
    { 
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        Dependencia dependencia = new Dependencia("", tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsNull()
    {
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        Dependencia dependencia = new Dependencia(null, tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsEspacio()
    {
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        Dependencia dependencia = new Dependencia("   ", tarea);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTareaEsNull()
    { 
        Dependencia dependencia = new Dependencia("", null);
    }

    [TestMethod]
    public void Constructor_AceptaTipoFF()
    {
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        Dependencia dependencia = new Dependencia("FF", tarea);
        Assert.AreEqual("FF", dependencia.Tipo);
    }



}