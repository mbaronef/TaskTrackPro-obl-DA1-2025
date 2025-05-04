
namespace Tests;
using Dominio;
using Dominio.Excepciones;
using Servicios.Gestores;

[TestClass]

public class TareaTests
{
    
    [TestMethod]

    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        string titulo = "Tarea";
        string descripcion = "Prueba de tarea";
        int duracionEnDias = 8;
        
        Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias, fechaInicioEstimada);
        
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracionEnDias, tarea.DuracionEnDias);
        Assert.AreEqual(fechaInicioEstimada, tarea.FechaInicioMasTemprana);
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado); // Estado por defecto
        Assert.IsNotNull(tarea.UsuariosAsignados);
        Assert.IsNotNull(tarea.Dependencias);
        Assert.IsNotNull(tarea.RecursosNecesarios);

    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsVacio()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("", "Descripción válida", 8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea(null, "Descripción válida",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsEspacio()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea(" ", "Descripción válida",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsNegativa()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", -3, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsCero()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", 0, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", "", 8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", null,  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsEspacio()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        
        Tarea tarea = new Tarea("Titulo", " ",  8, fechaInicioEstimada);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiFechaInicioEsMenorQueHoy()
    {
        DateTime fechaInicioAnterior = DateTime.Today.AddDays(-1); 

        Tarea tarea = new Tarea("Titulo", "Descripción válida", 5, fechaInicioAnterior);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAPendienteDesdeCompletada()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAEnProcesoDesdeCompletada()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.EnProceso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverABloqueadaDesdeCompletada()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Bloqueada); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAPendienteDesdeEnProceso()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermitePasarDePendienteACompletada()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.Pendiente);
        tarea.CambiarEstado(EstadoTarea.Completada); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermitePasarDeBloqueadaACompletada()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.CambiarEstado(EstadoTarea.Bloqueada);
        tarea.CambiarEstado(EstadoTarea.Completada); 
    }

    [TestMethod]
    public void CambiarEstadoAEnProcesoIncrementaCantidadDeTareasUsando()
    {
        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
        Tarea tarea = new Tarea("Título", "Descripción", 3, DateTime.Today);
        tarea.AgregarRecurso(recurso);

        tarea.CambiarEstado(EstadoTarea.EnProceso);

        Assert.AreEqual(1, recurso.CantidadDeTareasUsandolo);
    }
    
    [TestMethod]
    public void CambiarEstadoACompletadaDecrementaCantidadDeTareasUsandoYSeteaFecha()
    {
        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
        Tarea tarea = new Tarea("Título", "Descripción", 3, DateTime.Today);
        tarea.AgregarRecurso(recurso);
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        
        Assert.AreEqual(0, recurso.CantidadDeTareasUsandolo);
        Assert.AreEqual(DateTime.Today, tarea.FechaDeEjecucion);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoACompletada_SinUsoPrevio_LanzaExcepcion()
    {
        Recurso recurso = new Recurso ("recurso", "tipo", "descripcion" );
        Tarea tarea = new Tarea("Título", "Descripción", 3, DateTime.Today);
        tarea.AgregarRecurso(recurso);
    
        tarea.CambiarEstado(EstadoTarea.Completada);
    }
    
    [TestMethod]
    public void EsCriticaDevuelveTrueCuandoHolguraEsCero()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.Holgura = 0;
        bool esCritica = tarea.EsCritica();
        
        Assert.IsTrue(esCritica);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveTrueSiElUsuarioEstaAsignado()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        tarea.UsuariosAsignados.Add(usuario);
        
        bool resultado = tarea.EsMiembro(usuario);
        
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveFalseSiElUsuarioNoEstaAsignado()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        Usuario usuarioNoAsignado = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        
        bool resultado = tarea.EsMiembro(usuarioNoAsignado);
        
        Assert.IsFalse(resultado);
    }
    
    [TestMethod]
    public void FechaDeEjecucionInicializadaConMinValuePorDefecto()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
            
        Assert.AreEqual(DateTime.MinValue, tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    public void AsignarUsuario_AsignarUsuarioALista()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            
        tarea.AsignarUsuario(usuario); 
            
        Assert.IsTrue(tarea.UsuariosAsignados.Contains(usuario));
        Assert.AreEqual(1, tarea.UsuariosAsignados.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        Usuario usu = null;
        tarea.AsignarUsuario(usu); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioYaEstaEnUsuariosAsignados()
    {
        Usuario usu = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AsignarUsuario(usu);
        tarea.AsignarUsuario(usu);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuario_LanzaExcepcionSiUsuariosAsignadosEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        
        tarea.UsuariosAsignados = null;
        
        tarea.EliminarUsuario(1);
    }
    
    [TestMethod]
    public void EliminarUsuario_EliminarUsuarioDeAsignados()
    {
        Usuario usu = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        usu.Id = 1;
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AsignarUsuario(usu);
        tarea.EliminarUsuario(1);

        Assert.IsFalse(tarea.UsuariosAsignados.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuario_LanzaExcepcionSiUsuarioNoExiste()
    {
        Usuario usu = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        usu.Id = 1;
            
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AsignarUsuario(usu);
        tarea.EliminarUsuario(3);
    }
    
    [TestMethod]
    public void AgregarRecurso_AgregarRecursoALista()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
            
        tarea.AgregarRecurso(recurso); 
            
        Assert.IsTrue(tarea.RecursosNecesarios.Contains(recurso));
        Assert.AreEqual(1, tarea.RecursosNecesarios.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        Recurso recurso = null;
        tarea.AgregarRecurso(recurso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoYaEstaEnRecursosNecesarios()
    {
        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AgregarRecurso(recurso);
        tarea.AgregarRecurso(recurso);
    }
    
    [TestMethod]
    public void EliminarRecurso_EliminarRecursoDeNecesarios()
    { 
        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
        recurso.Id = 1;
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AgregarRecurso(recurso);
        tarea.EliminarRecurso(1);

        Assert.IsFalse(tarea.RecursosNecesarios.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarRecurso_LanzaExcepcionSiRecursoNoExiste()
    { 
        Recurso rec = new Recurso("recurso", "tipo", "descripcion" );
        rec.Id = 1;
            
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AgregarRecurso(rec);
        tarea.EliminarRecurso(3);
    }
    
    [TestMethod]
    public void ModificarTitulo_DeberiaActualizarElTitulo()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarTitulo("titulo nuevo");

        Assert.AreEqual("titulo nuevo", tarea.Titulo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarTitulo(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsVacio()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarTitulo("");
    }

    [TestMethod]
    public void ModificarDescripcion_DeberiaActualizarLaDescripcion()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarDescripcion("Desc nueva");

        Assert.AreEqual("Desc nueva", tarea.Descripcion);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarDescripcion(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarTitulo("");
    }
    
    [TestMethod]
    public void ModificarFechaInicioMasTemprana_ActualizaLaFechaOK()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        DateTime nuevaFecha = DateTime.Today.AddDays(10);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Assert.AreEqual(nuevaFecha, tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicioMasTemprana_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        tarea.ModificarFechaInicioMasTemprana(fechaPasada);
    } 
    
    [TestMethod]
    public void ModificarFechaDeEjecucion_ActualizaLaFechaOK()
    { 
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        DateTime nuevaFecha = DateTime.Today.AddDays(10);
        tarea.ModificarFechaDeEjecucion(nuevaFecha);

        Assert.AreEqual(nuevaFecha, tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaDeEjecucion_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        tarea.ModificarFechaDeEjecucion(fechaPasada);
    }

    [TestMethod]
    public void ModificarDuracion_ActualizaLaDuracionOK()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.ModificarDuracion(4);
        Assert.AreEqual(4, tarea.DuracionEnDias);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDuracion_LanzaExcepcionSiEsCeroONegativa()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);

        tarea.ModificarDuracion(-2);
        tarea.ModificarDuracion(0);
    }
    
    [TestMethod]
    public void DarListaAsignados_DevuelveListaDeUsuariosAsignados()
    {
        Usuario admin = new Usuario("Admin", "Root", new DateTime(1980, 1, 1), "admin@test.com", "TaskTrackPro@2025");
        admin.EsAdministradorSistema = true;
        GestorUsuarios gestor = new GestorUsuarios();
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(1995, 1, 1), "juan@test.com", "TaskTrackPro@2025");
        Usuario usuario2 = new Usuario("Juana", "Pereza", new DateTime(1996, 2, 2), "juana@test.com", "TaskTrackPro@2025");
        gestor.AgregarUsuario(admin, usuario1);
        gestor.AgregarUsuario(admin, usuario2);
        Tarea tarea = new Tarea("Título", "Descripción", 5, new DateTime(2026, 9, 1));
        tarea.AsignarUsuario(usuario1);
        tarea.AsignarUsuario(usuario2);
        
        List<Usuario> asignados = tarea.UsuariosAsignados;

        
        Assert.AreEqual(2, asignados.Count);
        Assert.IsTrue(asignados.Any(u => u.Id == usuario1.Id));
        Assert.IsTrue(asignados.Any(u => u.Id == usuario2.Id));
    }

    [TestMethod]
    public void DarListaRecursosNecesarios_DevuelveListaDeRecursosNecesarios()
    {
        Recurso necesario = new Recurso("recurso", "tipo", "descripcion" );
        Recurso necesario2 = new Recurso("recurso2", "tipo2", "descripcion" );
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
            
        tarea.AgregarRecurso(necesario);
        tarea.AgregarRecurso(necesario2);

        List<Recurso> lista = tarea.RecursosNecesarios;

        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Contains(necesario));
        Assert.IsTrue(lista.Contains(necesario2));
    
    }
    
    [TestMethod]
    public void NotificarMiembros_AgregaNotificacionATodosLosMiembros()
    {
        Usuario miembro = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Título", "Descripción", 5, fechaInicioEstimada);
        tarea.AsignarUsuario(miembro);
        tarea.NotificarMiembros("Se modificó el proyecto.");

        foreach (Usuario u in tarea.UsuariosAsignados)
        {
            Assert.IsTrue(u.Notificaciones.Any(n => n.Mensaje == "Se modificó el proyecto."));
        }
    }
    
    [TestMethod] 
    public void FechaFinMasTempranaSeCalculaCorrectamente() 
    {
        string titulo = "Título de prueba";
        string descripcion = "Descripción de prueba";
        int duracion = 5;
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        DateTime fechaEsperada = fechaInicio.AddDays(duracion);
        
        Tarea tarea = new Tarea(titulo, descripcion, duracion, fechaInicio);
        
        Assert.AreEqual(fechaEsperada, tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void ModificarDuracionActualizaFechaFinMasTemprana()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Tarea tarea = new Tarea("Titulo", "Descripcion", 3, fechaInicio);
        
        tarea.ModificarDuracion(5);
        
        Assert.AreEqual(fechaInicio.AddDays(5), tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void ModificarFechaInicioMasTempranaActualizaFechaFinMasTemprana()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        Tarea tarea = new Tarea("Titulo", "Descripcion", 3, fechaInicio);
        
        DateTime nuevaFechaInicio = fechaInicio.AddDays(2);
        tarea.ModificarFechaInicioMasTemprana(nuevaFechaInicio);
        
        Assert.AreEqual(nuevaFechaInicio.AddDays(3), tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void AgregarDependencia_AgregarDependenciaALista()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tareaD = new Tarea("Titulo", "Descripción", 3, fechaInicioEstimada);
        Tarea tarea = new Tarea("Titulo", "Descripción", 3, fechaInicioEstimada);

        Dependencia dependencia = new Dependencia("FF", tareaD);
            
        tarea.AgregarDependencia(dependencia); 
            
        Assert.IsTrue(tarea.Dependencias.Contains(dependencia));
        Assert.AreEqual(1, tarea.Dependencias.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaEsNull()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("Titulo", "Descripción", 3, fechaInicioEstimada);
        Dependencia dependencia = null;
        tarea.AgregarDependencia(dependencia); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaYaEstaEnDependencias()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tareaD = new Tarea("Titulo","Descripción",3, fechaInicioEstimada);
        Dependencia dependencia = new Dependencia("tipo", tareaD);
        
        Tarea tarea = new Tarea("tarea", "descr", 87, fechaInicioEstimada);
        tarea.AgregarDependencia(dependencia);
        tarea.AgregarDependencia(dependencia);
    }

    [TestMethod]
    public void EliminarDependencia_EliminarDependenciaDeDependencias()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tareaD = new Tarea("Titulo", "Descripción", 3, fechaInicioEstimada);
        tareaD.Id = 1;
        Dependencia dependencia = new Dependencia("FF", tareaD);
        Tarea tarea = new Tarea("tarea", "descr", 87, fechaInicioEstimada);
        tarea.AgregarDependencia(dependencia);
        tarea.EliminarDependencia(1);
        Assert.IsFalse(tarea.Dependencias.Any(d => d.Tarea.Id == 1));
    }

    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarDependencia_LanzaExcepcionSiDepenciaNoExiste()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tareaD = new Tarea("Titulo", "Descripción", 3, fechaInicioEstimada);
        Dependencia dependencia = new Dependencia("tipo", tareaD);
    
        Tarea tarea = new Tarea("tarea", "descr", 87, fechaInicioEstimada);
        tarea.AgregarDependencia(dependencia);
        
        tarea.EliminarDependencia(3);
    }
    
    [TestMethod]
    public void VerificarDependenciaNoEstaAgregada_NoExisteNoLanzaExcepcion()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("titulo", "descripcion", 3, fechaInicioEstimada);
        Tarea tarea2 = new Tarea("titulo2", "descripcion2", 3, fechaInicioEstimada);
        Dependencia dependencia = new Dependencia("FF", tarea2); 
        tarea.VerificarDependenciaNoEstaAgregada(dependencia);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void VerificarDependenciaNoEstaAgregada_YaExisteLanzaExcepcion()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        Tarea tarea = new Tarea("titulo", "descripcion", 3, fechaInicioEstimada);
        Tarea tarea2 = new Tarea("titulo2", "descripcion2", 3, fechaInicioEstimada);
        Dependencia dependencia = new Dependencia("FF", tarea2); 
        tarea.AgregarDependencia(dependencia);
        
        tarea.VerificarDependenciaNoEstaAgregada(dependencia);
    }
    
    


    
}
    


