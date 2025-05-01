using Dominio;
using Dominio.Excepciones;

namespace Tests;

[TestClass]
public class RecursoTests
{
    [TestMethod]
    public void ConstructorCreaRecursoYAsignaOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción");
        Assert.AreEqual("Nombre",recurso.Nombre);
        Assert.AreEqual("Tipo", recurso.Tipo);
        Assert.AreEqual("Descripción", recurso.Descripcion);
        Assert.IsNull(recurso.ProyectoAsociado);
        Assert.AreEqual(0,recurso.CantidadDeTareasUsandolo);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConNombreVacio()
    {
        Recurso recurso = new Recurso("", "Tipo", "Descripcion");
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConNombreNull()
    {
        Recurso recurso = new Recurso(null, "Tipo", "Descripcion");
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConTipoVacio()
    {
        Recurso recurso = new Recurso("Nombre", "", "Descripcion");
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConTipoNull()
    {
        Recurso recurso = new Recurso("Nombre", null, "Descripcion");
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConDescripcionVacia()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "");
    }
}