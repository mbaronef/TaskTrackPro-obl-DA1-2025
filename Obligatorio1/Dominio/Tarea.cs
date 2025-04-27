namespace Dominio;
using Dominio.Dummies;
using Dominio.Excepciones;

public class Tarea
{
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int DuracionEnDias { get; set; }
    public DateTime? FechaInicioMasTemprana { get; set; } //nullable
    // public DateTime FechaFinMasTemprana;
    // public DateTime FechaDeEjecucion;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    // public float Holgura;
    public List<Usuario> UsuariosAsignados { get; set; }
    public List<Recurso> RecursosNecesarios { get; set; }
    public List<Tarea> DependenciasFF { get; set; }
    public List<Tarea> DependenciasFS { get; set; }
    
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

    public Tarea(string unTitulo, string unDescripcion, int unaDuracionEnDias,  DateTime? unaFechaInicioMasTemprana = null)
    {
        ValidarStringNoVacioNiNull(unTitulo, "El título de la tarea no puede estar vacío o ser nulo.");
        ValidarIntNoNegativoNiCero(unaDuracionEnDias, "La duración no puede ser un número negativo.");
        ValidarStringNoVacioNiNull(unDescripcion, "La descrición no puede estar vacía ni nula.");
        ValidarFechaInicio(unaFechaInicioMasTemprana);
        Titulo = unTitulo;
        Descripcion = unDescripcion;
        DuracionEnDias = unaDuracionEnDias;
        FechaInicioMasTemprana  = unaFechaInicioMasTemprana;
        Estado = EstadoTarea.Pendiente;
        UsuariosAsignados = new List<Usuario>();
        RecursosNecesarios = new List<Recurso>();
        DependenciasFF = new List<Tarea>();
        DependenciasFS = new List<Tarea>();
    }
    
    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a pendiente.");
        }
        Estado = nuevoEstado;
    }
    
    
    

}