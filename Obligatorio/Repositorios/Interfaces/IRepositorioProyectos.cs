using Dominio;

namespace Repositorios.Interfaces;

public interface IRepositorioProyectos : IRepositorio<Proyecto>
{
    void UpdateTarea(Tarea tarea);
}