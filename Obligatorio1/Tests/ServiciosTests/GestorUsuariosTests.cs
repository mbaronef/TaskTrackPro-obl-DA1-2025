using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Gestores;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorUsuariosTests
{
    private GestorUsuarios _gestorUsuarios;
    private Usuario _adminSistema;

    [TestInitialize]
    public void SetUp()
    {
        _gestorUsuarios = new GestorUsuarios();
        _adminSistema = _gestorUsuarios.AdministradorInicial;
    }
    
    private Usuario CrearUsuario(string nombre, string apellido, string email, string contrasena)
    {
        return _gestorUsuarios.CrearUsuario(nombre, apellido, new DateTime(2007,4,28), email, contrasena);
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
        Assert.AreEqual(1, _gestorUsuarios.Usuarios.ObtenerTodos().Count); // se crea solo con administrador
        Assert.AreEqual("Admin", _gestorUsuarios.Usuarios.ObtenerTodos().Last().Nombre);
    }

    [TestMethod]
    public void GestorCreaUsuarioCorrectamente()
    {
        DateTime fecha = new DateTime(2007, 4, 28);
        
        Usuario usuarioCreadoPorGestor = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        
        Assert.AreEqual("Juan", usuarioCreadoPorGestor.Nombre);
        Assert.AreEqual("Pérez", usuarioCreadoPorGestor.Apellido);
        Assert.AreEqual(fecha, usuarioCreadoPorGestor.FechaNacimiento);
        Assert.AreEqual("unemail@gmail.com", usuarioCreadoPorGestor.Email);
        Assert.IsTrue(usuarioCreadoPorGestor.Autenticar("Contrase#a3"));
    }

    [TestMethod]
    public void GestorAgregaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema,usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        Assert.AreEqual(3, _gestorUsuarios.Usuarios.ObtenerTodos().Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios.ObtenerTodos().ElementAt(1));
        Assert.AreSame(usuario2, _gestorUsuarios.Usuarios.ObtenerTodos().ElementAt(2));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeAgregarUsuario()
    {   
        Usuario solicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema,solicitante);
        Usuario nuevoUsuario = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(solicitante,nuevoUsuario);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        _gestorUsuarios.EliminarUsuario(_adminSistema, usuario2.Id);
        Assert.AreEqual(2, _gestorUsuarios.Usuarios.ObtenerTodos().Count);
        Assert.AreSame(usuario1, _gestorUsuarios.Usuarios.ObtenerTodos().ElementAt(1));
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void GestorNoEliminaPrimerAdministradorSistema()
    {
        _gestorUsuarios.EliminarUsuario(_adminSistema,0);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeEliminarUsuario()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);
        _gestorUsuarios.EliminarUsuario(usuario1, usuario2.Id);
    }
    
    [TestMethod]
    public void UsuarioSePuedeEliminarASiMismo()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.EliminarUsuario(usuario, usuario.Id);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);

        Usuario busqueda = _gestorUsuarios.ObtenerUsuarioPorId(usuario1.Id);
        Assert.AreEqual(usuario1, busqueda);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorSiSeBuscaUnUsuarioNoRegistrado()
    {
        Usuario busqueda = _gestorUsuarios.ObtenerUsuarioPorId(4);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        Usuario usuario = CrearYAsignarAdminSistema();
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
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

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        Assert.IsTrue(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeAsignarAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeProyectoCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);

        Assert.IsFalse(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeEliminarAdministradorProyecto()
    {
        Usuario adminSistema = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(adminSistema, nuevoAdminProyecto.Id);

        Usuario usuarioSolicitante = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioNoEsAdministradorProyecto()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioEstaAdministrandoUnProyecto()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario nuevoAdminProyecto = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        nuevoAdminProyecto.EstaAdministrandoUnProyecto = true; // esto lo gestiona el gestor de proyectos

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [TestMethod]
    public void AdminSistemaReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void AdminProyectoReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        Usuario administrador = CrearYAsignarAdminSistema();
        Usuario usuarioSolicitante = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void UnUsuarioPuedeReiniciarSuContraseñaCorrectamente()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.ReiniciarContrasena(usuario, usuario.Id);
        Assert.IsTrue(usuario.Autenticar("TaskTrackPro@2025"));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeReiniciarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaAutogeneraUnaContraseñaCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo =
            new Usuario("José", "Perez", new DateTime(1999, 9, 1), "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
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
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNiDeProyectoPuedeAutogenerarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo.Id, nuevaContrasena);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }
    
    [TestMethod]
    public void AdminProyectoPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        Usuario administrador = CrearYAsignarAdminSistema();
        Usuario usuarioSolicitante = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        Usuario usuarioObjetivo = CrearUsuario("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo.Id, nuevaContrasena);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void UsuarioPuedeModificarSuContrasena()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        string nuevaContrasena =  "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
        Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeModificarContrasena()
    {
        Usuario usuarioSolicitante = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioSolicitante);
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);
        
        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivo.Id, nuevaContrasena);
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void DaErrorSiSeCambiaContrasenaInvalida()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        string nuevaContrasena = "c1.A";
        _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
    }

    [TestMethod]
    public void NoSeCambiaContrasenaInvalida()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        string nuevaContrasena = "c1.A";
        try
        {
            _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
        }
        catch
        {
        } // Ignorar la excepción

        Assert.IsFalse(usuario.Autenticar(nuevaContrasena));
        Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
    }

    [TestMethod]
    public void SeNotificaReinicioDeContrasena()
    {
        Usuario usuarioSolicitante = CrearYAsignarAdminSistema();
        Usuario usuarioObjetivo = CrearUsuario("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuarioObjetivo);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
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

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
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
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivo.Id, nuevaContrasena);
        
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
        _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
        
        Assert.AreEqual(0, usuario.Notificaciones.Count());
    }

    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeCreaUnUsuario()
    {
        Usuario admin2 = CrearYAsignarAdminSistema();
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual("Se creó un nuevo usuario: Juan Pérez", ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueCreaUnUsuario()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        Assert.AreEqual(0, _adminSistema.Notificaciones.Count());
    }
    
    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeEliminaUnUsuario()
    {
        Usuario admin2 = CrearYAsignarAdminSistema();
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistema, usuario.Id);
        
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual("Se eliminó un nuevo usuario. Nombre: Juan, Apellido: Pérez", ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueEliminaUnUsuario()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistema, usuario.Id);
        Assert.AreEqual(0, _adminSistema.Notificaciones.Count());
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

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void LoginIncorrectoConContraseñaIncorrecta()
    {
        Usuario usuario = CrearUsuario("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario);
        Usuario obtenido = _gestorUsuarios.LogIn(usuario.Email, "ContraseñaIncorrecta");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void LoginIncorrectoConEmailNoRegistrado()
    {
        Usuario obtenido = _gestorUsuarios.LogIn("unemail@noregistrado.com", "unaContraseña");
    }
    
    [TestMethod]
    public void SeObtienenLosUsuariosQueNoEstanEnUnaLista()
    {
        Usuario usuario1 = CrearUsuario("Juan", "Pérez", "jp@gmail.com", "Contrase#a3");
        Usuario usuario2 = CrearUsuario("Mateo", "Pérez", "mp@gmail.com", "Contrase#a3");
        Usuario usuario3 = CrearUsuario("José", "Pérez", "jp@adinet.com.uy", "Contrase#a3");
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario1);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario2);
        _gestorUsuarios.AgregarUsuario(_adminSistema, usuario3);
        
        List<Usuario> usuarios = new List<Usuario> { usuario1, usuario2, _adminSistema };
        List<Usuario> usuariosNoEnLista  = _gestorUsuarios.ObtenerUsuariosDiferentes(usuarios);
        
        Assert.AreEqual(1, usuariosNoEnLista.Count); 
        Assert.AreEqual(usuario3, usuariosNoEnLista.ElementAt(0));
    }
}

