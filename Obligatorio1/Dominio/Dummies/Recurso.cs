namespace Dominio.Dummies;

public class Recurso
{
    public int Id { get; set; }
    public int CantidadDeTareasUsando { get; set; }
    
    public void IncrementarCantidadDeTareasUsando()
    {
        CantidadDeTareasUsando++;
    }
    
    public void DecrementarCantidadDeTareasUsando()
    {
        CantidadDeTareasUsando--;
    }
    
}