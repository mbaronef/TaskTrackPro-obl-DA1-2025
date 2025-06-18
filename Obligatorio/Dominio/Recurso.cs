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
    public int Capacidad { get;  set; } 
    public int CantidadDeTareasUsandolo { get; set; } = 0;
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
        ValidarCapacidadMayorACero(capacidadRequerida);
        ValidarCapacidadMenorACapacidadRecurso(capacidadRequerida);
        ValidarInicioAntesQueFin(fechaDesde, fechaHasta);
        
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

    public void AgregarRangoDeUso(Tarea tarea, int cantidadNuevo)
    {
        ValidarTareaNoUsaElRecurso(tarea);
        DateTime fechaInicioNuevo = tarea.FechaInicioMasTemprana;
        DateTime fechaFinNuevo = tarea.FechaFinMasTemprana;
        ValidarCapacidadDisponibleEnRango(fechaInicioNuevo, fechaFinNuevo, cantidadNuevo);
        RangoDeUso nuevoRango = new RangoDeUso(fechaInicioNuevo, fechaFinNuevo, cantidadNuevo, tarea);
        RangosEnUso.Add(nuevoRango);
    }
    
    public void EliminarRangoDeUsoDeTarea(Tarea tarea)
    {
        RangoDeUso rangoAEliminar = RangosEnUso.FirstOrDefault(r => r.Tarea.Equals(tarea));
        RangosEnUso.Remove(rangoAEliminar);
    }

    public void ModificarCapacidad(int nuevaCapacidad)
    {
        ValidarCapacidadMayorACero(nuevaCapacidad);
        
        if (nuevaCapacidad < Capacidad)
        {
            ValidarSiElUsoSuperaLaNuevaCapacidad(nuevaCapacidad);
        }
        
        Capacidad = nuevaCapacidad;
    }

    private void ValidarSiElUsoSuperaLaNuevaCapacidad(int nuevaCapacidad)
    {
        DateTime primeraFechaDeUso = RangosEnUso.Min(r => r.FechaInicio.Date);
        DateTime ultimaFechaDeUso = RangosEnUso.Max(r => r.FechaFin.Date);

        for (DateTime dia = primeraFechaDeUso; dia <= ultimaFechaDeUso; dia = dia.AddDays(1))
        {
            int usoEnDia = CantidadDeUsosPorDia(dia);
            
            if (usoEnDia > nuevaCapacidad)
            {
                throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadNoReducible);
            }
        }
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
        ValidarIdentidad(recursoActualizado);
        
        ModificarNombre(recursoActualizado.Nombre);
        ModificarTipo(recursoActualizado.Tipo);
        ModificarDescripcion(recursoActualizado.Descripcion);
        CantidadDeTareasUsandolo = recursoActualizado.CantidadDeTareasUsandolo;
        ModificarCapacidad(recursoActualizado.Capacidad);
    }
    
    private int CantidadDeUsosPorDia(DateTime dia)
    {
        return RangosEnUso
            .Where(r => r.FechaInicio <= dia && r.FechaFin >= dia)
            .Sum(r => r.CantidadDeUsos);
    }
    
    private void ValidarCapacidadDisponibleEnRango(DateTime fechaInicioNuevo, DateTime fechaFinNuevo, int cantidadNuevo)
    {
        if (!TieneCapacidadDisponible(fechaInicioNuevo, fechaFinNuevo, cantidadNuevo))
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadInsuficienteEnElRango);
        }
    }
    
    private void ValidarTareaNoUsaElRecurso(Tarea tarea)
    {
        if (RangosEnUso.Any(r => r.Tarea.Equals(tarea)))
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.RecursoYaAgregadoATarea);
        }
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
    
    private void ValidarCapacidadMenorACapacidadRecurso(int capacidadRequerida)
    {
        if (capacidadRequerida > Capacidad)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadRecursoInvalida);
        }
    }
    
    private void ValidarInicioAntesQueFin(DateTime fechaDesde, DateTime fechaHasta)
    {
        if (fechaDesde > fechaHasta)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.FechaInicioRangoMayorQueFin);
        }
    }
    
    private void ValidarIdentidad(Recurso otroRecurso)
    {
        if (!Equals(otroRecurso))
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.ActualizarEntidadNoCoincidente);
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