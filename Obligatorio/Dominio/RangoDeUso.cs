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
        if (fechaFin < fechaInicio)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.FechaInicioRangoMayorQueFin);
        }

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        CantidadDeUsos = cantidadDeUsos;
        Tarea = tarea;
    }
}