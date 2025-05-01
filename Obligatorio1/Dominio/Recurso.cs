using Dominio.Dummies;

namespace Dominio;

public class Recurso
{
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public Proyecto? ProyectoAsociado { get; set; } = null;
    public int CantidadDeTareasUsandolo { get; set; } = 0;

    public Recurso(string nombre, string tipo, string descripcion)
    {
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
    }


}