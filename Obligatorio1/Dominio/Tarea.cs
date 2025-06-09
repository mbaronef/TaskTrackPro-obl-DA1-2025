using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Tarea
{
    public int Id { get; set; }
    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public int DuracionEnDias { get; private set; }
    public DateTime FechaInicioMasTemprana { get; private set; }
    public DateTime FechaFinMasTemprana { get; private set; }
    public DateTime FechaDeEjecucion { get; private set; } = DateTime.MinValue;
    public EstadoTarea Estado { get; private set; } = EstadoTarea.Pendiente;
    public int Holgura { get; set; }
    public List<Usuario> UsuariosAsignados { get; }
    public List<Recurso> RecursosNecesarios { get; }
    public List<Dependencia> Dependencias { get; }

    public Tarea()
    {
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<Recurso>();
        Dependencias = new List<Dependencia>();
    }

    public Tarea(string titulo, string descripcion, int duracionEnDias, DateTime fechaInicioMasTemprana)
    {
        ValidarStringNoVacioNiNull(titulo, MensajesErrorDominio.TituloTareaVacio);
        ValidarIntNoNegativoNiCero(duracionEnDias, MensajesErrorDominio.DuracionTareaInvalida);
        ValidarStringNoVacioNiNull(descripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarFechaInicio(fechaInicioMasTemprana);
        
        Titulo = titulo;
        Descripcion = descripcion;
        DuracionEnDias = duracionEnDias;
        FechaInicioMasTemprana = fechaInicioMasTemprana;
        Estado = EstadoTarea.Pendiente;
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<Recurso>();
        Dependencias = new List<Dependencia>();
        
        CalcularFechaFinMasTemprana();
    }

    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        ValidarTransicionInvalida(Estado, nuevoEstado);
        
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
            Holgura = 0;
        }
    }

    public void AsignarUsuario(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario, MensajesErrorDominio.UsuarioNullEnAsignacion);
        VerificarUsuarioNoEstaAsignado(usuario);
        
        UsuariosAsignados.Add(usuario);
    }

    public void EliminarUsuario(int idUsuario)
    {
        VerificarMiembrosNoVacia();
        Usuario usuarioAEliminar = BuscarMiembroPorId(idUsuario);
        ValidarObjetoNoNull(usuarioAEliminar, MensajesErrorDominio.UsuarioNoAsignado);
        UsuariosAsignados.Remove(usuarioAEliminar);
    }

    public void AsignarRecurso(Recurso recurso)
    {
        ValidarObjetoNoNull(recurso, MensajesErrorDominio.RecursoNullEnTarea);
        VerificarRecursoNoEstaAgregado(recurso);
        
        RecursosNecesarios.Add(recurso);
        recurso.IncrementarCantidadDeTareasUsandolo();
    }

    public void EliminarRecurso(int idRecurso)
    {
        Recurso recursoAEliminar = BuscarRecursoNecesarioPorId(idRecurso);
        ValidarObjetoNoNull(recursoAEliminar, MensajesErrorDominio.RecursoNoNecesario);

        recursoAEliminar.DecrementarCantidadDeTareasUsandolo();
        RecursosNecesarios.Remove(recursoAEliminar);
    }

    public void AgregarDependencia(Dependencia dependencia)
    {
        ValidarObjetoNoNull(dependencia, MensajesErrorDominio.DependenciaNullEnTarea);
        VerificarDependenciaNoEstaAgregada(dependencia);
        
        Dependencias.Add(dependencia);
        ActualizarEstadoBloqueadaOPendiente();
    }

    public void EliminarDependencia(int idTarea)
    {
        Dependencia dependenciaAEliminar = BuscarDependenciaPorIdDeTarea(idTarea);
        ValidarObjetoNoNull(dependenciaAEliminar, MensajesErrorDominio.DependenciaNoExisteEnTarea);
        
        Dependencias.Remove(dependenciaAEliminar);
        ActualizarEstadoBloqueadaOPendiente();
    }

    public void ModificarTitulo(string tituloNuevo)
    {
        ValidarStringNoVacioNiNull(tituloNuevo, MensajesErrorDominio.TituloTareaVacio);
        Titulo = tituloNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarStringNoVacioNiNull(nuevaDescripcion, MensajesErrorDominio.DescripcionVacia);
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
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaTareaInvalida);
        }
    }

    private void DuracionNoMenorACero(int duracion)
    {
        if (duracion <= 0)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.DuracionTareaInvalida);
        }
    }

    private void VerificarMiembrosNoVacia()
    {
        if (!UsuariosAsignados.Any())
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuariosAsignadosVacio);
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
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioInvalida);
        }
    }

    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarTransicionInvalida(EstadoTarea actual, EstadoTarea nuevo)
    {
        if ((actual == EstadoTarea.Completada && nuevo == EstadoTarea.Pendiente) ||
            (actual == EstadoTarea.Completada && nuevo == EstadoTarea.EnProceso) ||
            (actual == EstadoTarea.Completada && nuevo == EstadoTarea.Bloqueada) ||
            (actual == EstadoTarea.EnProceso && nuevo == EstadoTarea.Pendiente) ||
            (actual == EstadoTarea.Pendiente && nuevo == EstadoTarea.Completada) ||
            (actual == EstadoTarea.Bloqueada && nuevo == EstadoTarea.Completada))
        {
            throw new ExcepcionDominio(
                MensajesErrorDominio.TransicionEstadoInvalidaDesdeHacia(actual.ToString(), nuevo.ToString()));
        }
    }

    private void VerificarUsuarioNoEstaAsignado(Usuario usuario)
    {
        if (UsuariosAsignados.Contains(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuarioYaAgregado);
        }
    }

    private void VerificarRecursoNoEstaAgregado(Recurso recurso)
    {
        if (RecursosNecesarios.Contains(recurso))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.RecursoYaAgregado);
        }
    }

    private void VerificarDependenciaNoEstaAgregada(Dependencia dependencia)
    {
        if (Dependencias.Contains(dependencia))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.DependenciaYaAgregada);
        }
    }

    private Usuario BuscarMiembroPorId(int id)
    {
        return UsuariosAsignados.FirstOrDefault(u => u.Id == id);
    }

    private Recurso BuscarRecursoNecesarioPorId(int id)
    {
        return RecursosNecesarios.FirstOrDefault(r => r.Id == id);
    }

    private Dependencia BuscarDependenciaPorIdDeTarea(int id)
    {
        return Dependencias.FirstOrDefault(d => d.Tarea.Id == id);
    }

    private void LiberarRecursos()
    {
        foreach (Recurso recurso in RecursosNecesarios)
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