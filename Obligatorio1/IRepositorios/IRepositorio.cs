using Dominio.Dummies;

namespace IRepositorios;

public interface IRepositorio<T>
{
    void Agregar(T objeto); 
    void Eliminar(int id);
    T ObtenerPorId(int id);
}