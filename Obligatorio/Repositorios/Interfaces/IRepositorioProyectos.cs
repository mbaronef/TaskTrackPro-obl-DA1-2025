using Dominio;

namespace Repositorios.Interfaces;

public interface IRepositorioProyectos : IRepositorio<Proyecto>
{
    void AgregarTarea(Proyecto proyecto, Tarea nuevaTarea);
    void ActualizarTarea(Tarea tarea);
}