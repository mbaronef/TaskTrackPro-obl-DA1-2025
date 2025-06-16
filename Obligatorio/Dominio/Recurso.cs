using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Recurso
{
    public int Id { get; set; }
    public string Nombre { get; private set; }
    public string Tipo { get; private set; }
    public string Descripcion { get; private set; }
    public Proyecto? ProyectoAsociado { get; private set; } = null;
    public int Capacidad { get; private set; } 
    public int CantidadDeTareasUsandolo { get; private set; } = 0;
    public List<RangoDeUso> RangosEnUso { get; private set; }

    public Recurso()
    {
        RangosEnUso = new List<RangoDeUso>();
    }
    public Recurso(string nombre, string tipo, string descripcion, int capacidad)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(tipo, "tipo");
        ValidarAtributoNoVacio(descripcion, "descripcion");
        ValidarCapacidadMayorACero(capacidad);
        
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
        RangosEnUso = new List<RangoDeUso>();
        Capacidad = capacidad;
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
        if (EsExclusivo())
        {
            throw new ExcepcionDominio(MensajesErrorDominio.RecursoYaEsExclusivo);
        }
        ProyectoAsociado = proyecto;
    }

    public void IncrementarCantidadDeTareasUsandolo()
    {
        CantidadDeTareasUsandolo++;
    }

    public void DecrementarCantidadDeTareasUsandolo()
    {
        if (CantidadDeTareasUsandolo <= 0)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.CantidadTareasRecursoNegativa);
        }
        CantidadDeTareasUsandolo--;
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
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.AtributoVacio, nombreAtributo));
        }
    }

    private void ValidarCapacidadMayorACero(int capacidad)
    {
        if (capacidad <= 0)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadRecursoInvalida);
        }
    }

    public override bool Equals(object? otro)
    {
        Recurso otroRecurso = otro as Recurso;
        return otroRecurso != null && Id == otroRecurso.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"Nombre: '{Nombre}', tipo: '{Tipo}', descripción: '{Descripcion}'";
    }
}