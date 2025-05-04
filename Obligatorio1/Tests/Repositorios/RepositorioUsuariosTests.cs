using Dominio;
using Repositorios;
using Servicios.Utilidades;

namespace Tests;

[TestClass]
public class RepositorioUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private Usuario _usuario;
    
    [TestInitialize]
    public void Setup()
    { 
        _repositorioUsuarios = new RepositorioUsuarios();
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
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
        _usuario.Id = 1;
        _repositorioUsuarios.Agregar(_usuario);
        Usuario ultimoDelRepositorioUsuario = _repositorioUsuarios.ObtenerPorId(1);
        Assert.AreEqual(_usuario, ultimoDelRepositorioUsuario);
    }

    [TestMethod]
    public void SeEliminaUnUsuarioOk()
    {
        _usuario.Id = 1;
        _repositorioUsuarios.Agregar(_usuario);
        _repositorioUsuarios.Eliminar(_usuario.Id);
        Assert.IsNull(_repositorioUsuarios.ObtenerPorId(1));
    }

    [TestMethod]
    public void SeObtieneListaDelRepoOk()
    {
        List<Usuario> usuarios = _repositorioUsuarios.ObtenerTodos();
        Assert.IsNotNull(usuarios);
        Assert.AreEqual(1, usuarios.Count);
        Assert.AreEqual("Admin", usuarios.First().Nombre);
    }

    [TestMethod]
    public void SePuedeObtenerUsuariosPorEmail()
    {
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Usuario obtenidoPorEmail = _repositorioUsuarios.ObtenerUsuarioPorEmail("admin@sistema.com");
        Assert.AreEqual(admin, obtenidoPorEmail);
    }

    [TestMethod]
    public void SePuedeModificarContrasenaDeUsuario()
    {
        string nuevaContrasena = UtilidadesContrasena.ValidarYEncriptarContrasena("1Admin=Sistema1");
        _repositorioUsuarios.ActualizarContrasena(0, nuevaContrasena);
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Assert.IsTrue(admin.Autenticar("1Admin=Sistema1"));
    }
}