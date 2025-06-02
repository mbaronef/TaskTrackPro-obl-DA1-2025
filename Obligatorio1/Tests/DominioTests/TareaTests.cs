﻿using Dominio;
using Dominio.Excepciones;
using Servicios.Gestores;

namespace Tests.DominioTests;


[TestClass] 
public class TareaTests
{
    private DateTime _fechaInicio = new DateTime(2500, 9, 1);

    [TestMethod]
    public void ConstructorSinParametrosCreaTareaCorrectamente()
    {
        Tarea tarea = new Tarea();
        Assert.IsNotNull(tarea);
    }

    [TestMethod]
    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        string titulo = "Tarea";
        string descripcion = "Prueba de tarea";
        int duracionEnDias = 8;
        
        Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias, _fechaInicio);
        
        Assert.AreEqual(titulo, tarea.Titulo);
        Assert.AreEqual(descripcion, tarea.Descripcion);
        Assert.AreEqual(duracionEnDias, tarea.DuracionEnDias);
        Assert.AreEqual(_fechaInicio, tarea.FechaInicioMasTemprana);
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
        Assert.IsNotNull(tarea.UsuariosAsignados);
        Assert.IsNotNull(tarea.Dependencias);
        Assert.IsNotNull(tarea.RecursosNecesarios);

    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsVacio()
    { 
        Tarea tarea = new Tarea("", "Descripción válida", 8, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsNull()
    {
        Tarea tarea = new Tarea(null, "Descripción válida",  8, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTituloEsEspacio()
    {
        Tarea tarea = new Tarea(" ", "Descripción válida",  8, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsNegativa()
    {
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", -3, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDuracionEsCero()
    {
        Tarea tareaNegativa = new Tarea("Tarea negativa", "Descripción válida", 0, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsVacia()
    { 
        Tarea tarea = new Tarea("Titulo", "", 8, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsNull()
    {
        Tarea tarea = new Tarea("Titulo", null,  8, _fechaInicio);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiDescripcionEsEspacio()
    {
        Tarea tarea = new Tarea("Titulo", " ",  8, _fechaInicio);
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
    public void CambiarEstado_NoPermiteVolverAPendienteDesdeCompletada()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_NoPermiteVolverAEnProcesoDesdeCompletada()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.EnProceso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_NoPermiteVolverABloqueadaDesdeCompletada()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Completada);
        tarea.CambiarEstado(EstadoTarea.Bloqueada); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_NoPermiteVolverAPendienteDesdeEnProceso()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Pendiente); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_NoPermitePasarDePendienteACompletada()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.Pendiente);
        tarea.CambiarEstado(EstadoTarea.Completada); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_NoPermitePasarDeBloqueadaACompletada()
    {
        Tarea tarea = CrearTareaValida();
        tarea.CambiarEstado(EstadoTarea.Bloqueada);
        tarea.CambiarEstado(EstadoTarea.Completada); 
    }
    
    [TestMethod]
    public void CambiarEstado_AEnProcesoModificaLaFechaInicioMasTemprana()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarFechaInicioMasTemprana(DateTime.Today.AddDays(5));
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        Assert.AreEqual(DateTime.Today, tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    public void CambiarEstado_ACompletadaDecrementaCantidadDeTareasUsandoYSeteaFecha()
    {
        Recurso recurso = CrearRecursoValido();
        Tarea tarea = CrearTareaValida();
        tarea.AgregarRecurso(recurso);
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        tarea.CambiarEstado(EstadoTarea.Completada);
        Assert.AreEqual(0, recurso.CantidadDeTareasUsandolo);
        Assert.AreEqual(DateTime.Today, tarea.FechaDeEjecucion);
        Assert.AreEqual(DateTime.Today, tarea.FechaFinMasTemprana);
        Assert.AreEqual(1,tarea.DuracionEnDias);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void CambiarEstado_ACompletadaSinUsoPrevioLanzaExcepcion()
    {
        Recurso recurso = CrearRecursoValido();
        Tarea tarea = CrearTareaValida();
        tarea.AgregarRecurso(recurso);
        tarea.CambiarEstado(EstadoTarea.Completada);
    }
    
    [TestMethod]
    public void EsCritica_DevuelveTrueCuandoHolguraEsCero()
    {
        Tarea tarea = CrearTareaValida();
        tarea.Holgura = 0;
        bool esCritica = tarea.EsCritica();
        Assert.IsTrue(esCritica);
    }
    
    [TestMethod]
    public void EsMiembro_DevuelveTrueSiElUsuarioEstaAsignado()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = CrearUsuarioValido();
        tarea.UsuariosAsignados.Add(usuario);
        bool resultado = tarea.EsMiembro(usuario);
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsMiembro_DevuelveFalseSiElUsuarioNoEstaAsignado()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuarioNoAsignado = CrearUsuarioValido();
        bool resultado = tarea.EsMiembro(usuarioNoAsignado);
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsSucesoraDe_DevuelveTrueSiLaTareaTieneDependencias()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        bool resultado = tarea.EsSucesoraDe(dependencia.Tarea.Id);
        Assert.IsTrue(resultado);
    }
    
    [TestMethod]
    public void EsSucesoraDe_DevuelveFalseSiLaTareaDependeDeOtraTarea()
    {
        Dependencia dependencia = CrearDependenciaValida();
        Tarea tarea = CrearTareaValida();
        Tarea otraTarea = CrearTareaValida();
        tarea.Id = 2; //se hardcodean ids por simplicidad de tests. Los maneja el gestor
        otraTarea.Id = 3;
        tarea.AgregarDependencia(dependencia);
        
        bool resultado = tarea.EsSucesoraDe(otraTarea.Id);
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EsSucesoraDe_DevuelveFalseSiNoTieneDependencias()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        tarea.EliminarDependencia(dependencia.Tarea.Id);
        bool resultado = tarea.EsSucesoraDe(dependencia.Tarea.Id);
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void FechaDeEjecucionInicializadaConMinValuePorDefecto()
    { 
        Tarea tarea = CrearTareaValida();
        Assert.AreEqual(DateTime.MinValue, tarea.FechaDeEjecucion);
    }
    
    [TestMethod]
    public void AsignarUsuario_AsignarUsuarioALista()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = CrearUsuarioValido();
        tarea.AsignarUsuario(usuario); 
        Assert.IsTrue(tarea.UsuariosAsignados.Contains(usuario));
        Assert.AreEqual(1, tarea.UsuariosAsignados.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioEsNull()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = null;
        tarea.AsignarUsuario(usuario); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AsignarUsuario_LanzarExcepcionSiUsuarioYaEstaEnUsuariosAsignados()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = CrearUsuarioValido();
        tarea.AsignarUsuario(usuario);
        tarea.AsignarUsuario(usuario);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuario_LanzaExcepcionSiUsuariosAsignadosEsNull()
    {
        Tarea tarea = CrearTareaValida();
        tarea.EliminarUsuario(1);
    }
    
    [TestMethod]
    public void EliminarUsuario_EliminarUsuarioDeAsignados()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = CrearUsuarioValido();
        usuario.Id = 1;
        tarea.AsignarUsuario(usuario);
        tarea.EliminarUsuario(1);
        Assert.IsFalse(tarea.UsuariosAsignados.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarUsuario_LanzaExcepcionSiUsuarioNoExiste()
    {
        Tarea tarea = CrearTareaValida();
        Usuario usuario = CrearUsuarioValido();
        usuario.Id = 1;
        tarea.AsignarUsuario(usuario);
        tarea.EliminarUsuario(3);
    }

    [TestMethod]
    public void AgregarRecurso_AgregarRecursoALista()
    {
        Tarea tarea = CrearTareaValida();
        Recurso recurso = CrearRecursoValido();
        tarea.AgregarRecurso(recurso); 
        Assert.IsTrue(tarea.RecursosNecesarios.Contains(recurso));
        Assert.AreEqual(1, tarea.RecursosNecesarios.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoEsNull()
    {
        Tarea tarea = CrearTareaValida();
        Recurso recurso = null;
        tarea.AgregarRecurso(recurso); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarRecurso_LanzarExcepcionSiRecursoYaEstaEnRecursosNecesarios()
    {
        Recurso recurso = CrearRecursoValido();
        Tarea tarea = CrearTareaValida();
        tarea.AgregarRecurso(recurso);
        tarea.AgregarRecurso(recurso);
    }
    
    [TestMethod]
    public void EliminarRecurso_EliminarRecursoDeNecesarios()
    { 
        Recurso recurso = CrearRecursoValido();
        recurso.Id = 1;
        Tarea tarea = CrearTareaValida();
        tarea.AgregarRecurso(recurso);
        tarea.EliminarRecurso(1);
        Assert.IsFalse(tarea.RecursosNecesarios.Any(t => t.Id == 1));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarRecurso_LanzaExcepcionSiRecursoNoExiste()
    { 
        Recurso recurso = CrearRecursoValido();
        recurso.Id = 1;
        Tarea tarea = CrearTareaValida();
        tarea.AgregarRecurso(recurso);
        tarea.EliminarRecurso(3);
    }
    
    [TestMethod]
    public void ModificarTitulo_DeberiaActualizarElTitulo()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarTitulo("titulo nuevo");
        Assert.AreEqual("titulo nuevo", tarea.Titulo);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsNull()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarTitulo(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarTitulo_LanzaExcepcionSiTituloEsVacio()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarTitulo("");
    }

    [TestMethod]
    public void ModificarDescripcion_DeberiaActualizarLaDescripcion()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarDescripcion("Desc nueva");
        Assert.AreEqual("Desc nueva", tarea.Descripcion);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsNull()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarDescripcion(null);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDescripcion_LanzaExcepcionSiDescripcionEsVacia()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarTitulo("");
    }
    
    [TestMethod]
    public void ModificarFechaInicioMasTemprana_ActualizaLaFechaOK()
    { 
        Tarea tarea = CrearTareaValida();
        DateTime nuevaFecha = DateTime.Today.AddDays(10);
        tarea.ModificarFechaInicioMasTemprana(nuevaFecha);
        Assert.AreEqual(nuevaFecha, tarea.FechaInicioMasTemprana);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarFechaInicioMasTemprana_LanzaExcepcionSiFechaEsAnteriorAHoy()
    {
        Tarea tarea = CrearTareaValida();
        DateTime fechaPasada = DateTime.Now.AddDays(-1);
        tarea.ModificarFechaInicioMasTemprana(fechaPasada);
    } 

    [TestMethod]
    public void ModificarDuracion_ActualizaLaDuracionOK()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarDuracion(4);
        Assert.AreEqual(4, tarea.DuracionEnDias);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void ModificarDuracion_LanzaExcepcionSiEsCeroONegativa()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarDuracion(-2);
        tarea.ModificarDuracion(0);
    }
    
    [TestMethod]
    public void DarListaAsignados_DevuelveListaDeUsuariosAsignados()
    {
        Usuario usuario1 = CrearUsuarioValido();
        Usuario usuario2 = new Usuario("Juana", "Pereza", new DateTime(1996, 2, 2), "juana@test.com", "TaskTrackPro@2025");
        usuario1.Id = 1; // se hardcodean ids por simplicidad de tests. 
        usuario2.Id = 2;
        Tarea tarea = CrearTareaValida();
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
        Recurso necesario = CrearRecursoValido();
        necesario.Id = 1;
        Recurso necesario2 = new Recurso("recurso2", "tipo2", "descripcion" );
        necesario2.Id = 2;
        Tarea tarea = CrearTareaValida();
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
        Usuario miembro = CrearUsuarioValido();
        Tarea tarea = CrearTareaValida();
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
        Tarea tarea = CrearTareaValida();
        DateTime fechaEsperada = tarea.FechaInicioMasTemprana.AddDays(tarea.DuracionEnDias - 1);
        Assert.AreEqual(fechaEsperada, tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void ModificarDuracionActualizaFechaFinMasTemprana()
    {
        Tarea tarea = CrearTareaValida();
        tarea.ModificarDuracion(5);
        Assert.AreEqual(tarea.FechaInicioMasTemprana.AddDays(5 - 1), tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void ModificarFechaInicioMasTempranaActualizaFechaFinMasTemprana()
    {
        int duracion = 5;
        Tarea tarea = CrearTareaValida(); // duración 5
        DateTime nuevaFechaInicio = tarea.FechaInicioMasTemprana.AddDays(2);
        tarea.ModificarFechaInicioMasTemprana(nuevaFechaInicio);
        Assert.AreEqual(nuevaFechaInicio.AddDays(duracion - 1), tarea.FechaFinMasTemprana);
    }
    
    [TestMethod]
    public void AgregarDependencia_AgregarDependenciaALista()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia); 
        Assert.IsTrue(tarea.Dependencias.Contains(dependencia));
        Assert.AreEqual(1, tarea.Dependencias.Count);
    }
    
    [TestMethod]
    public void AgregarDependencia_AgregarDependenciaAListaYModificaEstado()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia); 
        Assert.AreEqual(EstadoTarea.Bloqueada, tarea.Estado);
    }
    
    [TestMethod]
    public void AgregarDependenciaCompletada_AgregarDependenciaAListaYModificaEstado()
    {
        Tarea tarea = CrearTareaValida();
        
        Dependencia dependencia = CrearDependenciaValida();
        dependencia.Tarea.CambiarEstado(EstadoTarea.EnProceso);
        dependencia.Tarea.CambiarEstado(EstadoTarea.Completada);
        
        tarea.AgregarDependencia(dependencia); 
        Assert.AreEqual(EstadoTarea.Pendiente, tarea.Estado);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaEsNull()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = null;
        tarea.AgregarDependencia(dependencia); 
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void AgregarDependencia_LanzarExcepcionSiDependenciaYaEstaEnDependencias()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        tarea.AgregarDependencia(dependencia);
    }

    [TestMethod]
    public void EliminarDependencia_EliminarDependenciaDeDependencias()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        tarea.EliminarDependencia(1);
        Assert.IsFalse(tarea.Dependencias.Any(d => d.Tarea.Id == 1));
    }

    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void EliminarDependencia_LanzaExcepcionSiDepenciaNoExiste()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        
        tarea.EliminarDependencia(3);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void VerificarDependenciaNoEstaAgregada_YaExisteLanzaExcepcion()
    {
        Tarea tarea = CrearTareaValida();
        Dependencia dependencia = CrearDependenciaValida();
        tarea.AgregarDependencia(dependencia);
        tarea.AgregarDependencia(dependencia);
    }
    
    [TestMethod]
    public void EqualsRetornaTrueSiLosIdsSonIguales()
    {
        Tarea tarea1 = CrearTareaValida();
        tarea1.Id = 1;
        Tarea tarea2 = CrearTareaValida();
        tarea2.Id = 1;
        bool sonIguales = tarea1.Equals(tarea2);
        Assert.IsTrue(sonIguales);
    }
    
    [TestMethod]
    public void EqualsRetornaFalseSiLosIdsNoSonIguales()
    {
        Tarea tarea1 = CrearTareaValida();
        tarea1.Id = 1;
        Tarea tarea2 = CrearTareaValida();
        tarea2.Id = 2;
        bool sonIguales = tarea1.Equals(tarea2);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoEsNull()
    { 
        Tarea tarea = CrearTareaValida();
        bool sonIguales = tarea.Equals(null);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoNoEsTarea()
    {
        Tarea tarea = CrearTareaValida();
        int otro = 0;
        bool sonIguales = tarea.Equals(otro);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod] 
    public void GetHashCodeFuncionaOk()
    {
        Tarea tarea1 = CrearTareaValida();
        Tarea tarea2 = CrearTareaValida();
        //ambos tienen mismo id ya que no hay un gestor que maneje ids
        Tarea tarea3 = CrearTareaValida();
        tarea3.Id = 3; //  id distinto a los otros 2 (se hardcodea en vez de llamar al gestor por simplicidad)
        Assert.AreEqual(tarea1.GetHashCode(), tarea2.GetHashCode());
        Assert.AreNotEqual(tarea3.GetHashCode(), tarea1.GetHashCode());
    }
    
    // HELPERS
    private Tarea CrearTareaValida()
    {
        Tarea tarea = new Tarea("Título", "Descripción", 5, _fechaInicio);
        return tarea;
    }

    private Recurso CrearRecursoValido()
    {
        Recurso recurso = new Recurso("recurso", "tipo", "descripcion" );
        return recurso;
    }

    private Usuario CrearUsuarioValido()
    {
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
        return usuario;
    }

    private Dependencia CrearDependenciaValida()
    {
        Tarea tareaD = new Tarea("TituloD","DescripciónD",3, _fechaInicio);
        tareaD.Id = 1;
        Dependencia dependencia = new Dependencia("SS", tareaD);
        return dependencia;
    }
    
}
    


