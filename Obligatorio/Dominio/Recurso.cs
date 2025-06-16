using System.Reflection.Metadata.Ecma335;
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
    public virtual ICollection<RangoDeUso> RangosEnUso { get; private set; }

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

    public bool TieneCapacidadDisponible(DateTime fechaDesde, DateTime fechaHasta, int capacidadRequerida)
    {
        for (DateTime dia = fechaDesde; dia <= fechaHasta; dia = dia.AddDays(1))
        {
            int usosEnElDia = CantidadDeUsosPorDia(dia); 
            
            int totalUsosEnElDia = usosEnElDia + capacidadRequerida;

            if (totalUsosEnElDia > Capacidad)
            {
                return false;
            }
        }
        return true;
    }
    
    /* public void AgregarRangoDeUso(DateTime fechaInicioNuevo, DateTime fechaFinNuevo, int cantidadNuevo, Tarea tarea)
    {
        if (!TieneCapacidadDisponible(fechaInicioNuevo, fechaFinNuevo, cantidadNuevo))
        {
            throw new Exception("No hay capacidad suficiente en el recurso para ese rango de fechas.");
        }

        var nuevoRango = new RangoDeUso(fechaInicioNuevo, fechaFinNuevo, cantidadNuevo, tarea);
        RangosEnUso.Add(nuevoRango);
    }*/

    public void ModificarCapacidad(int nuevaCapacidad)
    {
        ValidarCapacidadMayorACero(nuevaCapacidad);
        Capacidad = nuevaCapacidad;
    }

    public bool EsExclusivo()
    {
        return ProyectoAsociado != null;
    }

    public bool SeEstaUsando()
    {
        return CantidadDeTareasUsandolo > 0;
    }

    private int CantidadDeUsosPorDia(DateTime dia)
    {
        return RangosEnUso
            .Where(r => r.FechaInicio <= dia && r.FechaFin >= dia)
            .Sum(r => r.CantidadDeUsos);
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