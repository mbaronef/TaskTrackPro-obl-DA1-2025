using Dominio;
using Dominio.Dummies;
using Servicios.Gestores;

namespace Tests;

[TestClass]
public class GestorRecursosTests
{
    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;

    [TestInitialize]
    public void SetUp()
    {
        _gestorProyectos = new GestorProyectos();
        _gestorRecursos = new GestorRecursos(_gestorProyectos);
    }

    private Usuario CrearAdministradorSistema()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        return admin;
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count);
    }

    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        Usuario adminSistema = CrearAdministradorSistema();

        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminSistema, recurso1);
        _gestorRecursos.AgregarRecurso(adminSistema, recurso2);

        Assert.AreEqual(2, _gestorRecursos.Recursos.Count);
        Assert.AreEqual(recurso1, _gestorRecursos.Recursos.ElementAt(0));
        Assert.AreEqual(recurso2, _gestorRecursos.Recursos.ElementAt(1));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiProyectoNoAgregaRecurso()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(usuario, recurso);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorLlevaCuentaDeUsuariosCorrectamenteYAsignaIdsIncrementales()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);

        Assert.AreEqual(1, recurso1.Id);
        Assert.AreEqual(2, recurso2.Id);
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso1 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        Recurso recurso2 = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso1);
        _gestorRecursos.AgregarRecurso(admin, recurso2);
        Assert.AreEqual(recurso1, _gestorRecursos.ObtenerRecursoPorId(1));
        Assert.AreEqual(recurso2, _gestorRecursos.ObtenerRecursoPorId(2));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void GestorNoObtieneRecursoConIdInexistente()
    {
        Recurso recurso = _gestorRecursos.ObtenerRecursoPorId(20);
    }

    [TestMethod]
    public void SeEliminaUnRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(admin, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoSeEliminaRecursoSiEstaEnUso()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        recurso.ModificarCantidadDeTareasUsandolo(3);
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(admin, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoEliminaRecursos()
    {
        Usuario admin = CrearAdministradorSistema();
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.EliminarRecurso(usuario, recurso.Id);
    }

    [TestMethod]
    public void AdminProyectoEliminaRecursoExclusivo()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.Recursos.Count());
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursoNoExclusivo()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);

        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;

        _gestorRecursos.EliminarRecurso(adminProyecto, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosExclusivosDeOtrosProyectos()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);

        Usuario otroAdminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        otroAdminProyecto.EsAdministradorProyecto = true;
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción", otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        _gestorRecursos.EliminarRecurso(otroAdminProyecto, recurso.Id);
    }

    [TestMethod]
    public void AdminSistemaModificaNombreDeRecursoOk()
    {
        Usuario admin = CrearAdministradorSistema();
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarNombreRecurso(admin, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [TestMethod]
    public void AdminProyectoModificaNombreDeRecursoExclusivoOk()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);

        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);

        _gestorRecursos.ModificarNombreRecurso(adminProyecto, recurso.Id, "Nuevo nombre");
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarNombre()
    {
        Usuario admin = CrearAdministradorSistema();
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(admin, recurso);
        _gestorRecursos.ModificarNombreRecurso(usuario, recurso.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosNoExclusivosDeSuProyecto()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.Id = 1; // lo hace el gestor de usuarios
        adminProyecto.EsAdministradorProyecto = true;
        Proyecto proyecto = new Proyecto("Nombre", "Descripción", adminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(proyecto, adminProyecto);
        
        Usuario otroAdminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        otroAdminProyecto.Id = 2; // lo hace el gestor de usuarios
        otroAdminProyecto.EsAdministradorProyecto = true;
        Proyecto otroProyecto = new Proyecto("Otro nombre", "Otra descripción", otroAdminProyecto, new List<Usuario>());
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);
        
        Recurso recurso = new Recurso("Analista Senior", "Humano", "Un analista Senior con experiencia");
        _gestorRecursos.AgregarRecurso(adminProyecto, recurso);
        
        _gestorRecursos.ModificarNombreRecurso(otroAdminProyecto, recurso.Id, "Nuevo nombre");
    }

}

//TODO: 1.Métodos modificación 2.Refactor 3.Notificar al eliminar recursos y al modificar