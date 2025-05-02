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

    // agregar fecha inicio
    // verificar que sea mayor que la actual
    public Proyecto(string nombre, string descripcion, Usuario administrador, List<Usuario> miembros)
    {
        ValidarStringNoVacioNiNull(nombre, "El nombre del proyecto no puede estar vacío o null.");
        ValidarStringNoVacioNiNull(descripcion, "La descripción del proyecto no puede estar vacía o null.");
        ValidarObjetoNoNull(administrador, "El proyecto debe tener un administrador.");
        ValidarObjetoNoNull(miembros, "La lista de miembros no puede ser null.");
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
        ValidarObjetoNoNull(tarea, "No se puede agregar una tarea null.");
        VerificarQueLaTareaNoExista(tarea);

        Tareas.Add(tarea);
    }
    

    public void AsignarMiembro(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario, "No se puede agregar un miembro null.");
        ValidarUsuarioNoSeaMiembro(usuario);

        Miembros.Add(usuario);
    }

    public void EliminarMiembro(int idUsuario)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);

        ValidarObjetoNoNull(usuarioAEliminar, "El usuario no es miembro del proyecto.");
        ValidarQueUsuarioAEliminarNoSeaAdministrador(usuarioAEliminar);

        Miembros.Remove(usuarioAEliminar);
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador.Equals(usuario);
    }


    // que no sea posterior a la de tarea mas temprana
    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        ValidarFechaInicioMenorAActual(nuevaFecha);

        FechaInicio = nuevaFecha;
    }

    //  falta ModificarFechaFinMasTemprana, con sus respectivas validaciones (que sea posterior a fechaInicio y que no sea menor a la fecha fin de las tareas).

    public void ModificarNombre(string nombreNuevo)
    {
        ValidarStringNoVacioNiNull(nombreNuevo, "El nombre no puede estar vacío");

        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarStringNoVacioNiNull(nuevaDescripcion, "La descripción no puede estar vacía");
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
                Administrador = usuario;
                return;
            }
        }

        throw new ExcepcionDominio("El nuevo administrador debe ser miembro del proyecto.");
    }

    public void NotificarMiembros(string mensaje)
    {
        foreach (Usuario usuario in Miembros)
        {
            usuario.RecibirNotificacion(mensaje);
        }
    }

    public void NotificarAdministrador(string mensaje)
    {
        Administrador.RecibirNotificacion(mensaje);
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

    private void VerificarQueLaTareaNoExista(Tarea tarea)
    {
        if (Tareas.Contains(tarea))
            throw new ExcepcionDominio("La tarea ya fue agregada al proyecto.");
    }

    private void ValidarUsuarioEnMiembros(int idUsuario)
    {
        Usuario usuario = BuscarUsuarioPorId(idUsuario);
        ValidarObjetoNoNull(usuario, "El usuario no es miembro del proyecto.");
    }

    private void ValidarQueUsuarioAEliminarNoSeaAdministrador(Usuario usuario)
    {
        if (EsAdministrador(usuario))
            throw new ExcepcionDominio(
                "No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.");
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

    private void ValidarFechaInicioMenorAActual(DateTime fecha)
    {
        if (fecha < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha de inicio no puede ser anterior a hoy.");
    }

}