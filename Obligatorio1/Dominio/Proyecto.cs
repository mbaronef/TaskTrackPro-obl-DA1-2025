using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class Proyecto
{
    public int Id { get; private set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; set; }
    public Usuario Administrador { get; set; }
    public List<Usuario> Miembros { get; set; }
    
    public DateTime FechaInicio { get; set; } = DateTime.Now;
    
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MinValue;

    public Proyecto(string nombre, string descripcion, Usuario administrador, List<Usuario> miembros)
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
        {
            miembros.Add(administrador); // ASEGURA QUE EL ADMIN SIEMPRE ESTE EN MIEMBROS DEL PROYECTO
        }
        
        if (descripcion.Length > 400)
            throw new ExcepcionDominio("La descripción del proyecto no puede superar los 400 caracteres.");

        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        Miembros = miembros;
    }
    
    // Constructor con Id: lo usaría solo el Gestor
    public Proyecto(int id, string nombre, string descripcion, Usuario administrador, List<Usuario> miembros)
        : this(nombre, descripcion, administrador, miembros)
    {
        Id = id;
    }
    
    public void AgregarTarea(Tarea tarea)
    {
        if(tarea is null)
            throw new ExcepcionDominio("No se puede agregar una tarea null.");
        
        if(Tareas.Contains(tarea))
            throw new ExcepcionDominio("La tarea ya fue agregada al proyecto.");
        
        Tareas.Add(tarea);
    }
    
    public void EliminarTarea(int idTarea)
    {
        Tarea tareaAEliminar = null;

        foreach (Tarea tarea in Tareas)
        {
            if (tarea.Id == idTarea)
            {
                tareaAEliminar = tarea;
                break;
            }
        }

        if (tareaAEliminar is null)
            throw new ExcepcionDominio("La tarea no pertenece al proyecto.");

        Tareas.Remove(tareaAEliminar);
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
    
    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        if (nuevaFecha < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha de inicio no puede ser anterior a hoy.");

        FechaInicio = nuevaFecha;
    }

    public void ModificarNombre(string nombreNuevo)
    {
        if (string.IsNullOrWhiteSpace(nombreNuevo))
            throw new ExcepcionDominio("El nombre no puede estar vacío");
        
        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        if (string.IsNullOrWhiteSpace(nuevaDescripcion))
            throw new ExcepcionDominio("La descripción no puede estar vacía");

        if (nuevaDescripcion.Length > 400)
            throw new ExcepcionDominio("La descripción no puede superar los 400 caracteres");

        Descripcion = nuevaDescripcion;
    }
    
    public void AsignarNuevoAdministrador(int idNuevoAdministrador)
    {
        foreach (Usuario usuario in Miembros)
        {
            if (usuario.Id == idNuevoAdministrador)
            {
                Administrador = usuario;
                return;
            }
        }

        throw new ExcepcionDominio("El nuevo administrador debe ser miembro del proyecto.");
    }

    public List<Usuario> DarListaMiembros()
    {
        return Miembros;
    }

    public List<Tarea> DarListaTareas()
    {
        return Tareas;
    }
    
    public void NotificarMiembros(string mensaje)
    {
        foreach (Usuario usuario in Miembros)
        {
            Notificacion nuevaNotificacion = new Notificacion(mensaje);
            usuario.AgregarNotificacion(nuevaNotificacion);
        }
    }

    public void NotificarAdministrador(string mensaje)
    {
        Notificacion notificacion = new Notificacion(mensaje);
        Administrador.AgregarNotificacion(notificacion);
    }
    
    public List<Recurso> DarRecursosFaltantes()
    {
        List<Recurso> faltantes = new List<Recurso>();

        foreach (Tarea tarea in Tareas)
        {
            foreach (Recurso recurso in tarea.RecursosNecesarios)
            {
                if (!recurso.EnUso && !faltantes.Any(r => r.Id == recurso.Id))
                {
                    faltantes.Add(recurso);
                }
            }
        }

        return faltantes;
    }
}