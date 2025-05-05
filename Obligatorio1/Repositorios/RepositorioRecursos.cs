using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioRecursos : IRepositorioRecursos
{
    public List<Recurso> _recursos;

    public RepositorioRecursos()
    {
        _recursos = new List<Recurso>();
    }

    public void Agregar(Recurso objeto)
    {
        _recursos.Add(objeto);
    }

    public Recurso ObtenerPorId(int id)
    {
        return _recursos.Find(recurso => recurso.Id == id);
    }

    public void Eliminar(int id)
    {
        _recursos.Remove(_recursos.Find(recurso => recurso.Id == id));
    }

    public List<Recurso> ObtenerTodos()
    {
        return _recursos;
    }

    public void ModificarNombre(int idRecurso, string nombre)
    {
        Recurso recurso = ObtenerPorId(idRecurso);
        recurso.ModificarNombre(nombre);
    }

    public void ModificarTipo(int idRecurso, string tipo)
    {
        throw new NotImplementedException();
    }

    public void ModificarDescripcion(int idRecurso, string descripcion)
    {
        throw new NotImplementedException();
    }

    public void ModificarProyectoAsociado(int idRecurso, Proyecto proyecto)
    {
        throw new NotImplementedException();
    }

    public void ModificarCantidadDeTareasUsandolo(int idRecurso, int cantidadDeTareasUsandolo)
    {
        throw new NotImplementedException();
    }
}