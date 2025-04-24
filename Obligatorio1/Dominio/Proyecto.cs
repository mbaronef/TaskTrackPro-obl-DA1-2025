using Dominio.Dummies;
namespace Dominio;

public class Proyecto
{
    public string Nombre { get; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; set; }
    public Usuario Administrador { get; set; }
    public List<Usuario> Miembros { get; set; }
    
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public Proyecto(string nombre, string descripcion, List<Tarea> tareas, Usuario administrador, List<Usuario> miembros)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = tareas;
        Administrador = administrador;
        Miembros = miembros;
    }
}