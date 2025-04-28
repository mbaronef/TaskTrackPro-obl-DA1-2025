namespace Dominio;
using Dominio.Dummies;
using Dominio.Excepciones;

public class Tarea
{
    public int Id {get; set;}
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int DuracionEnDias { get; set; }
    public DateTime? FechaInicioMasTemprana { get; set; } //nullable
    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MinValue;
    public DateTime FechaDeEjecucion { get; set; } = DateTime.MinValue;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public float Holgura {get; set;}
    public List<Usuario> UsuariosAsignados { get; set; }
    public List<Recurso> RecursosNecesarios { get; set; }
    public List<Tarea> DependenciasFF { get; set; }
    public List<Tarea> DependenciasFS { get; set; }
    
    private void ValidarStringNoVacioNiNull(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarIntNoNegativoNiCero(int valor, string mensajeError)
    {
        if (valor <= 0)
            throw new ExcepcionDominio(mensajeError);
    }
    
    private void ValidarFechaInicio(DateTime? fechaInicio)
    {
        if (fechaInicio.HasValue && fechaInicio.Value.Date < DateTime.Today)
        {
            throw new ExcepcionDominio("La fecha de inicio debe ser igual o posterior a la fecha de hoy.");
        }
    }
    
    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    public Tarea(string unTitulo, string unDescripcion, int unaDuracionEnDias,  DateTime? unaFechaInicioMasTemprana = null)
    {
        ValidarStringNoVacioNiNull(unTitulo, "El título de la tarea no puede estar vacío o ser nulo.");
        ValidarIntNoNegativoNiCero(unaDuracionEnDias, "La duración no puede ser un número negativo.");
        ValidarStringNoVacioNiNull(unDescripcion, "La descrición no puede estar vacía ni nula.");
        ValidarFechaInicio(unaFechaInicioMasTemprana);
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
    
    public Tarea(int unId, string unTitulo, string unaDescripcion, int unaDuracionEnDias,  DateTime? unaFechaInicioMasTemprana = null)
        : this(unTitulo, unaDescripcion, unaDuracionEnDias, unaFechaInicioMasTemprana)
    {
        Id = unId;
    }
    
    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a pendiente.");
        }
        Estado = nuevoEstado;
    }
    
    public bool EsCritica()
    {
        return Holgura == 0;
    }
    
    public bool EsMiembro(Usuario usuario)
    {
        return UsuariosAsignados.Contains(usuario);
    }
    
    public void EliminarUsuarioAsignado(int id)
    {
        if (UsuariosAsignados == null)
        {
            throw new ExcepcionDominio("La lista de usuarios asignados está vacía o no está inicializada.");
        }
        for (int i = 0; i < UsuariosAsignados.Count; i++)
        {
            if (UsuariosAsignados[i].Id == id)
            {
                UsuariosAsignados.RemoveAt(i);
                return;
            }
        }
        throw new ExcepcionDominio("El usuario con el id dado no está asignado.");
    }
    
    public void AsignarUsuario(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario,"No se puede asignar una tarea a un usuario null.");
        if(UsuariosAsignados.Contains(usuario))
            throw new ExcepcionDominio("El usuario ya fue agregado a la tarea.");
        UsuariosAsignados.Add(usuario);
    }
    
    private Usuario BuscarUsuarioPorId(int id)
    {
        return UsuariosAsignados.FirstOrDefault(u => u.Id == id);
    }
    
    public void EliminarUsuario(int idUsu)
    {
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsu);
        
        ValidarObjetoNoNull(usuarioAEliminar,"El usuario no está asignado a la tarea.");

        UsuariosAsignados.Remove(usuarioAEliminar);
    }
    
    public void AgregarRecurso(Recurso recurso)
    {
        ValidarObjetoNoNull(recurso,"No se puede agregar un recurso null.");
        if(RecursosNecesarios.Contains(recurso))
            throw new ExcepcionDominio("El recurso ya fue agregado.");
        RecursosNecesarios.Add(recurso);
    }
    
    private Recurso BuscarRecursoPorId(int id)
    {
        return RecursosNecesarios.FirstOrDefault(u => u.Id == id);
    }
    public void EliminarRecurso(int idRec)
    {
        Recurso recursoAEliminar = BuscarRecursoPorId(idRec);
        
        ValidarObjetoNoNull(recursoAEliminar,"El recurso no se encuentra dentro de los recursos necesarios.");

        RecursosNecesarios.Remove(recursoAEliminar);
    }
    
    public void ModificarTitulo(string tituloNuevo)
    {
        ValidarStringNoVacioNiNull(tituloNuevo,"El titulo no puede estar vacío");
        Titulo = tituloNuevo;
    }
    

}