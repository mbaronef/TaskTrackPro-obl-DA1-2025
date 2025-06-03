using Controladores;
using DTOs;
using Repositorios;
using Repositorios.Interfaces;
using Servicios.Gestores;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private GestorUsuarios _gestorUsuarios;
    private ControladorUsuarios _controladorUsuarios;
    
    [TestInitialize]
    public void AntesDeCadaTest()
    {
        _repositorioUsuarios = new RepositorioUsuarios();
        _gestorUsuarios = new GestorUsuarios(_repositorioUsuarios);
        _controladorUsuarios = new ControladorUsuarios(_gestorUsuarios);
    }

    [TestMethod]
    public void Constructor()
    {
        Assert.IsNotNull(_controladorUsuarios);
    }
    
    [TestMethod]
    public void ObtenerUsuariosDiferentes_ObtieneUsuariosOK()
    {
        UsuarioListarDTO administrador = UsuarioListarDTO.DesdeEntidad(_gestorUsuarios.AdministradorInicial);
        List<UsuarioListarDTO> usuarios = new List<UsuarioListarDTO>();
        usuarios.Add(administrador);
        List<UsuarioListarDTO> usuariosSinAdministrador = _controladorUsuarios.ObtenerUsuariosDiferentes(usuarios);
        Assert.AreEqual(0, usuariosSinAdministrador.Count);
    }
}