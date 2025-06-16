using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RecursoTests
{
    [TestMethod]
    public void ConstructorCreaRecursoYAsignaOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción", 1);
        Assert.AreEqual("Nombre", recurso.Nombre);
        Assert.AreEqual("Tipo", recurso.Tipo);
        Assert.AreEqual("Descripción", recurso.Descripcion);
        Assert.IsNull(recurso.ProyectoAsociado);
        Assert.AreEqual(1, recurso.Capacidad);
        Assert.AreEqual(0, recurso.CantidadDeTareasUsandolo);
        Assert.IsNotNull(recurso.RangosEnUso);
    }
    
    //TODO validar capacidad mayor a 0

    [TestMethod]
    public void RecursoSeHaceExclusivoOk()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "unEmail@gmail.com", "UnAc@ntr4");
        List<Usuario> usuarios = new List<Usuario>();
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto = new Proyecto("Nombre", "Descripcion", fechaInicio, usuario, usuarios);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 1);
        recurso.AsociarAProyecto(proyecto);
        Assert.IsTrue(recurso.EsExclusivo());
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoSePuedeHacerExclusivoUnRecursoYaExclusivo()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "unEmail@gmail.com", "UnAc@ntr4");
        List<Usuario> usuarios = new List<Usuario>();
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Proyecto proyecto1 = new Proyecto("Nombre", "Descripción", fechaInicio, usuario, usuarios);
        Proyecto proyecto2 = new Proyecto("Otro nombre", "Otra descripción", fechaInicio, usuario, usuarios);
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripción", 1);
        recurso.AsociarAProyecto(proyecto1);
        recurso.AsociarAProyecto(proyecto2);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConNombreVacio()
    {
        Recurso recurso = new Recurso("", "Tipo", "Descripcion",1 );
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConNombreNull()
    {
        Recurso recurso = new Recurso(null, "Tipo", "Descripcion",1 );
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConTipoVacio()
    {
        Recurso recurso = new Recurso("Nombre", "", "Descripcion",1 );
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConTipoNull()
    {
        Recurso recurso = new Recurso("Nombre", null, "Descripcion",1 );
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConDescripcionVacia()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", " ",1 );
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorCrearRecursoConDescripcionNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", null,1 );
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    public void DaErrorCrearRecursoConCapacidadCero()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 0);
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    public void DaErrorCrearRecursoConCapacidadMenorACero()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", -3);
    }

    [TestMethod]
    public void SeModificaNombreOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoNombre = "otro";
        recurso.ModificarNombre(nuevoNombre);
        Assert.AreEqual(nuevoNombre, recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorNombreVacio()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoNombre = "";
        recurso.ModificarNombre(nuevoNombre);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorNombreNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoNombre = null;
        recurso.ModificarNombre(nuevoNombre);
    }

    [TestMethod]
    public void SeModificaTipoOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoTipo = "otro";
        recurso.ModificarTipo(nuevoTipo);
        Assert.AreEqual(nuevoTipo, recurso.Tipo);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorTipoVacio()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoTipo = "";
        recurso.ModificarTipo(nuevoTipo);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorTipoNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevoTipo = null;
        recurso.ModificarTipo(nuevoTipo);
    }

    [TestMethod]
    public void SeModificaDescripcionOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevaDesc = "otro";
        recurso.ModificarDescripcion(nuevaDesc);
        Assert.AreEqual(nuevaDesc, recurso.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorDescripcionVacia()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevaDesc = "";
        recurso.ModificarDescripcion(nuevaDesc);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void DaErrorModificarPorDescripcionNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        string nuevaDesc = null;
        recurso.ModificarDescripcion(nuevaDesc);
    }

    [TestMethod]
    public void RecursoAsignaOkCantidadDeTareasUsandolo()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1 );
        int cantidadActualDeTareasUsandolo = recurso.CantidadDeTareasUsandolo;
        recurso.IncrementarCantidadDeTareasUsandolo();
        Assert.AreEqual(1, recurso.CantidadDeTareasUsandolo);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoLoPuedeUsarUnaCantidadNegativaDeTareas()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1);
        recurso.DecrementarCantidadDeTareasUsandolo();
    }

    [TestMethod]
    public void SeCalculaSiSeEstaUsando()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1);
        Assert.IsFalse(recurso.SeEstaUsando());
    }

    [TestMethod]
    public void EqualsRetornaTrueSiLosIdsSonIguales()
    {
        // por simplicidad se hardcodean los ids, los gestiona el repo.
        Recurso recurso1 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 1 };
        Recurso recurso2 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 1 };
        bool sonIguales = recurso1.Equals(recurso2);
        Assert.IsTrue(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiLosIdsNoSonIguales()
    {
        // por simplicidad se hardcodean los ids, los gestiona el repo.
        Recurso recurso1 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 1 };
        Recurso recurso2 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 2 };
        bool sonIguales = recurso1.Equals(recurso2);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoEsNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1);
        bool sonIguales = recurso.Equals(null);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoNoEsUsuario()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1);
        int otro = 0;
        bool sonIguales = recurso.Equals(otro);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void GetHashCodeFuncionaOk()
    {
        // por simplicidad se hardcodean los ids, los gestiona el repo.
        Recurso recurso1 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 1 };
        Recurso recurso2 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 1 };
        Recurso recurso3 = new Recurso("Nombre", "Tipo", "Descripcion",1) { Id = 2 };
        Assert.AreEqual(recurso1.GetHashCode(), recurso2.GetHashCode());
        Assert.AreNotEqual(recurso3.GetHashCode(), recurso1.GetHashCode());
    }

    [TestMethod]
    public void ToStringFuncionaCorrectamente()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion",1);
        string resultadoEsperado =
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'";
        Assert.AreEqual(resultadoEsperado, recurso.ToString());
    }
}