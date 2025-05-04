using Dominio;

namespace Repositorios.Interfaces;
public interface IRepositorioRecursos : IRepositorio<Recurso>
{
    void ModificarNombre(int idRecurso, string nombre);
    void ModificarTipo(int idRecurso, string tipo);
    void ModificarDescripcion(int idRecurso, string descripcion);
    void ModificarProyectoAsociado(int idRecurso, Proyecto proyecto);
    void ModificarCantidadDeTareasUsandolo(int idRecurso, int cantidadDeTareasUsandolo);
}