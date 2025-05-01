using Dominio.Excepciones;

namespace Dominio.Dummies;

public class Proyecto
{
     private const int MaximoCaracteresDescripcion = 400;
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; set; }
    public Usuario Administrador { get; set; }
    public List<Usuario> Miembros { get; set; }

    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MinValue;

    private void ValidarStringNoVacioNiNull(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    public Proyecto(string nombre, string descripcion, Usuario administrador, List<Usuario> miembros)
    {
        ValidarStringNoVacioNiNull(nombre, "El nombre del proyecto no puede estar vacío o null.");
        ValidarStringNoVacioNiNull(descripcion, "La descripción del proyecto no puede estar vacía o null.");
        ValidarObjetoNoNull(administrador, "El proyecto debe tener un administrador.");
        ValidarObjetoNoNull(miembros,"La lista de miembros no puede ser null.");
        if (!miembros.Contains(administrador))
        {
            miembros.Add(administrador); // ASEGURA QUE EL ADMIN SIEMPRE ESTE EN MIEMBROS DEL PROYECTO
        }
        if (descripcion.Length > MaximoCaracteresDescripcion)
            throw new ExcepcionDominio("La descripción del proyecto no puede superar los 400 caracteres.");
        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        Miembros = miembros;
    }

    private Tarea BuscarTareaPorId(int id)
    {
        return Tareas.FirstOrDefault(t => t.Id == id);
    }

    private Usuario BuscarUsuarioPorId(int id)
    {
        return Miembros.FirstOrDefault(u => u.Id == id);
    }

    public void AgregarTarea(Tarea tarea)
    {
        ValidarObjetoNoNull(tarea,"No se puede agregar una tarea null.");

        if(Tareas.Contains(tarea))
            throw new ExcepcionDominio("La tarea ya fue agregada al proyecto.");

        Tareas.Add(tarea);
    }

    public void EliminarTarea(int idTarea)
    {
        Tarea tareaAEliminar = BuscarTareaPorId(idTarea);

        ValidarObjetoNoNull(tareaAEliminar,"La tarea no pertenece al proyecto.");

        Tareas.Remove(tareaAEliminar);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario,"No se puede agregar un miembro null.");
        if (Miembros.Contains(usuario))
            throw new ExcepcionDominio("El miembro ya pertenece al proyecto.");
        Miembros.Add(usuario);
    }

    public void EliminarMiembro(int idUsuario)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);
        ValidarObjetoNoNull(usuarioAEliminar,"El usuario no es miembro del proyecto.");
        if (EsAdministrador(usuarioAEliminar))
            throw new ExcepcionDominio("No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.");
        Miembros.Remove(usuarioAEliminar);
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador.Id == usuario.Id;
    }

    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        if (nuevaFecha < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha de inicio no puede ser anterior a hoy.");
        FechaInicio = nuevaFecha;
    }

    public void ModificarNombre(string nombreNuevo)
    {
        ValidarStringNoVacioNiNull(nombreNuevo,"El nombre no puede estar vacío");
        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarStringNoVacioNiNull(nuevaDescripcion,"La descripción no puede estar vacía");
        if (nuevaDescripcion.Length > MaximoCaracteresDescripcion)
            throw new ExcepcionDominio($"La descripción no puede superar los {MaximoCaracteresDescripcion} caracteres");
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
    
}