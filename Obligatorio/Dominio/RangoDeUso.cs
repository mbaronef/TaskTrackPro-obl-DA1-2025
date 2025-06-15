using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class RangoDeUso
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadDeUsos { get; set; }
    public Tarea Tarea { get; set; }
    
    public RangoDeUso()
    {
    }
    
    public RangoDeUso(DateTime fechaInicio, DateTime fechaFin, int cantidadDeUsos, Tarea tarea)
    {
        ValidarFechaInicioMenorAFin(fechaInicio, fechaFin);
        ValidarCantidadDeUsosMayorACero(cantidadDeUsos);
        if (tarea == null)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.TareaNoPuedeSerNula);
        }

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        CantidadDeUsos = cantidadDeUsos;
        Tarea = tarea;
    }
    
    private void ValidarFechaInicioMenorAFin(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaFin < fechaInicio)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.FechaInicioRangoMayorQueFin);
        }
    }
    
    private void ValidarCantidadDeUsosMayorACero(int cantidadDeUsos)
    {
        if(cantidadDeUsos <= 0)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.RangoDeUsoNoPuedeSerCeroOMenos);
        }
    }
}