using Dominio.Dummies;
using Repositorios.IRepositorios;

namespace Repositorios;

public class RepositorioTareas : IRepositorioTareas
{
    private List<Tarea> _tareas;

    public RepositorioTareas()
    {
        _tareas = new List<Tarea>();
    }

    public void Agregar(Tarea objeto)
    {
        _tareas.Add(objeto);
    }

    public Tarea ObtenerPorId(int id)
    {
        return _tareas.Find(tarea => tarea.Id == id);
    }

    public void Eliminar(int id)
    {
        _tareas.Remove(_tareas.FirstOrDefault(tarea => tarea.Id == id));
    }

    public List<Tarea> ObtenerTodos()
    {
        return _tareas;
    }

    public void ModificarTitulo(int idTarea, string titulo)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarTitulo(titulo);
    }

    public void ModificarDescripcion(int idTarea, string descripcion)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarDescripcion(descripcion);
    }

    public void ModificarDuracion(int idTarea, int duracion)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarDuracion(duracion);
    }

    public void ModificarFechaInicioMasTemprana(int idTarea, DateTime fechaInicioMasTemprana)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarFechaInicioMasTemprana(fechaInicioMasTemprana);
    }

    public void ModificarFechaFinMasTemprana(int idTarea, DateTime fechaFinMasTemprana)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarFechaFinMasTemprana(fechaFinMasTemprana);
    }

    public void ModificarFechaDeEjecucion(int idTarea, DateTime fechaDeEjecucion)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.ModificarFechaDeEjecucion(fechaDeEjecucion);
    }

    public void ModificarEstado(int idTarea, EstadoTarea estado)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.CambiarEstado(estado);
    }

    public void ModificarHolgura(int idTarea, float holgura)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.Holgura = holgura;
    }

    public void AgregarUsuario(int idTarea, Usuario usuarioAsignado)
    {
        Tarea tarea = ObtenerPorId(idTarea);
        tarea.AsignarUsuario(usuarioAsignado);
    }

    public void EliminarUsuario(int idTarea, int idUsuarioAsignado)
    {
        throw new NotImplementedException();
    }

    public void AgregarRecursoNecesario(int idTarea, Recurso recursoNecesario)
    {
        throw new NotImplementedException();
    }

    public void EliminarRecursoNecesario(int idTarea, Recurso recursoNecesario)
    {
        throw new NotImplementedException();
    }

    public void AgregarDependencia(int idTarea, Dependencia dependencia)
    {
        throw new NotImplementedException();
    }

    public void EliminarDependencia(int idTarea, Dependencia dependencia)
    {
        throw new NotImplementedException();
    }
}