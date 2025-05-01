using Dominio.Dummies;
using Repositorios.IRepositorios;

namespace Repositorios;

public class RepositorioProyectos : IRepositorioProyectos
{
    private List<Proyecto> _proyectos;

    public RepositorioProyectos()
    {
        _proyectos = new List<Proyecto>();
    }

    public void Agregar(Proyecto objeto)
    {
        _proyectos.Add(objeto);
    }

    public Proyecto ObtenerPorId(int id)
    {
        return _proyectos.FirstOrDefault(p=>p.Id == id);
    }

    public void Eliminar(int id)
    {
        _proyectos.Remove(_proyectos.FirstOrDefault(p => p.Id == id));
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _proyectos;
    }

    public void ModificarNombre(int idProyecto, string nombre)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.ModificarNombre(nombre);
    }

    public void ModificarDescripcion(int idProyecto, string descripcion)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.ModificarDescripcion(descripcion);
    }

    public void ModificarFechaInicio(int idProyecto, DateTime fechaInicio)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.ModificarFechaInicio(fechaInicio);
    }

    public void ModificarFechaFinMasTemprana(int idProyecto, DateTime fechaFinMasTemprana)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.ModificarFechaFinMasTemprana(fechaFinMasTemprana);
    }

    public void ModificarAdministrador(int idProyecto, Usuario administrador)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.Administrador = administrador;
    }

    public void AgregarTarea(int idProyecto, Tarea tarea)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.AgregarTarea(tarea);
    }

    public void EliminarTarea(int idProyecto, int idTarea)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.EliminarTarea(idTarea);
    }

    public void AgregarMiembro(int idProyecto, Usuario miembro)
    {
        throw new NotImplementedException();
    }

    public void EliminarMiembro(int idProyecto, int idMiembro)
    {
        throw new NotImplementedException();
    }
}