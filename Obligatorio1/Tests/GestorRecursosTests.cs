using Dominio;
using Servicios.Gestores;

namespace Tests;

[TestClass]
public class GestorRecursosTests
{
    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        GestorRecursos gestorRecursos = new GestorRecursos();
        Assert.IsNotNull(gestorRecursos);
        Assert.AreEqual(0, gestorRecursos.Recursos.Count);
    }

    [TestMethod]
    public void GestorAgregaUnRecursoCorrectamente()
    {
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        GestorRecursos gestorRecursos = new GestorRecursos();
        gestorRecursos.AgregarRecurso(recurso1);
        gestorRecursos.AgregarRecurso(recurso2);
        Assert.AreEqual(2, gestorRecursos.Recursos.Count);
        Assert.AreEqual(recurso1, gestorRecursos.Recursos.ElementAt(0));
        Assert.AreEqual(recurso2, gestorRecursos.Recursos.ElementAt(1));
    }

}