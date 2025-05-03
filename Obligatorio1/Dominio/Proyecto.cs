using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

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
    
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.Now;
    
    public Proyecto(string nombre, string descripcion, Usuario administrador, List<Usuario> miembros) 
    {
        ValidarTextoObligatorio(nombre, "El nombre del proyecto no puede estar vacío o null.");
        ValidarTextoObligatorio(descripcion, "La descripción del proyecto no puede estar vacía o null.");
        ValidarNoNulo(administrador, "El proyecto debe tener un administrador.");
        ValidarNoNulo(miembros,"La lista de miembros no puede ser null.");
        ValidarLargoDescripción(descripcion);
        ValidarAdministradorEsteEnMiembros(administrador, miembros);
        
        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        Miembros = miembros;
    }
    
    public void AsignarId(int id)
    {
        Id = id;
    }
    
    public void AgregarTarea(Tarea tarea)
    {
        ValidarNoNulo(tarea,"No se puede agregar una tarea null.");
        ValidarTareaNoDuplicada(tarea);
        
        Tareas.Add(tarea);
    }
    
    public void EliminarTarea(int idTarea)
    {
        Tarea tareaAEliminar = BuscarTareaPorId(idTarea);

        ValidarNoNulo(tareaAEliminar,"La tarea no pertenece al proyecto.");

        Tareas.Remove(tareaAEliminar);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        ValidarNoNulo(usuario,"No se puede agregar un miembro null.");
        ValidarUsuarioNoSeaMiembro(usuario);
        
        Miembros.Add(usuario);
    }

    public void EliminarMiembro(int idUsuario)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);
        
        ValidarNoNulo(usuarioAEliminar,"El usuario no es miembro del proyecto.");
        ValidarQueUsuarioAEliminarNoSeaAdministrador(usuarioAEliminar);
        
        Miembros.Remove(usuarioAEliminar);
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador.Equals(usuario);
    }

    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        ValidarFechaInicioMayorAActual(nuevaFecha);
        ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(nuevaFecha);
        
        FechaInicio = nuevaFecha;
    }
    
    public void ModificarFechaFinMasTemprana(DateTime nuevaFecha)
    {
        ValidarFechaFinMayorAInicio(nuevaFecha);
        ValidarFechaFinNoMenorALaDeLasTareas(nuevaFecha);

        //FechaFinMasTemprana = nuevaFecha;
    }
    
    public void ModificarNombre(string nombreNuevo)
    {
        ValidarTextoObligatorio(nombreNuevo,"El nombre no puede estar vacío");
        
        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarTextoObligatorio(nuevaDescripcion,"La descripción no puede estar vacía");
        ValidarLargoDescripción(nuevaDescripcion);
        
        Descripcion = nuevaDescripcion;
    }
    
    public void AsignarNuevoAdministrador(int idNuevoAdministrador)
    {
        ValidarUsuarioEnMiembros(idNuevoAdministrador);
        
        foreach (Usuario usuario in Miembros)
        {
            if (usuario.Id == idNuevoAdministrador)
            {
                Administrador.EstaAdministrandoProyecto = false;
                Administrador = usuario;
                Administrador.EstaAdministrandoProyecto = true;
            }
        }
    }
    
    public void NotificarMiembros(string mensaje)
    {
        foreach (Usuario usuario in Miembros)
        {
            Notificacion nuevaNotificacion = new Notificacion(mensaje);
            usuario.RecibirNotificacion(nuevaNotificacion);
        }
    }

    public void NotificarAdministrador(string mensaje)
    {
        Notificacion notificacion = new Notificacion(mensaje);
        Administrador.RecibirNotificacion(notificacion);
    }

    public bool EsMiembro(int idUsuario)
    {
        return Miembros.Any(u => u.Id == idUsuario);
    }

    public bool EsMiembro(Usuario usuario)
    {
        return Miembros.Contains(usuario);
    }


    private Tarea BuscarTareaPorId(int id)
    {
        return Tareas.FirstOrDefault(t => t.Id == id);
    }

    private Usuario BuscarUsuarioPorId(int id)
    {
        return Miembros.FirstOrDefault(u => u.Id == id);
    }

    private void ValidarLargoDescripción(string descripcion)
    {
        if (descripcion.Length > MaximoCaracteresDescripcion)
            throw new ExcepcionDominio($"La descripción no puede superar los {MaximoCaracteresDescripcion} caracteres");
    }
    
    private void ValidarTextoObligatorio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }
    
    private void ValidarNoNulo(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarTareaNoDuplicada(Tarea tarea)
    {
        if(Tareas.Contains(tarea))
            throw new ExcepcionDominio("La tarea ya fue agregada al proyecto.");
    }

    private void ValidarUsuarioEnMiembros(int idUsuario)
    {
        Usuario usuario = BuscarUsuarioPorId(idUsuario);
        ValidarNoNulo(usuario, "El usuario no es miembro del proyecto.");
    }

    private void ValidarQueUsuarioAEliminarNoSeaAdministrador(Usuario usuario)
    {
        if (EsAdministrador(usuario))
            throw new ExcepcionDominio("No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.");
    }

    private void ValidarAdministradorEsteEnMiembros(Usuario administrador, List<Usuario> miembros)
    {
        if (!miembros.Contains(administrador))
        {
            miembros.Add(administrador);
        }
    }

    private void ValidarUsuarioNoSeaMiembro(Usuario usuario)
    {
        if (Miembros.Contains(usuario))
            throw new ExcepcionDominio("El miembro ya pertenece al proyecto.");
    }

    private void ValidarFechaInicioMayorAActual(DateTime fecha)
    {
        if (fecha < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha de inicio no puede ser anterior a hoy.");
    }
    
    private void ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(DateTime nuevaFecha)
    {
        if (Tareas.Any(t => t.FechaInicio < DateTime.MaxValue && nuevaFecha > t.FechaInicio))
            throw new ExcepcionDominio("La fecha de inicio no puede ser posterior a la de alguna tarea.");
    }
    
    private void ValidarFechaFinMayorAInicio(DateTime fecha)
    {
        if (fecha < FechaInicio)
            throw new ExcepcionDominio("La fecha de fin más temprana no puede ser anterior a la fecha de inicio del proyecto.");
    }
    
    private void ValidarFechaFinNoMenorALaDeLasTareas(DateTime fecha)
    {
        if (Tareas.Any(tarea => tarea.FechaFinMasTemprana > DateTime.MinValue && fecha < tarea.FechaFinMasTemprana))
            throw new ExcepcionDominio("La fecha de fin más temprana no puede ser menor que la fecha de fin de una tarea.");
    }


    // TODO:
    // FALTA:
    // agregar fecha inicio al constructor y verificar que sea mayor que la actual y menor a la fecha fin mas temprana
    // ModificarFechaFinMasTemprana, con sus respectivas validaciones:(que sea posterior a fechaInicio y que no sea menor a la fecha fin de ninguna de las tareas).
    
    // -> REFACTOR 
}