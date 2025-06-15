using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Recurso
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public Proyecto? ProyectoAsociado { get; set; } = null;
    public int CantidadDeTareasUsandolo { get; set; } = 0;

    public Recurso() { }
    public Recurso(string nombre, string tipo, string descripcion)
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

    public void Actualizar(Recurso recursoActualizado)
    {

        ModificarNombre(recursoActualizado.Nombre);
    }


    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.AtributoVacio, nombreAtributo));
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