namespace Dominio;

public class Dependencia
{
    public string Tipo { get; set; }
    public Tarea Tarea { get; set; }

    public Dependencia(string unTipo, Tarea unaTarea)
    {
        this.Tipo = unTipo;
        this.Tarea = unaTarea;
    }
}