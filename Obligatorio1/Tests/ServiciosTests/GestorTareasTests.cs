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
        _gestorTareas = new GestorTareas();
        _gestorProyectos = new GestorProyectos();
    }

    [TestMethod]
    public void Constructor_CreaGestorValido()
    {
        Assert.IsNotNull(_gestorTareas);
    }
    
}
