using Dominio.Dummies;
using Dominio.Excepciones;

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
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(tipo, "tipo");
        ValidarAtributoNoVacio(descripcion, "descripcion");
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
    }
    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrEmpty(texto))
        {
            throw new ExcepcionDominio($"El atributo {nombreAtributo} no puede ser vacío");
        }
    }
}