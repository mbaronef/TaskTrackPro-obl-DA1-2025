using Dominio;
using Repositorios;
using Microsoft.EntityFrameworkCore;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private SqlContext _contexto;
    private Usuario _usuario;

    [TestInitialize]
    public void Setup()
    {
        var opciones = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // base única para cada test
            .Options;

        _contexto = new SqlContext(opciones);

        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void SeAgregaUnUsuarioOk()
    {
        _repositorioUsuarios.Agregar(_usuario);
        Usuario ultimoDelRepositorioUsuario = _repositorioUsuarios.ObtenerPorId(1);
        Assert.AreEqual(_usuario, ultimoDelRepositorioUsuario);
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        _repositorioUsuarios.Agregar(_usuario);
        Usuario usuario2 = new Usuario("Mateo", "Pérez", new DateTime(1999, 2, 2), "email@gmail.com", "contr5Ase{a");
        _repositorioUsuarios.Agregar(usuario2);

        Assert.AreEqual(1, _usuario.Id);
        Assert.AreEqual(2, usuario2.Id);
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
        _repositorioUsuarios.Agregar(_usuario);
        List<Usuario> usuarios = _repositorioUsuarios.ObtenerTodos();
        Assert.IsNotNull(usuarios);
        Assert.AreEqual(1, usuarios.Count);
        Assert.AreEqual("Juan", usuarios.First().Nombre);
    }

    [TestMethod]
    public void SePuedeObtenerUsuariosPorEmail()
    {
        Usuario admin = _repositorioUsuarios.ObtenerPorId(0);
        Usuario obtenidoPorEmail = _repositorioUsuarios.ObtenerUsuarioPorEmail("admin@sistema.com");
        Assert.AreEqual(admin, obtenidoPorEmail);
    }
}