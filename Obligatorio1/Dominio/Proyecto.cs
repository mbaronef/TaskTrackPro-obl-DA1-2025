using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class Proyecto
{
    // public int Id { get; private set; }
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
        
        if (descripcion.Length > 400)
            throw new ExcepcionDominio("La descripción del proyecto no puede superar los 400 caracteres.");

        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = tareas;
        Administrador = administrador;
        Miembros = miembros;
    }
    
    // Constructor con Id: lo usaría solo el Gestor
    //public Proyecto(int id, string nombre, string descripcion, List<Tarea> tareas, Usuario administrador, List<Usuario> miembros)
     //   : this(nombre, descripcion, tareas, administrador, miembros)
    //{
     //   Id = id;
    //}
    
    public void AgregarTarea(Tarea tarea)
    {
        if(tarea is null)
            throw new ExcepcionDominio("No se puede agregar una tarea null.");
        
        if(Tareas.Contains(tarea))
            throw new ExcepcionDominio("La tarea ya fue agregada al proyecto.");
        
        Tareas.Add(tarea);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        if (usuario is null)
            throw new ExcepcionDominio("No se puede agregar un miembro null.");
        
        if (Miembros.Contains(usuario))
            throw new ExcepcionDominio("El miembro ya pertenece al proyecto.");
        
        Miembros.Add(usuario);
    }

    public void EliminarMiembro(int idUsuario)
    {
        foreach (Usuario usuario in Miembros)
        {
            if (usuario.Id == idUsuario)
            {
                if (usuario == Administrador)
                    throw new ExcepcionDominio(
                        "No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.");
                    
                Miembros.Remove(usuario);
                return; 
            }
        }

        // Si llega hasta aca el usuario no esta en miembros
        throw new ExcepcionDominio("El usuario no es miembro del proyecto.");
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador == usuario;
    }
}