namespace Dominio;
using Dominio.Dummies;

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

    public Tarea(string unTitulo, string unDescripcion, int unaDuracionEnDias,  DateTime? unaFechaInicioMasTemprana = null)
    {
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
    
    

}