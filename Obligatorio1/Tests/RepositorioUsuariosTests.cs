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
}