using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    
    [TestInitialize]
    public void Setup()
    { 
        _repositorioUsuarios = new RepositorioUsuarios();
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Assert.AreEqual("Admin", admin.Nombre);
        Assert.AreEqual("Admin", admin.Apellido);
        Assert.IsTrue(admin.EsAdministradorSistema);
    }

    [TestMethod]
    public void SeAgregaUnUsuarioOk()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
        usuario.Id = 1;
        _repositorioUsuarios.Agregar(usuario);
        Usuario ultimoDelRepositorioUsuario = _repositorioUsuarios.ObtenerPorId(1);
        Assert.AreEqual(usuario, ultimoDelRepositorioUsuario);
    }

    [TestMethod]
    public void SeEliminaUnUsuarioOk()
    {
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
        usuario.Id = 1;
        _repositorioUsuarios.Agregar(usuario);
        _repositorioUsuarios.Eliminar(usuario.Id);
        Assert.IsNull(_repositorioUsuarios.ObtenerPorId(1));
    }

    [TestMethod]
    public void SePuedeObtenerUsuariosPorEmail()
    {
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Usuario obtenido = _repositorioUsuarios.ObtenerUsuarioPorEmail("admin@sistema.com");
        Assert.AreEqual(admin, obtenido);
    }

    [TestMethod]
    public void SePuedeModificarContrasenaDeUsuario()
    {
        _repositorioUsuarios.ActualizarContrasena(0,"1Admin=Sistema1");
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Assert.IsTrue(admin.Autenticar("1Admin=Sistema1"));
    }
}