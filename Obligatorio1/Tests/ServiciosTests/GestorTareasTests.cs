using Dominio;
using Servicios.Excepciones;
using Servicios.Gestores;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorTareasTests
{
    private GestorTareas _gestorTareas;
    private GestorProyectos _gestorProyectos;
    
    [TestInitialize]
    public void Inicializar()
    {
        _gestorProyectos = new GestorProyectos();
        _gestorTareas = new GestorTareas(_gestorProyectos);
    }

    [TestMethod]
    public void Constructor_CreaGestorValido()
    {
        Assert.IsNotNull(_gestorTareas);
    }
    
}
