
namespace Tests;
using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

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
    public void Constructor_SinFechaInicioEsNull()
    {
        string titulo = "Tarea";
        string descripcion = "Prueba de tarea";
        int duracionEnDias = 8;
        
        Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias);
        
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracionEnDias, tarea.DuracionEnDias);
        Assert.IsNull(tarea.FechaInicioMasTemprana); 
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
        Assert.IsNotNull(tarea.UsuariosAsignados);
        Assert.IsNotNull(tarea.RecursosNecesarios);
        Assert.IsNotNull(tarea.Dependencias);

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
    public void CambiarEstadoDePendienteAFinalizada()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        
        Assert.AreEqual(EstadoTarea.Completada, tarea.Estado);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAPendienteDesdeCompletada()
    {
        var tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAEnProcesoDesdeCompletada()
    {
        var tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.EnProceso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverABloqueadaDesdeCompletada()
    {
        var tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Bloqueada); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstadoNoPermiteVolverAPendienteDesdeEnProceso()
    {
        var tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    //Ver si hay algun otro camino que no se pueda hacer (pendiente directo a completada se puede??)
    
    [TestMethod]
    public void EsCriticaDevuelveTrueCuandoHolguraEsCero()
    {
        Tarea tarea = new Tarea("Tarea Crítica", "Descripción", 5);
        
        tarea.Holgura = 0;
        bool esCritica = tarea.EsCritica();
        
        Assert.IsTrue(esCritica);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveTrueSiElUsuarioEstaAsignado()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuario = new Usuario("nombre", "apellido", "mail@ejemplo.com", "password");
        tarea.UsuariosAsignados.Add(usuario);
        
        bool resultado = tarea.EsMiembro(usuario);
        
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsMiembroDevuelveFalseSiElUsuarioNoEstaAsignado()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuarioNoAsignado = new Usuario("nombre", "apellido", "mail@ejemplo.com", "password");
        
        bool resultado = tarea.EsMiembro(usuarioNoAsignado);
        
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EliminarUsuarioAsignadoEliminaUsuarioDeLaLista()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuario = new Usuario("hola", "hola", "hola", "hola") { Id = 1 };
        tarea.UsuariosAsignados.Add(usuario);
        
        tarea.EliminarUsuarioAsignado(usuario.Id);
        
        Assert.IsFalse(tarea.UsuariosAsignados.Contains(usuario));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuarioAsignadoLanzaExcepcionSiUsuariosAsignadosEsNull()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.UsuariosAsignados = null;
        
        tarea.EliminarUsuarioAsignado(1);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuarioAsignadoLanzaExcepcionSiUsuarioNoEstaAsignado()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        
        tarea.EliminarUsuarioAsignado(1);
    }
    
    [TestMethod]
    public void EliminarUsuarioAsignado_UsuarioEnSegundaPosicion_SeEliminaCorrectamente()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5);
        Usuario usuario1 = new Usuario("a", "a", "a", "a") { Id = 1 };
        Usuario usuario2 = new Usuario("b", "b", "b", "b") { Id = 2 };
        tarea.UsuariosAsignados.Add(usuario1);
        tarea.UsuariosAsignados.Add(usuario2);

        tarea.EliminarUsuarioAsignado(2);

        Assert.IsFalse(tarea.UsuariosAsignados.Contains(usuario2));
        Assert.IsTrue(tarea.UsuariosAsignados.Contains(usuario1));
    }

    
    [TestMethod]
    public void FechaFinMasTempranaInicializadaConMinValuePorDefecto()
    { 
        Tarea tarea = new Tarea("Titulo", "Descripción", 5);
            
        Assert.AreEqual(DateTime.MinValue, tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void FechaDeEjecucionInicializadaConMinValuePorDefecto()
    { 
        Tarea tarea = new Tarea("Titulo", "Descripción", 5);
            
        Assert.AreEqual(DateTime.MinValue, tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    public void ConstructorConIdCreaTareaCorrectamente()
    {
        int id = 1;
        string titulo = "Tarea";
        string descripcion = "Descripción de la tarea";
        int duracion = 5;
        DateTime fechaInicio = DateTime.Today;
        
        Tarea tarea = new Tarea(id, titulo, descripcion, duracion, fechaInicio);
        
        Assert.AreEqual(id, tarea.Id);
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracion, tarea.DuracionEnDias);
        Assert.AreEqual(fechaInicio, tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    public void AsignarUsuario_AsignarUsuarioALista()
    {
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);

        Usuario usu = new Usuario();
            
        tarea.AsignarUsuario(usu); 
            
        Assert.IsTrue(tarea.UsuariosAsignados.Contains(usu));
        Assert.AreEqual(1, tarea.UsuariosAsignados.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioEsNull()
    {
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);
        Usuario usu = null;
        tarea.AsignarUsuario(usu); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioYaEstaEnUsuariosAsignados()
    {
        Usuario usu = new Usuario(); 
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AsignarUsuario(usu);
        tarea.AsignarUsuario(usu);
    }
    
    [TestMethod]
    public void EliminarUsuario_EliminarUsuarioDeAsignados()
    {
        Usuario usu = new Usuario();
        usu.Id = 1;
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AsignarUsuario(usu);
        tarea.EliminarUsuario(1);

        Assert.IsFalse(tarea.UsuariosAsignados.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuario_LanzaExcepcionSiUsuarioNoExiste()
    {
        Usuario usu = new Usuario();
        usu.Id = 1;
            
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AsignarUsuario(usu);
        tarea.EliminarUsuario(3);
    }
    
    [TestMethod]
    public void AgregarRecurso_AgregarRecursoALista()
    {
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);

        Recurso recurso = new Recurso();
            
        tarea.AgregarRecurso(recurso); 
            
        Assert.IsTrue(tarea.RecursosNecesarios.Contains(recurso));
        Assert.AreEqual(1, tarea.RecursosNecesarios.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoEsNull()
    {
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);
        Recurso recurso = null;
        tarea.AgregarRecurso(recurso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoYaEstaEnRecursosNecesarios()
    {
        Recurso recurso = new Recurso(); 
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarRecurso(recurso);
        tarea.AgregarRecurso(recurso);
    }
    
    [TestMethod]
    public void EliminarRecurso_EliminarRecursoDeNecesarios()
    {
        Recurso recurso = new Recurso();
        recurso.Id = 1;
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarRecurso(recurso);
        tarea.EliminarRecurso(1);

        Assert.IsFalse(tarea.RecursosNecesarios.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarRecurso_LanzaExcepcionSiRecursoNoExiste()
    {
        Recurso rec = new Recurso();
        rec.Id = 1;
            
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarRecurso(rec);
        tarea.EliminarRecurso(3);
    }
    
    [TestMethod]
    public void ModificarTitulo_DeberiaActualizarElTitulo()
    {
        Tarea tarea = new Tarea("titulo viejo", "Desc",  2);

        tarea.ModificarTitulo("titulo nuevo");

        Assert.AreEqual("titulo nuevo", tarea.Titulo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsNull()
    {
        Tarea tarea = new Tarea("titulo viejo", "Desc",  2);

        tarea.ModificarTitulo(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsVacio()
    {
        Tarea tarea = new Tarea("titulo viejo", "Desc",  2);

        tarea.ModificarTitulo("");
    }

    [TestMethod]
    public void ModificarDescripcion_DeberiaActualizarLaDescripcion()
    {
        Tarea tarea = new Tarea("titulo", "Desc vieja",  2);

        tarea.ModificarDescripcion("Desc nueva");

        Assert.AreEqual("Desc nueva", tarea.Descripcion);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
    {
        Tarea tarea = new Tarea("t", "Desc",  2);

        tarea.ModificarDescripcion(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
    {
        Tarea tarea = new Tarea("t", "Desc",  2);

        tarea.ModificarTitulo("");
    }
    
    [TestMethod]
    public void ModificarFechaInicioMasTemprana_ActualizaLaFechaOK()
    { 
        Tarea tarea = new Tarea("t", "Desc",  2);

        DateTime nuevaFecha = new DateTime(2025, 5, 1);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);

        Assert.AreEqual(nuevaFecha, tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicioMasTemprana_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);

        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        tarea.ModificarFechaInicioMasTemprana(fechaPasada);
    } 
    
    [TestMethod]
    public void ModificarFechaDeEjecucion_ActualizaLaFechaOK()
    { 
        Tarea tarea = new Tarea("t", "Desc",  2);

        DateTime nuevaFecha = new DateTime(2025, 5, 1);
        tarea.ModificarFechaDeEjecucion(nuevaFecha);

        Assert.AreEqual(nuevaFecha, tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaDeEjecucion_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);

        DateTime fechaPasada = DateTime.Now.AddDays(-1);

        tarea.ModificarFechaDeEjecucion(fechaPasada);
    }

    [TestMethod]
    public void ModificarDuracion_ActualizaLaDuracionOK()
    {
        Tarea tarea = new Tarea("t", "Desc",  2);
        tarea.ModificarDuracion(4);
        Assert.AreEqual(4, tarea.DuracionEnDias);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDuracion_LanzaExcepcionSiEsCeroONegativa()
    {
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);

        tarea.ModificarDuracion(-2);
        tarea.ModificarDuracion(0);
    }
    
    [TestMethod]
    public void DarListaAsignados_DevuelveListaDeUsuariosAsignados()
    {
        Usuario asignado = new Usuario();
        Usuario asignado2 = new Usuario();
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);
            
        tarea.AsignarUsuario(asignado);
        tarea.AsignarUsuario(asignado2);

        List<Usuario> lista = tarea.UsuariosAsignados;

        Assert.AreEqual(2, lista.Count);
        Assert.IsTrue(lista.Contains(asignado));
        Assert.IsTrue(lista.Contains(asignado2));
    }
    [TestMethod]
    public void DarListaRecursosNecesarios_DevuelveListaDeRecursosNecesarios()
    {
        Recurso necesario = new Recurso();
        Recurso necesario2 = new Recurso();
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);
            
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
        Usuario miembro = new Usuario();
        Tarea tarea = new Tarea("Tarea", "Descripción",  9);
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
        DateTime fechaInicio = new DateTime(2025, 5, 1);
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
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);

        Dependencia dependencia = new Dependencia();
            
        tarea.AgregarDependencia(dependencia); 
            
        Assert.IsTrue(tarea.Dependencias.Contains(dependencia));
        Assert.AreEqual(1, tarea.Dependencias.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaEsNull()
    {
        Tarea tarea = new Tarea("Titulo", "Descripción", 3);
        Dependencia dependencia = null;
        tarea.AgregarDependencia(dependencia); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaYaEstaEnDependencias()
    {
        Dependencia dependencia = new Dependencia(); 
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarDependencia(dependencia);
        tarea.AgregarDependencia(dependencia);
    }

    [TestMethod]
    public void EliminarDependencia_EliminarDependenciaDeDependencias()
    {
        Tarea tareaDependiente = new Tarea(1, "dep", "desc", 5);
        Dependencia dependencia = new Dependencia
        {
            Tarea = tareaDependiente
        };

        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarDependencia(dependencia);
        tarea.EliminarDependencia(1);

        Assert.IsFalse(tarea.Dependencias.Any(d => d.Tarea.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarDependencia_LanzaExcepcionSiDepenciaNoExiste()
    {
        Dependencia dep = new Dependencia();
        dep.Tarea.Id = 1;
            
        Tarea tarea = new Tarea("tarea", "descr", 87);
        tarea.AgregarDependencia(dep);
        tarea.EliminarDependencia(3);
    }

    
    //TO DO:
    // Ver como hacemos con las listas de dependencias, y segun como se modifiquen hacer esos tests
    
}
    


