using Dominio.Dummies;
using Repositorios;

namespace Tests;

[TestClass]
public class RepositorioUsuariosTests
{
    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        RepositorioUsuarios repositorioUsuarios = new RepositorioUsuarios();
        Usuario admin = repositorioUsuarios.ObtenerPorId(0);
        Assert.AreEqual("Admin", admin.Nombre);
        Assert.AreEqual("Admin", admin.Apellido);
        Assert.IsTrue(admin.EsAdministradorSistema);
    }

    [TestMethod]
    public void SeAgregaUnUsuarioOk()
    {
        RepositorioUsuarios repositorioUsuarios = new RepositorioUsuarios();
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(1998,7,6), "unEmail@gmail.com", "uNaC@ntr4seña");
        usuario.Id = 1;
        repositorioUsuarios.Agregar(usuario);
        Usuario ultimoDelRepositorioUsuario = repositorioUsuarios.ObtenerPorId(1);
        Assert.AreEqual(usuario, ultimoDelRepositorioUsuario);
    }
}