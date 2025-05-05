
﻿namespace Dominio;

using Excepciones;

public class Tarea
{
    public int Id {get; set;}
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int DuracionEnDias { get; set; }
    public DateTime FechaInicioMasTemprana { get; set; }
    public DateTime FechaFinMasTemprana { get; private set; }
    public DateTime FechaDeEjecucion { get; set; } = DateTime.MinValue;
    public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
    public float Holgura {get; set;}
    public List<Usuario> UsuariosAsignados { get; set; }
    public List<Recurso> RecursosNecesarios { get; set; }
    public List<Dependencia> Dependencias { get; set; }
    
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
    
    private void CalcularFechaFinMasTemprana()
    {
        FechaFinMasTemprana = FechaInicioMasTemprana.AddDays(DuracionEnDias-1);
    }


    public Tarea(string unTitulo, string unDescripcion, int unaDuracionEnDias,  DateTime unaFechaInicioMasTemprana)
    {
        ValidarStringNoVacioNiNull(unTitulo, "El título de la tarea no puede estar vacío o ser nulo.");
        ValidarIntNoNegativoNiCero(unaDuracionEnDias, "La duración no puede ser un número negativo.");
        ValidarStringNoVacioNiNull(unDescripcion, "La descripción no puede estar vacía ni nula.");
        ValidarFechaInicio(unaFechaInicioMasTemprana);
        this.Titulo = unTitulo;
        this.Descripcion = unDescripcion;
        this.DuracionEnDias = unaDuracionEnDias;
        this.FechaInicioMasTemprana  = unaFechaInicioMasTemprana;
        this.Estado = EstadoTarea.Pendiente;
        this.UsuariosAsignados = new List<Usuario>();
        this.RecursosNecesarios = new List<Recurso>();
        this.Dependencias = new List<Dependencia>();
        CalcularFechaFinMasTemprana();
    }

    private void EstadoCompletadaAPendiente(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a pendiente.");
        }
    }
    
    private void EstadoCompletadaAEnProceso(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.EnProceso)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a en proceso.");
        }
    }
    
    private void EstadoCompletadaABloqueada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Completada && nuevoEstado == EstadoTarea.Bloqueada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea finalizada a bloqueada.");
        }
    }
    
    private void EstadoEnProcesoAPendiente(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.EnProceso && nuevoEstado == EstadoTarea.Pendiente)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea en proceso a pendiente.");
        }
    }
    
    private void EstadoPendienteACompletada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Pendiente && nuevoEstado == EstadoTarea.Completada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea pendiente a completada.");
        }
    }
    
    private void EstadoBloqueadaACompletada(EstadoTarea nuevoEstado)
    {
        if (Estado == EstadoTarea.Bloqueada && nuevoEstado == EstadoTarea.Completada)
        {
            throw new ExcepcionDominio("No se puede cambiar una tarea bloqueada a completada.");
        }
    }

    private void AsignaRecursos()
    {
        foreach (Recurso recurso in RecursosNecesarios)
        {
            recurso.IncrementarCantidadDeTareasUsandolo();
        }
    }

    private void LiberaRecursos()
    {
        foreach (var recurso in RecursosNecesarios)
        {
            recurso.DecrementarCantidadDeTareasUsandolo();
        }
        FechaDeEjecucion = DateTime.Today;
    }
    
    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        EstadoCompletadaAPendiente(nuevoEstado);
        EstadoCompletadaAEnProceso(nuevoEstado);
        EstadoCompletadaABloqueada(nuevoEstado);
        EstadoEnProcesoAPendiente(nuevoEstado);
        EstadoPendienteACompletada(nuevoEstado);
        EstadoBloqueadaACompletada(nuevoEstado);
        Estado = nuevoEstado;
        if (nuevoEstado == EstadoTarea.EnProceso)
        {
            AsignaRecursos();
        }
        else if (nuevoEstado == EstadoTarea.Completada)
        {
            LiberaRecursos();
        }
    }
    
    public bool EsCritica()
    {
        return Holgura == 0;
    }
    
    public bool EsMiembro(Usuario usuario)
    {
        return UsuariosAsignados.Contains(usuario);
    }
    
    private void VerificarUsuarioNoEstaAsignado(Usuario usuario)
    {
        if(UsuariosAsignados.Contains(usuario))
            throw new ExcepcionDominio("El usuario ya fue agregado a la tarea.");
    }
    
    public void AsignarUsuario(Usuario usuario)
    {
        ValidarObjetoNoNull(usuario,"No se puede asignar una tarea a un usuario null.");
        VerificarUsuarioNoEstaAsignado(usuario);
        UsuariosAsignados.Add(usuario);
    }
    
    private Usuario BuscarUsuarioPorId(int id)
    {
        return UsuariosAsignados.FirstOrDefault(u => u.Id == id);
    }
    
    private void ListaEsNullLanzaExcepcion()
    {
        if (this.UsuariosAsignados == null)
        {
            throw new ExcepcionDominio("La lista de usuarios asignados está vacía o no está inicializada.");
        }
    }
    
    public void EliminarUsuario(int idUsu)
    {
        ListaEsNullLanzaExcepcion();
        Usuario usuarioAEliminar = BuscarUsuarioPorId(idUsu);
        ValidarObjetoNoNull(usuarioAEliminar,"El usuario no está asignado a la tarea.");
        UsuariosAsignados.Remove(usuarioAEliminar);
    }

    private void VerificarRecursoNoEstaAgregado(Recurso recurso)
    {
        if(RecursosNecesarios.Contains(recurso))
            throw new ExcepcionDominio("El recurso ya fue agregado.");
    }
    
    public void AgregarRecurso(Recurso recurso)
    {
        ValidarObjetoNoNull(recurso,"No se puede agregar un recurso null.");
        VerificarRecursoNoEstaAgregado(recurso);
        RecursosNecesarios.Add(recurso);
    }
    
    private Recurso BuscarRecursoPorId(int id)
    {
        return RecursosNecesarios.FirstOrDefault(r => r.Id == id);
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
    
    public void ModificarDescripcion(string nuevaDesc)
    {
        ValidarStringNoVacioNiNull(nuevaDesc,"La descripción no puede estar vacía");
        Descripcion = nuevaDesc;
    }

    private void VerificarFechaNoMenorAHoy(DateTime fechaNueva)
    {
        if (fechaNueva < DateTime.Now.Date)
            throw new ExcepcionDominio("La fecha no puede ser anterior a hoy.");
    }

    public void ModificarFechaInicioMasTemprana(DateTime fechaInicioNueva)
    {
        VerificarFechaNoMenorAHoy(fechaInicioNueva);
        FechaInicioMasTemprana = fechaInicioNueva;
        CalcularFechaFinMasTemprana();
    }
    
    public void ModificarFechaDeEjecucion(DateTime fechaNueva)
    {
        VerificarFechaNoMenorAHoy(fechaNueva);
        FechaDeEjecucion = fechaNueva;
    }

    private void DuracionNoMenorACero(int duracion)
    {
        if (duracion <= 0)
            throw new ExcepcionDominio("La fecha de duración no puede ser cero o negativa.");
    }
    
    public void ModificarDuracion(int nuevaDuracion)
    {
        DuracionNoMenorACero(nuevaDuracion);
        DuracionEnDias = nuevaDuracion;
        CalcularFechaFinMasTemprana();
    }

    public void NotificarMiembros(string mensaje)
    {
        foreach (Usuario usuario in UsuariosAsignados)
        {
            usuario.RecibirNotificacion(mensaje);
        }
    }
    
    public void VerificarDependenciaNoEstaAgregada(Dependencia dependencia)
    {
        if (Dependencias.Contains(dependencia))
            throw new ExcepcionDominio("La dependencia ya fue agregada.");
    }
    
    public void AgregarDependencia(Dependencia dependencia)
    {
        ValidarObjetoNoNull(dependencia,"No se puede agregar una dependencia null.");
        VerificarDependenciaNoEstaAgregada(dependencia);
        Dependencias.Add(dependencia);
    }
    
    private Dependencia BuscarDependenciaPorIdDeTarea (int id)
    {
        return Dependencias.FirstOrDefault(d =>d.Tarea.Id  == id);
    }
    public void EliminarDependencia(int idTarea)
    {
        Dependencia dependenciaAEliminar = BuscarDependenciaPorIdDeTarea(idTarea);
        ValidarObjetoNoNull(dependenciaAEliminar,"La dependencia no se encuentra dentro de la lista de dependencias.");
        Dependencias.Remove(dependenciaAEliminar);
    }
}
