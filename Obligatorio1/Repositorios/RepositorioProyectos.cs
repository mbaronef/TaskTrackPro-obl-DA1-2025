using Dominio;
using Repositorios.Interfaces;

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
        return _proyectos.Find(proyecto=>proyecto.Id == id);
    }

    public void Eliminar(int id)
    {
        _proyectos.Remove(_proyectos.Find(proyecto => proyecto.Id == id));
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
        proyecto.AsignarNuevoAdministrador(administrador);
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
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.AsignarMiembro(miembro);
    }

    public void EliminarMiembro(int idProyecto, int idMiembro)
    {
        Proyecto proyecto = ObtenerPorId(idProyecto);
        proyecto.EliminarMiembro(idMiembro);
    }
}