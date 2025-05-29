using System.Security.Cryptography;
using Dominio.Excepciones;

namespace Dominio;
public class Tarea
{
    public int Id {get; set;}
    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public int DuracionEnDias { get; private set; }
    public DateTime FechaInicioMasTemprana { get; private set; }
    public DateTime FechaFinMasTemprana { get; private set; }
    public DateTime FechaDeEjecucion { get; private set; } = DateTime.MinValue;
    public EstadoTarea Estado { get; private set; } = EstadoTarea.Pendiente;
    public int Holgura {get; set;}
    public List<Usuario> UsuariosAsignados { get; }
    public List<Recurso> RecursosNecesarios { get; }
    public List<Dependencia> Dependencias { get; }

    public Tarea()
    {
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<Recurso>();
        Dependencias = new List<Dependencia>();
    }

    public Tarea(string titulo, string descripcion, int duracionEnDias,  DateTime fechaInicioMasTemprana)
    {
        ValidarStringNoVacioNiNull(titulo, "El título de la tarea no puede estar vacío o ser nulo.");
        ValidarIntNoNegativoNiCero(duracionEnDias, "La duración no puede ser un número negativo.");
        ValidarStringNoVacioNiNull(descripcion, "La descripción no puede estar vacía ni nula.");
        ValidarFechaInicio(fechaInicioMasTemprana);
        Titulo = titulo;
        Descripcion = descripcion;
        DuracionEnDias = duracionEnDias;
        FechaInicioMasTemprana  = fechaInicioMasTemprana;
        Estado = EstadoTarea.Pendiente;
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<Recurso>();
        Dependencias = new List<Dependencia>();
        CalcularFechaFinMasTemprana();
    }
    
    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        EstadoCompletadaAPendiente(nuevoEstado);
        EstadoCompletadaAEnProceso(nuevoEstado);
        EstadoCompletadaABloqueada(nuevoEstado);
        EstadoEnProcesoAPendiente(nuevoEstado);
        EstadoPendienteACompletada(nuevoEstado);
        EstadoBloqueadaACompletada(nuevoEstado);
        Estado = nuevoEstado;
        if (nuevoEstado == EstadoTarea.EnProceso)
        {
            ModificarFechaInicioMasTemprana(DateTime.Today);
        }
        else if (nuevoEstado == EstadoTarea.Completada)
        {
            LiberarRecursos();
            FechaDeEjecucion = DateTime.Today;
            FechaFinMasTemprana = DateTime.Today;
            DuracionEnDias = (int)(FechaFinMasTemprana - FechaInicioMasTemprana).TotalDays + 1;
        }
    }
    
    public void AsignarUsuario(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario,"No se puede asignar una tarea a un usuario null.");
        VerificarUsuarioNoEstaAsignado(usuario);
        UsuariosAsignados.Add(usuario);
    }
    public void EliminarUsuario(int idUsuario)
    {
        VerificarMiembrosNoVacia();
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);
        ValidarObjetoNoNull(usuarioAEliminar,"El usuario no está asignado a la tarea.");
        UsuariosAsignados.Remove(usuarioAEliminar);
    }
    
    public void AgregarRecurso(Recurso recurso)
    {
        ValidarObjetoNoNull(recurso,"No se puede agregar un recurso null.");
        VerificarRecursoNoEstaAgregado(recurso);
        RecursosNecesarios.Add(recurso);
        recurso.IncrementarCantidadDeTareasUsandolo();
    }
    public void EliminarRecurso(int idRecurso)
    {
        Recurso recursoAEliminar = BuscarRecursoPorId(idRecurso);
        ValidarObjetoNoNull(recursoAEliminar,"El recurso no se encuentra dentro de los recursos necesarios.");
        RecursosNecesarios.Remove(recursoAEliminar);
    }
    
    public void AgregarDependencia(Dependencia dependencia)
    {
        ValidarObjetoNoNull(dependencia,"No se puede agregar una dependencia null.");
        VerificarDependenciaNoEstaAgregada(dependencia);
        Dependencias.Add(dependencia);
        ActualizarEstadoBloqueadaOPendiente();
    }
    
    public void EliminarDependencia(int idTarea)
    {
        Dependencia dependenciaAEliminar = BuscarDependenciaPorIdDeTarea(idTarea);
        ValidarObjetoNoNull(dependenciaAEliminar,"La dependencia no se encuentra dentro de la lista de dependencias.");
        Dependencias.Remove(dependenciaAEliminar);
        ActualizarEstadoBloqueadaOPendiente();
    }
    
    public void ModificarTitulo(string tituloNuevo)
    {
        ValidarStringNoVacioNiNull(tituloNuevo,"El titulo no puede estar vacío");
        Titulo = tituloNuevo;
    }
    
    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarStringNoVacioNiNull(nuevaDescripcion,"La descripción no puede estar vacía");
        Descripcion = nuevaDescripcion;
    }
    
    public void ModificarDuracion(int nuevaDuracion)
    {
        DuracionNoMenorACero(nuevaDuracion);
        DuracionEnDias = nuevaDuracion;
        CalcularFechaFinMasTemprana();
    }
    
    public void ModificarFechaInicioMasTemprana(DateTime fechaInicioNueva)
    {
        VerificarFechaNoMenorAHoy(fechaInicioNueva);
        FechaInicioMasTemprana = fechaInicioNueva;
        CalcularFechaFinMasTemprana();
    }
    
    public bool EsCritica()
    {
        return Holgura == 0;
    }
    
    public void ActualizarEstadoBloqueadaOPendiente()
    {
        if (Estado == EstadoTarea.Bloqueada || Estado == EstadoTarea.Pendiente)
        {
            if (SePuedeRealizar())
            {
                CambiarEstado(EstadoTarea.Pendiente);
            }
            else
            {
                CambiarEstado(EstadoTarea.Bloqueada);
            }
        }
    }
    
    public bool EsMiembro(Usuario usuario)
    {
        return UsuariosAsignados.Contains(usuario);
    }

    public bool EsSucesoraDe(int idTarea)
    {
        return Dependencias.Any(d => d.Tarea.Id == idTarea);
    }

    private void CalcularFechaFinMasTemprana()
    {
        FechaFinMasTemprana = FechaInicioMasTemprana.AddDays(DuracionEnDias - 1);
    }
    
    private void VerificarFechaNoMenorAHoy(DateTime fechaNueva)
    {
        if (fechaNueva < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha no puede ser anterior a hoy.");
    }
    
    private void DuracionNoMenorACero(int duracion)
    {
        if (duracion <= 0)
            throw new ExcepcionDominio("La duración no puede ser cero o negativa.");
    }
    
    private void VerificarMiembrosNoVacia()
    {
        if (!UsuariosAsignados.Any())
        {
            throw new ExcepcionDominio("La lista de usuarios asignados está vacía o no está inicializada.");
        }
    }
    
    private void ValidarStringNoVacioNiNull(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarIntNoNegativoNiCero(int valor, string mensajeError)
    {
        if (valor <= 0)
            throw new ExcepcionDominio(mensajeError);
    }
    
    private void ValidarFechaInicio(DateTime? fechaInicio)
    {
        if (fechaInicio.HasValue && fechaInicio.Value.Date < DateTime.Today)
        {
            throw new ExcepcionDominio("La fecha de inicio debe ser igual o posterior a la fecha de hoy.");
        }
    }
    
    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    private void EstadoCompletadaAPendiente(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a pendiente.");
        }
    }
    
    private void EstadoCompletadaAEnProceso(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.EnProceso)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a en proceso.");
        }
    }
    
    private void EstadoCompletadaABloqueada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Bloqueada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a bloqueada.");
        }
    }
    
    private void EstadoEnProcesoAPendiente(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.EnProceso && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea en proceso a pendiente.");
        }
    }
    
    private void EstadoPendienteACompletada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Pendiente && nuevoEstado == EstadoTarea.Completada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea pendiente a completada.");
        }
    }
    
    private void EstadoBloqueadaACompletada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Bloqueada && nuevoEstado == EstadoTarea.Completada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea bloqueada a completada.");
        }
    }
    
    private void VerificarUsuarioNoEstaAsignado(Usuario usuario)
    {
        if(UsuariosAsignados.Contains(usuario))
            throw new ExcepcionDominio("El usuario ya fue agregado a la tarea.");
    }
    
    private void VerificarRecursoNoEstaAgregado(Recurso recurso)
    {
        if(RecursosNecesarios.Contains(recurso))
            throw new ExcepcionDominio("El recurso ya fue agregado.");
    }
    
    private void VerificarDependenciaNoEstaAgregada(Dependencia dependencia)
    {
        if (Dependencias.Contains(dependencia))
            throw new ExcepcionDominio("La dependencia ya fue agregada.");
    }
    
    private Usuario BuscarUsuarioPorId(int id)
    {
        return UsuariosAsignados.FirstOrDefault(u => u.Id == id);
    }
    
    private Recurso BuscarRecursoPorId(int id)
    {
        return RecursosNecesarios.FirstOrDefault(r => r.Id == id);
    }
    
    private Dependencia BuscarDependenciaPorIdDeTarea (int id)
    {
        return Dependencias.FirstOrDefault(d =>d.Tarea.Id  == id);
    }

    private void LiberarRecursos()
    {
        foreach (var recurso in RecursosNecesarios)
        {
            recurso.DecrementarCantidadDeTareasUsandolo();
        }
    }

    private bool SePuedeRealizar()
    {
        bool dependenciasFSCompletas =
            Dependencias.Where(d => d.Tipo == "FS").All(d => d.Tarea.Estado == EstadoTarea.Completada);
        bool dependenciasSSEnProcesoOCompletadas = Dependencias.Where(d => d.Tipo == "SS")
            .All(d => d.Tarea.Estado == EstadoTarea.EnProceso || d.Tarea.Estado == EstadoTarea.Completada);
        return dependenciasFSCompletas && dependenciasSSEnProcesoOCompletadas;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Tarea otra) return false;
        return Id == otra.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}