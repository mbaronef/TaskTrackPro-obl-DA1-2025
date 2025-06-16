using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RecursoNecesarioTests
{
    [TestMethod]
    public void Constructor_ConParametros_AsignacionCorrecta()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        int cantidad = 2;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);

        Assert.AreEqual(recurso, rn.Recurso);
        Assert.AreEqual(cantidad, rn.Cantidad);
    }
}