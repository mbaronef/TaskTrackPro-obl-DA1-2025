namespace IGestores;

public interface IGestor<T>
{
    void Agregar(T objeto); 
    void Eliminar(int id);
}