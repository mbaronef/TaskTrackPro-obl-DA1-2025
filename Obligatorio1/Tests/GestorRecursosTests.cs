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
        GestorRecursos gestorRecursos = new GestorRecursos();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        gestorRecursos.AgregarRecurso(recurso);
        Assert.AreEqual(recurso, gestorRecursos.Recursos.ElementAt(0));
    }

}