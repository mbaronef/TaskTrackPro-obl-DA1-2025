using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class Proyecto
{
    public string Nombre { get; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; set; }
    public Usuario Administrador { get; set; }
    public List<Usuario> Miembros { get; set; }
    
    public DateTime FechaInicio { get; set; } = DateTime.Now;
    
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MinValue;

    public Proyecto(string nombre, string descripcion, List<Tarea> tareas, Usuario administrador, List<Usuario> miembros)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre del proyecto no puede estar vacío o null.");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ExcepcionDominio("La descripción del proyecto no puede estar vacía o null.");

        if (administrador is null)
            throw new ExcepcionDominio("El proyecto debe tener un administrador.");

        if (miembros is null)
            throw new ExcepcionDominio("La lista de miembros no puede ser null.");

        if (!miembros.Contains(administrador))
            throw new ExcepcionDominio("El administrador debe estar incluido en la lista de miembros.");

        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = tareas;
        Administrador = administrador;
        Miembros = miembros;
    }
    
    public void AgregarTarea(Tarea tarea)
    {
        Tareas.Add(tarea);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        if (usuario is null)
            throw new ExcepcionDominio("No se puede agregar un miembro null.");
        
        Miembros.Add(usuario);
    }
    
}