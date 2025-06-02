using Dominio;
using DTOs;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Gestores;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorUsuariosTests
{
    private GestorUsuarios _gestorUsuarios;
    private UsuarioDTO _adminSistemaDTO;
    private RepositorioUsuarios _repositorioUsuarios;

    [TestInitialize]
    public void SetUp()
    {
        _repositorioUsuarios = new RepositorioUsuarios();
        _gestorUsuarios = new GestorUsuarios(_repositorioUsuarios);
        _adminSistemaDTO = UsuarioDTO.DesdeEntidad(_gestorUsuarios.AdministradorInicial);
    }
    
    private UsuarioDTO CrearUsuarioDTO(string nombre, string apellido, string email, string contrasena)
    {
        return new UsuarioDTO()
        {
            Nombre = nombre, Apellido = apellido, FechaNacimiento = new DateTime(2007,4,28), Email = email,Contrasena = contrasena
        };
    }
    
    private UsuarioDTO CrearYAsignarAdminSistema()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.AgregarAdministradorSistema(_adminSistemaDTO, usuario.Id);
        return usuario;
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorUsuarios);
        Assert.AreEqual(1, _gestorUsuarios.ObtenerTodos().Count); // se crea solo con administrador
        Assert.AreEqual("Admin", _gestorUsuarios.ObtenerTodos().Last().Nombre);
    }

    [TestMethod]
    public void GestorCreaYAgregaUsuariosCorrectamente()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO,usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        Assert.AreEqual(3, _gestorUsuarios.ObtenerTodos().Count);
        Assert.AreEqual(usuario1.Id, _gestorUsuarios.ObtenerTodos().ElementAt(1).Id);
        Assert.AreEqual(usuario2.Id, _gestorUsuarios.ObtenerTodos().ElementAt(2).Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeAgregarUsuario()
    {  
        UsuarioDTO solicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO,solicitante);
        UsuarioDTO nuevoUsuario = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(solicitante,nuevoUsuario);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario2.Id);
        Assert.AreEqual(2, _gestorUsuarios.ObtenerTodos().Count);
        Assert.AreEqual(usuario1.Id, _gestorUsuarios.ObtenerTodos().ElementAt(1).Id);
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void GestorNoEliminaPrimerAdministradorSistema()
    {
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO,1);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeEliminarUsuario()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
        _gestorUsuarios.EliminarUsuario(usuario1, usuario2.Id);
    }
    
    [TestMethod]
    public void UsuarioSePuedeEliminarASiMismo()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(usuario, usuario.Id);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        UsuarioDTO busqueda = _gestorUsuarios.ObtenerUsuarioPorId(usuario1.Id);
        Assert.AreEqual(usuario1.Id, busqueda.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorSiSeBuscaUnUsuarioNoRegistrado()
    {
        UsuarioDTO busqueda = _gestorUsuarios.ObtenerUsuarioPorId(4);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        UsuarioDTO usuario = CrearYAsignarAdminSistema();
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void SoloUnAdminDeSistemaPuedeAsignarOtro()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
        
        _gestorUsuarios.AgregarAdministradorSistema(usuario1, usuario2.Id);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeProyectoCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        Assert.IsTrue(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeAsignarAdministradorProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeProyectoCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);

        Assert.IsFalse(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeEliminarAdministradorProyecto()
    {
        UsuarioDTO adminSistema = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(adminSistema, nuevoAdminProyecto.Id);

        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioNoEsAdministradorProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioEstaAdministrandoUnProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        nuevoAdminProyecto.EstaAdministrandoUnProyecto = true; // esto lo gestiona el gestor de proyectos

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ErrorEliminarUsuarioMiembroDeProyecto()
    {
        UsuarioDTO adminDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, adminDTO);
        adminDTO.EsAdministradorProyecto = true;

        Usuario admin = _repositorioUsuarios.ObtenerPorId(adminDTO.Id);
        Proyecto proyecto = new Proyecto("Proyecto", "descripción",DateTime.Today.AddDays(1), admin, new List<Usuario>());
        
        _gestorUsuarios.EliminarUsuario(adminDTO, adminDTO.Id);
    }

    [TestMethod]
    public void AdminSistemaReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void AdminProyectoReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void UnUsuarioPuedeReiniciarSuContraseñaCorrectamente()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        _gestorUsuarios.ReiniciarContrasena(usuarioDTO, usuarioDTO.Id);
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsTrue(usuario.Autenticar("TaskTrackPro@2025"));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeReiniciarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaAutogeneraUnaContraseñaCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = new UsuarioDTO(){
                Nombre = "José", Apellido = "Perez", FechaNacimiento = new DateTime(1999, 9, 1), Email = "unemail@gmail.com", Contrasena = "Contrase#a9"};
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);
        
        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void AdminProyectoAutogeneraUnaContraseñaCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena = ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminDeSistemaNiDeProyectoPuedeAutogenerarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivo = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivo);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivoDTO.Id, nuevaContrasena);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }
    
    [TestMethod]
    public void AdminProyectoPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivoDTO.Id, nuevaContrasena);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void UsuarioPuedeModificarSuContrasena()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena =  "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);
        
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeModificarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivo = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivo);
        
        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivo.Id, nuevaContrasena);
    }
    
    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void DaErrorSiSeCambiaContrasenaInvalida()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        string nuevaContrasena = "c1.A";
        _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
    }

    [TestMethod]
    public void NoSeCambiaContrasenaInvalida()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena = "c1.A";
        try
        {
            _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);
        }
        catch
        {
        } // Ignorar la excepción

        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsFalse(usuario.Autenticar(nuevaContrasena));
        Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
    }

    [TestMethod]
    public void SeNotificaReinicioDeContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        Assert.AreEqual("Se reinició su contraseña. La nueva contraseña es TaskTrackPro@2025", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeNotificaContrasenaAutogenerada()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.AutogenerarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
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
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante,usuarioObjetivoDTO.Id, nuevaContrasena);
        
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        Assert.AreEqual($"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}", ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void NoSeNotificaSiElPropioUsuarioModificaSuContrasena()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena =  "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);
        
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.AreEqual(0, usuario.Notificaciones.Count());
    }

    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeCreaUnUsuario()
    {
        UsuarioDTO admin2DTO = CrearYAsignarAdminSistema();
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        
        Usuario admin2 = _repositorioUsuarios.ObtenerPorId(admin2DTO.Id);
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual("Se creó un nuevo usuario: Juan Pérez", ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueCreaUnUsuario()
    {
        Usuario adminSistema = _gestorUsuarios.AdministradorInicial;

        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        Assert.AreEqual(0, adminSistema.Notificaciones.Count());
    }
    
    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeEliminaUnUsuario()
    {
        UsuarioDTO admin2DTO = CrearYAsignarAdminSistema();
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario.Id);
        
        Usuario admin2 = _repositorioUsuarios.ObtenerPorId(admin2DTO.Id);
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual("Se eliminó un nuevo usuario. Nombre: Juan, Apellido: Pérez", ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueEliminaUnUsuario()
    {
        Usuario adminSistema = _gestorUsuarios.AdministradorInicial;
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario.Id);
        Assert.AreEqual(0, adminSistema.Notificaciones.Count());
    }

    [TestMethod]
    public void LoginCorrecto()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        UsuarioDTO otro = CrearUsuarioDTO("Mateo", "Pérez", "unemail@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, otro);
        UsuarioDTO obtenido = _gestorUsuarios.LogIn(usuario.Email, "Contrase#a3");
        Assert.AreEqual(usuario.Id, obtenido.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void LoginIncorrectoConContraseñaIncorrecta()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        UsuarioDTO obtenido = _gestorUsuarios.LogIn(usuario.Email, "ContraseñaIncorrecta");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void LoginIncorrectoConEmailNoRegistrado()
    {
        UsuarioDTO obtenido = _gestorUsuarios.LogIn("unemail@noregistrado.com", "unaContraseña");
    }
    
    [TestMethod]
    public void SeObtienenLosUsuariosQueNoEstanEnUnaLista()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "jp@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "mp@gmail.com", "Contrase#a3");
        UsuarioDTO usuario3 = CrearUsuarioDTO("José", "Pérez", "jp@adinet.com.uy", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario3);
        
        List<UsuarioListarDTO> usuarios = new List<UsuarioListarDTO>
        {
            UsuarioListarDTO.DesdeDTO(usuario1),
            UsuarioListarDTO.DesdeDTO(usuario2),
            UsuarioListarDTO.DesdeDTO(_adminSistemaDTO),
        };
        List<UsuarioListarDTO> usuariosNoEnLista  = _gestorUsuarios.ObtenerUsuariosDiferentes(usuarios);
        
        Assert.AreEqual(1, usuariosNoEnLista.Count); 
        Assert.AreEqual(usuario3.Id, usuariosNoEnLista.ElementAt(0).Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void ValidarQueUnUsuarioNoEsPrimerAdminLanzaExcepcionConElPrimerAdmin()
    {
        _gestorUsuarios.ValidarUsuarioNoEsAdministradorInicial(_adminSistemaDTO.Id);
    }
}

