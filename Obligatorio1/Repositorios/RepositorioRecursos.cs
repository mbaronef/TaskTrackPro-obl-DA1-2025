using Dominio;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioRecursos : IRepositorioRecursos
{
    public List<Recurso> _recursos;
    private static int _cantidadRecursos;

    public RepositorioRecursos()
    {
        _recursos = new List<Recurso>();
    }

    public void Agregar(Recurso objeto)
    {
        objeto.Id = ++_cantidadRecursos;
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
        Recurso recurso = ObtenerPorId(idRecurso);
        recurso.ModificarTipo(tipo);
    }

    public void ModificarDescripcion(int idRecurso, string descripcion)
    {
        Recurso recurso = ObtenerPorId(idRecurso);
        recurso.ModificarDescripcion(descripcion);
    }

    public void ModificarProyectoAsociado(int idRecurso, Proyecto proyecto)
    {
        Recurso recurso = ObtenerPorId(idRecurso);
        recurso.AsociarAProyecto(proyecto);
    }

    public void ModificarCantidadDeTareasUsandolo(int idRecurso, int nuevaCantidadDeTareasUsandolo)
    {
        Recurso recurso = ObtenerPorId(idRecurso);
        int diferencia = nuevaCantidadDeTareasUsandolo - recurso.CantidadDeTareasUsandolo;

        if (diferencia > 0)
        {
            IncrementarTareas(recurso, diferencia);
        }
        else if (diferencia < 0)
        {
            DecrementarTareas(recurso, -diferencia);
        }
    }

    private void IncrementarTareas(Recurso recurso, int veces)
    {
        for (int i = 0; i < veces; i++)
            recurso.IncrementarCantidadDeTareasUsandolo();
    }

    private void DecrementarTareas(Recurso recurso, int veces)
    {
        for (int i = 0; i < veces; i++)
            recurso.DecrementarCantidadDeTareasUsandolo();
    }
}