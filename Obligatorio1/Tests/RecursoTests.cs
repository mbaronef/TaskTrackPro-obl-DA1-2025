using Dominio;

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
    
}