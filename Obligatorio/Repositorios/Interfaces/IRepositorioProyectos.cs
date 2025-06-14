using Dominio;

namespace Repositorios.Interfaces;

public interface IRepositorioProyectos : IRepositorio<Proyecto>
{
    void ActualizarTarea(Tarea tarea);
    void AgregarTarea(Proyecto proyecto, Tarea nuevaTarea);
}