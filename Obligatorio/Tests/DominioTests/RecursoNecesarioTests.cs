using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RecursoNecesarioTests
{

    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RecursoNecesario recursoNecesario = new RecursoNecesario();
        Assert.IsNotNull(recursoNecesario);
    }
    

    [TestMethod]
    public void Constructor_ConParametrosAsignaCorrectamente()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        int cantidad = 2;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);

        Assert.AreEqual(recurso, rn.Recurso);
        Assert.AreEqual(cantidad, rn.Cantidad);
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void Constructor_LanzaExcepcionSiRecursoEsNull()
    {
        Recurso recurso = null;
        int cantidad = 2;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);
    }
}