using Dominio;
using Repositorios;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private Usuario _usuario;

    [TestInitialize]
    public void Setup()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(RepositorioUsuarios).GetField("_cantidadUsuarios",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);

        _repositorioUsuarios = new RepositorioUsuarios();
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
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