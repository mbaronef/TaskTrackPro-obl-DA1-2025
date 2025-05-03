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
        throw new NotImplementedException();
    }

    public Tarea ObtenerPorId(int id)
    {
        return _tareas.Find(tarea => tarea.Id == id);
    }

    public void Eliminar(int id)
    {
        throw new NotImplementedException();
    }

    public List<Tarea> ObtenerTodos()
    {
        throw new NotImplementedException();
    }

    public void ModificarTitulo(int idTarea, string titulo)
    {
        throw new NotImplementedException();
    }

    public void ModificarDescripcion(int idTarea, string descripcion)
    {
        throw new NotImplementedException();
    }

    public void ModificarDuracion(int idTarea, int duracion)
    {
        throw new NotImplementedException();
    }

    public void ModificarFechaInicioMasTemprana(int idTarea, DateTime fechaInicioMasTemprana)
    {
        throw new NotImplementedException();
    }

    public void ModificarFechaFinMasTemprana(int idTarea, DateTime fechaFinMasTemprana)
    {
        throw new NotImplementedException();
    }

    public void ModificarFechaDeEjecucion(int idTarea, DateTime fechaDeEjecucion)
    {
        throw new NotImplementedException();
    }

    public void ModificarEstado(int idTarea, EstadoTarea estado)
    {
        throw new NotImplementedException();
    }

    public void ModificarHolgura(int idTarea, int holgura)
    {
        throw new NotImplementedException();
    }

    public void AgregarUsuario(int idTarea, Usuario usuarioAsignado)
    {
        throw new NotImplementedException();
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