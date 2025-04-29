using Dominio;
using Dominio.Excepciones;

namespace Tests;

[TestClass]
public class GestorUsuariosTests
{
    private GestorUsuarios _gestorUsuarios;
    private Usuario _adminSistema;

    [TestInitialize]
    public void SetUp()
    {
        // setup para reiniciar la variable estática, sin agregar un método en la clase que no sea coherente con el diseño
        typeof(GestorUsuarios).GetField("_cantidadUsuarios", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, 0);
        
        _adminSistema = new Usuario("admin", "admin", new DateTime(0001,01,01), "admin@admin.com", "AdminTaskTrackPro@2025");
        _gestorUsuarios = new GestorUsuarios(_adminSistema); // Primer admin sistema siempre tiene ID 0
    }
    
    private Usuario CrearUsuario(string nombre, string apellido, string email, string contrasena)
    {
        return new Usuario(nombre, apellido, new DateTime(2007,4,28), email, contrasena);
    }
    
    private Usuario CrearYAsignarAdminSistema()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.AgregarAdministradorSistema(_adminSistema, usuario.Id);
        return usuario;
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorUsuarios);
        Assert.AreEqual(1, _gestorUsuarios.Usuarios.Count); // se crea solo con administrador
    }

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema,usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        Assert.AreEqual(3, _gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios[1]);
        Assert.AreSame(usuario2, _gestorUsuarios.Usuarios[2]);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeAgregarUsuario()
    {   
        Usuario solicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema,solicitante);
        Usuario nuevoUsuario = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(solicitante,nuevoUsuario);
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIdsIncrementales()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        Assert.AreEqual(1, usuario1.Id);
        Assert.AreEqual(2, usuario2.Id);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        _gestorUsuarios.EliminarUsuario(usuario2.Id);
        Assert.AreEqual(2, _gestorUsuarios.Usuarios.Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios[1]);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void GestorNoEliminaPrimerAdministradorSistema()
    {
        _gestorUsuarios.EliminarUsuario(0);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        Usuario busqueda = _gestorUsuarios.ObtenerUsuario(usuario1.Id);
        Assert.AreEqual(usuario1, busqueda);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void ErrorSiSeBuscaUnUsuarioNoRegistrado()
    {
        Usuario busqueda = _gestorUsuarios.ObtenerUsuario(4);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        Usuario usuario = CrearYAsignarAdminSistema();
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void SoloUnAdminDeSistemaPuedeAsignarOtro()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);
        
        _gestorUsuarios.AgregarAdministradorSistema(usuario1, usuario2.Id);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeProyectoCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        Assert.IsTrue(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeAsignarAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeProyectoCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);

        Assert.IsFalse(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeEliminarAdministradorProyecto()
    {
        Usuario adminSistema = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(adminSistema, nuevoAdminProyecto);

        Usuario usuarioSolicitante = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);

        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioNoEsAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }
    
    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioEstaAdministrandoUnProyecto()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
        nuevoAdminProyecto.EstaAdministrandoUnProyecto = true; // esto lo gestiona el gestor de proyectos

        _gestorUsuarios.EliminarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto);
    }

    [TestMethod]
    public void AdminSistemaReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void AdminProyectoReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario administrador = CrearYAsignarAdminSistema();
        Usuario usuarioSolicitante = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void UnUsuarioPuedeReiniciarSuContraseñaCorrectamente()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.ReiniciarContrasena(usuario, usuario);
        Assert.IsTrue(usuario.Autenticar("TaskTrackPro@2025"));
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeReiniciarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
    }

    [TestMethod]
    public void AdminSistemaAutogeneraUnaContraseñaCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo =
            new Usuario("José", "Perez", new DateTime(1999, 9, 1), "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void AdminProyectoAutogeneraUnaContraseñaCorrectamente()
    {
        Usuario administrador = CrearYAsignarAdminSistema();
        Usuario usuarioSolicitante = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminDeSistemaNiDeProyectoPuedeAutogenerarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo);
    }

    [TestMethod]
    public void AdminSistemaPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo, nuevaContrasena);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }
    
    [TestMethod]
    public void AdminProyectoPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        Usuario administrador = CrearYAsignarAdminSistema();
        Usuario usuarioSolicitante = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo, nuevaContrasena);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void UsuarioPuedeModificarSuContrasena()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        string nuevaContrasena =  "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuario, usuario, nuevaContrasena);
        Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeModificarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);
        
        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivo, nuevaContrasena);
    }

    [TestMethod]
    public void SeNotificaReinicioDeContrasena()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        Assert.AreEqual("Se reinició su contraseña. La nueva contraseña es TaskTrackPro@2025", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeNotificaContrasenaAutogenerada()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        
        Assert.IsTrue(ultimaNotificacion.Mensaje.StartsWith("Se modificó su contraseña. La nueva contraseña es "));
        Assert.IsFalse(string.IsNullOrEmpty(nuevaContrasena)); // asegurar que hay una nueva contraseña en el mensaje
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeNotificaModificacionDeContrasena()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo, nuevaContrasena);
        
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        Assert.AreEqual($"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void NoSeNotificaSiElPropioUsuarioModificaSuContrasena()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        string nuevaContrasena =  "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuario, usuario, nuevaContrasena);
        
        Assert.AreEqual(0, usuario.Notificaciones.Count());
    }
    
    [TestMethod]
    public void LoginCorrecto()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3"); 
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        Usuario otro = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, otro);
        Usuario obtenido = _gestorUsuarios.LogIn(usuario.Email, "Contrase#a3");
        Assert.AreEqual(usuario, obtenido);
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void LoginIncorrectoConContraseñaIncorrecta()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        Usuario obtenido = _gestorUsuarios.LogIn(usuario.Email, "ContraseñaIncorrecta");
    }

    [ExpectedException(typeof(ExcepcionDominio))]
    [TestMethod]
    public void LoginIncorrectoConEmailNoRegistrado()
    {
        Usuario obtenido = _gestorUsuarios.LogIn("unemail@noregistrado.com", "unaContraseña");
    }
}

