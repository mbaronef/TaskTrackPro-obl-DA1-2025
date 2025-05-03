using Dominio.Dummies;
using Dominio.Excepciones;

namespace Dominio;

public class Recurso
{
    public int Id { get; set; }
    public string Nombre { get; private set; }
    public string Tipo { get; private set; }
    public string Descripcion { get; private set; }
    public Proyecto? ProyectoAsociado { get; private set; } = null;
    public int CantidadDeTareasUsandolo { get; private set; } = 0;

    public Recurso(string nombre, string tipo, string descripcion) // ctor. recursos no exclusivos
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(tipo, "tipo");
        ValidarAtributoNoVacio(descripcion, "descripcion");
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
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
    
    public void AsociarAProyecto(Proyecto proyecto)
    {
        if (ProyectoAsociado != null)
        {
            throw new ExcepcionDominio("El recurso ya es exclusivo de otro proyecto");
        }
        ProyectoAsociado = proyecto;
    }
    
    public void ModificarCantidadDeTareasUsandolo(int cantidadDeTareasUsandolo)
    {
        if (cantidadDeTareasUsandolo < 0)
        {
            throw new ExcepcionDominio("No puede haber una cantidad negativa de tareas utilizando el recurso");
        }
        CantidadDeTareasUsandolo = cantidadDeTareasUsandolo;
    }
    
    public bool EsExclusivo()
    {
        return ProyectoAsociado != null;
    }

    public bool SeEstaUsando()
    {
        return CantidadDeTareasUsandolo > 0;
    }
    
    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ExcepcionDominio($"El atributo {nombreAtributo} no puede ser vacío");
        }
    }
}