using Dominio;
using Dominio.Excepciones;

namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    private GestorUsuarios _gestorUsuarios;

    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(GestorUsuarios).GetField("_cantidadUsuarios", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        
        _gestorUsuarios = new GestorUsuarios();
    }

    private Usuario CrearUsuario1()
    {
        return new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
    }

    private Usuario CrearUsuario2()
    {
        return new Usuario("Mateo", "Perez", new DateTime(1980, 9, 1), "email@gmail.com", "Contrase#a9");
    }
    
    [TestMethod]
    public void ConstructorSinParametrosCreaGestorValido()
    { 
        Assert.IsNotNull(_gestorUsuarios);
        Assert.AreEqual(0, _gestorUsuarios.Usuarios.Count);
    }

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario1();
        Usuario usuario2 = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuario1);
        _gestorUsuarios.AgregarUsuario(usuario2);

        Assert.AreEqual(2, _gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios[0]);
        Assert.AreSame(usuario2, _gestorUsuarios.Usuarios[1]);
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIdsIncrementales()
    {
        Usuario usuario1 = CrearUsuario1();
        Usuario usuario2 = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuario1);
        _gestorUsuarios.AgregarUsuario(usuario2);

        Assert.AreEqual(1, usuario1.Id);
        Assert.AreEqual(2, usuario2.Id);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario1();
        Usuario usuario2 = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuario1);
        _gestorUsuarios.AgregarUsuario(usuario2);

        _gestorUsuarios.EliminarUsuario(usuario2.Id);
        Assert.AreEqual(1, _gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios[0]);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        Usuario usuario1 = CrearUsuario1();
        Usuario usuario2 = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuario1); 
        _gestorUsuarios.AgregarUsuario(usuario2); 

        Usuario busqueda = _gestorUsuarios.ObtenerUsuario(usuario1.Id);
        Assert.AreEqual(usuario1, busqueda);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        Usuario usuario = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuario);
        _gestorUsuarios.AgregarAdministradorSistema(usuario.Id);
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeSistemaCorrectamente()
    {
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(2000, 9, 1), "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(usuario);
        _gestorUsuarios.AgregarAdministradorSistema(usuario.Id);
        _gestorUsuarios.EliminarAdministradorSistema(usuario.Id);
        Assert.IsFalse(usuario.EsAdministradorSistema);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeProyectoCorrectamente()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        _gestorUsuarios.AgregarAdministradorSistema(usuarioSolicitante.Id);
        Usuario nuevoAdminProyecto = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(nuevoAdminProyecto);
        
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        Assert.IsTrue(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeAsignarAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        Usuario nuevoAdminProyecto = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(nuevoAdminProyecto);
        
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeProyectoCorrectamente()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        _gestorUsuarios.AgregarAdministradorSistema(usuarioSolicitante.Id);
        Usuario nuevoAdminProyecto = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(nuevoAdminProyecto);
        
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        
        Assert.IsFalse(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeEliminarAdministradorProyecto()
    {
        Usuario adminSistema = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(adminSistema);
        _gestorUsuarios.AgregarAdministradorSistema(adminSistema.Id);
        Usuario nuevoAdminProyecto = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(adminSistema, nuevoAdminProyecto);
        
        Usuario usuarioSolicitante = new Usuario("José", "Perez", new DateTime(1999, 9, 1), "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        
        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioNoEsAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        _gestorUsuarios.AgregarAdministradorSistema(usuarioSolicitante.Id);
        Usuario nuevoAdminProyecto = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(nuevoAdminProyecto);
        
        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [TestMethod]
    public void AdminSistemaReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        _gestorUsuarios.AgregarAdministradorSistema(usuarioSolicitante.Id);
        Usuario usuarioObjetivo = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }
    
    [TestMethod]
    public void AdminProyectoReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario administrador = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(administrador);
        _gestorUsuarios.AgregarAdministradorSistema(administrador.Id);
        Usuario usuarioSolicitante = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante);
        
        Usuario usuarioObjetivo = new Usuario("José", "Perez", new DateTime(1999, 9, 1), "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeReiniciarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario1();
        _gestorUsuarios.AgregarUsuario(usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario2();
        _gestorUsuarios.AgregarUsuario(usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
    }
}

