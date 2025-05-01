using Dominio;
using Dominio.Dummies;
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

    [TestMethod]
    public void ConstructorCreaRecursoExclusivoOk()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "unEmail@gmail.com", "UnAc@ntr4");
        List<Usuario> usuarios = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Nombre", "Descripcion", usuario, usuarios);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", proyecto);
        Assert.IsTrue(recurso.EsExclusivo());
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
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConDescripcionNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", null);
    }

    [TestMethod]
    public void SeModificaNombreOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoNombre = "otro";
        recurso.ModificarNombre(nuevoNombre);
        Assert.AreEqual(nuevoNombre, recurso.Nombre);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorNombreVacio()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoNombre = "";
        recurso.ModificarNombre(nuevoNombre);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorNombreNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoNombre = null;
        recurso.ModificarNombre(nuevoNombre);
    }
    
    [TestMethod]
    public void SeModificaTipoOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoTipo = "otro";
        recurso.ModificarTipo(nuevoTipo);
        Assert.AreEqual(nuevoTipo, recurso.Tipo);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorTipoVacio()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoTipo = "";
        recurso.ModificarTipo(nuevoTipo);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorTipoNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevoTipo = null;
        recurso.ModificarTipo(nuevoTipo);
    }
    
    [TestMethod]
    public void SeModificaDescripcionOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevaDesc = "otro";
        recurso.ModificarDescripcion(nuevaDesc);
        Assert.AreEqual(nuevaDesc, recurso.Descripcion);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorDescripcionVacia()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevaDesc = "";
        recurso.ModificarDescripcion(nuevaDesc);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorDescripcionNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        string nuevaDesc = null;
        recurso.ModificarDescripcion(nuevaDesc);
    }

    [TestMethod]
    public void SeHaceExclusivoOk()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "unEmail@gmail.com", "UnAc@ntr4");
        List<Usuario> usuarios = new List<Usuario>();
        Proyecto proyecto = new Proyecto("Nombre", "Descripcion", usuario, usuarios);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion");
        recurso.HacerExclusivo(proyecto);
        Assert.IsTrue(recurso.EsExclusivo());
        Assert.AreEqual(proyecto, recurso.ProyectoAsociado);
    }
}