using Dominio.Dummies;

namespace IGestores;

public interface IGestor<T>
{
    void Agregar(Usuario solicitante, T objeto); 
    void Eliminar(int id);
}