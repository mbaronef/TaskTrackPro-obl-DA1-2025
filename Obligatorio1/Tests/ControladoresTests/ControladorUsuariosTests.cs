using Repositorios;
using Repositorios.Interfaces;
using Servicios.Gestores;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private GestorUsuarios _gestorUsuarios;
    
    [TestInitialize]
    public void AntesDeCadaTest()
    {
        _repositorioUsuarios = new RepositorioUsuarios();
        _gestorUsuarios = new GestorUsuarios(_repositorioUsuarios);
    }

    [TestMethod]
    public void Constructor()
    {
        ControladorUsuariosTests controlador = new ControladorUsuarios(_gestorUsuarios);
        Assert.IsNotNull(controlador);
    }
}