using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Proyecto
{
    private static int _maximoCaracteresDescripcion = 400;
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; }
    public Usuario Administrador { get; set; }
    
    public Usuario? Lider { get; private set; }    public List<Usuario> Miembros { get; }
    public DateTime FechaInicio { get; set; } = DateTime.Today;
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MaxValue;

    public Proyecto(string nombre, string descripcion, DateTime fechaInicio, Usuario administrador,
        List<Usuario> miembros)
    {
        ValidarTextoObligatorio(nombre, MensajesErrorDominio.NombreProyectoVacio);
        ValidarTextoObligatorio(descripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarNoNulo(administrador, MensajesErrorDominio.ProyectoSinAdministrador);
        ValidarNoNulo(miembros, MensajesErrorDominio.MiembrosProyectoNull);
        ValidarLargoDescripción(descripcion);
        ValidarFechaInicioMayorAActual(fechaInicio);

        Miembros = miembros;
        ValidarAdministradorEsteEnMiembros(administrador);

        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        FechaInicio = fechaInicio;
        FechaFinMasTemprana = fechaInicio.AddDays(100000);
        miembros.ForEach(usuario => usuario.CantidadProyectosAsignados++);
    }

    public void AgregarTarea(Tarea tarea)
    {
        ValidarNoNulo(tarea, MensajesErrorDominio.TareaNull);
        ValidarTareaNoDuplicada(tarea);

        Tareas.Add(tarea);
    }

    public void EliminarTarea(int idTarea)
    {
        Tarea tareaAEliminar = BuscarTareaPorId(idTarea);

        ValidarNoNulo(tareaAEliminar, MensajesErrorDominio.TareaNoPerteneceAlProyecto);

        Tareas.Remove(tareaAEliminar);
    }

    public void AsignarMiembro(Usuario usuario)
    {
        ValidarNoNulo(usuario, MensajesErrorDominio.MiembroNull);
        ValidarUsuarioNoSeaMiembro(usuario);

        Miembros.Add(usuario);
        usuario.CantidadProyectosAsignados++;
    }

    public void EliminarMiembro(int idUsuario)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsuario);

        ValidarNoNulo(usuarioAEliminar, MensajesErrorDominio.UsuarioNoEsMiembroDelProyecto);
        ValidarQueUsuarioAEliminarNoSeaAdministrador(usuarioAEliminar);

        Miembros.Remove(usuarioAEliminar);
        usuarioAEliminar.CantidadProyectosAsignados--;
    }

    public bool EsAdministrador(Usuario usuario)
    {
        return Administrador.Equals(usuario);
    }

    public void ModificarFechaInicio(DateTime nuevaFecha)
    {
        ValidarFechaInicioMayorAActual(nuevaFecha);
        ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(nuevaFecha);
        ValidarFechaInicioMenorAFechaFinMasTemprana(nuevaFecha, FechaFinMasTemprana);

        FechaInicio = nuevaFecha;
    }

    public void ModificarFechaFinMasTemprana(DateTime nuevaFecha)
    {
        ValidarFechaFinMayorAInicio(nuevaFecha);
        ValidarFechaFinNoMenorALaDeLasTareas(nuevaFecha);

        FechaFinMasTemprana = nuevaFecha;
    }

    public void ModificarNombre(string nombreNuevo)
    {
        ValidarTextoObligatorio(nombreNuevo, MensajesErrorDominio.NombreProyectoVacio);

        Nombre = nombreNuevo;
    }

    public void ModificarDescripcion(string nuevaDescripcion)
    {
        ValidarTextoObligatorio(nuevaDescripcion, MensajesErrorDominio.DescripcionVacia);
        ValidarLargoDescripción(nuevaDescripcion);

        Descripcion = nuevaDescripcion;
    }

    public void AsignarNuevoAdministrador(Usuario nuevoAdministrador)
    {
        ValidarUsuarioEnMiembros(nuevoAdministrador.Id);

        foreach (Usuario usuario in Miembros)
        {
            if (usuario.Equals(nuevoAdministrador))
            {
                Administrador.EstaAdministrandoUnProyecto = false;
                Administrador = usuario;
                Administrador.EstaAdministrandoUnProyecto = true;
            }
        }
    }
    
    public void AsignarLider(Usuario usuario)
    {
        ValidarNoNulo(usuario, MensajesErrorDominio.LiderNull);
        ValidarUsuarioEnMiembros(usuario.Id);
        if (Lider != null && Lider.Equals(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.UsuarioYaEsLider);
        }
        Lider = usuario;
        Lider.AsignarRolLider();
    }

    public bool EsMiembro(int idUsuario)
    {
        return Miembros.Any(u => u.Id == idUsuario);
    }

    public bool TieneTareas()
    {
        return Tareas.Any();
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
        if (descripcion.Length > _maximoCaracteresDescripcion)
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.DescripcionMuyLarga,
                _maximoCaracteresDescripcion));
        }
    }

    private void ValidarTextoObligatorio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }

    private void ValidarNoNulo(object objeto, string mensajeError)
    {
        if (objeto is null)
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }

    private void ValidarTareaNoDuplicada(Tarea tarea)
    {
        if (Tareas.Contains(tarea))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.TareaYaAgregada);
        }
    }

    private void ValidarUsuarioEnMiembros(int idUsuario)
    {
        Usuario usuario = BuscarUsuarioPorId(idUsuario);
        ValidarNoNulo(usuario, MensajesErrorDominio.UsuarioNoEsMiembroDelProyecto);
    }

    private void ValidarQueUsuarioAEliminarNoSeaAdministrador(Usuario usuario)
    {
        if (EsAdministrador(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.NoPuedeEliminarAdmin);
        }
    }

    private void ValidarAdministradorEsteEnMiembros(Usuario administrador)
    {
        if (!Miembros.Contains(administrador))
        {
            Miembros.Add(administrador);
        }
    }

    private void ValidarUsuarioNoSeaMiembro(Usuario usuario)
    {
        if (Miembros.Contains(usuario))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.MiembroYaEnProyecto);
        }
    }

    private void ValidarFechaInicioMayorAActual(DateTime fecha)
    {
        if (fecha < DateTime.Today)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMenorAHoy);
        }
    }

    private void ValidarFechaInicioNoPosteriorAFechaInicioDeTareas(DateTime nuevaFecha)
    {
        if (Tareas.Any(t => nuevaFecha > t.FechaInicioMasTemprana))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMayorQueTareas);
        }
    }

    private void ValidarFechaFinMayorAInicio(DateTime fecha)
    {
        if (fecha < FechaInicio)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaFinProyectoMenorQueInicio);
        }
    }

    private void ValidarFechaFinNoMenorALaDeLasTareas(DateTime fecha)
    {
        if (Tareas.Any(tarea => tarea.FechaFinMasTemprana > DateTime.MinValue && fecha < tarea.FechaFinMasTemprana))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaFinProyectoMenorQueTareas);
        }
    }

    private void ValidarFechaInicioMenorAFechaFinMasTemprana(DateTime inicio, DateTime fin)
    {
        if (inicio > fin)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoMayorQueFin);
        }

        if (inicio == fin)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.FechaInicioProyectoIgualFin);
        }
    }

    public override bool Equals(object? otro)
    {
        Proyecto otroProyecto = otro as Proyecto;
        return otroProyecto != null && Id == otroProyecto.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}