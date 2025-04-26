namespace Dominio;
using Dominio.Dummies;

public class Tarea
{
    public string Titulo;
    public string Descripcion;
    public int DuracionEnDias;
    public DateTime FechaInicioMasTemprana;
    // public DateTime FechaFinMasTemprana;
    // public DateTime FechaDeEjecucion;
    public EstadoTarea Estado = EstadoTarea.Pendiente;
    // public float Holgura;
    public List<Usuario> UsuariosAsignados;
    public List<Recurso> RecursosNecesarios;
    public List<Tarea> DependenciasFF;
    public List<Tarea> DependenciasFS;

    public Tarea(string unTitulo, string unDescripcion, int unaDuracionEnDias,  DateTime unaFechaInicioMasTemprana)
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