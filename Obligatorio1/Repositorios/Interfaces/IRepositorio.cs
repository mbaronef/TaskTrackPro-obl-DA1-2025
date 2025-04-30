using Dominio.Dummies;

namespace Repositorios.IRepositorios;

public interface IRepositorio<T>
{
    void Agregar(T objeto);
    Usuario ObtenerPorId(int id);
    void Eliminar(int id);
}