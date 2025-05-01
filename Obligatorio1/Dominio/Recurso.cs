using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class Recurso
{
    public string Nombre { get; private set; }
    public string Tipo { get; private set; }
    public string Descripcion { get; private set; }
    public Proyecto? ProyectoAsociado { get; private set; } = null;
    public int CantidadDeTareasUsandolo { get; private set; } = 0;

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

    public void ModificarNombre(string nombre)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        Nombre = nombre;
    }

    public void ModificarTipo(string tipo)
    {
        ValidarAtributoNoVacio(tipo, "tipo");
        Tipo = tipo;
    }

    public void ModificarDescripcion(string descripcion)
    {
        ValidarAtributoNoVacio(descripcion, "descripcion");
        Descripcion = descripcion;
    }
}