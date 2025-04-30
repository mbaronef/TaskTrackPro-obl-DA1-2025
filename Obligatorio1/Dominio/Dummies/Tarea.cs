namespace Dominio.Dummies;

public class Tarea
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    
    public List<Recurso> RecursosNecesarios { get; set; } = new List<Recurso>();
}