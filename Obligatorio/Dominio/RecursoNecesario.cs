namespace Dominio;

public class RecursoNecesario
{
    public int Id {get; set;}
    public Recurso Recurso { get; set; }
    public int Cantidad { get; set; }

    public RecursoNecesario(Recurso recurso, int cantidad)
    {
        Recurso = recurso;
        Cantidad = cantidad;
    }
}