using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class RecursoNecesario
{
    public int Id {get; set;}
    public Recurso Recurso { get; set; }
    public int Cantidad { get; set; }

    public RecursoNecesario()
    {
    }

    public RecursoNecesario(Recurso recurso, int cantidad)
    {
        ValidarRecursoNoNulo(recurso);
        ValidarCantidadMayorACero(cantidad);
        Recurso = recurso;
        Cantidad = cantidad;
    }

    private void ValidarCantidadMayorACero(int cantidad)
    {
        if (cantidad <= 0)
            throw new ExcepcionRecurso(MensajesErrorDominio.CantidadMayorACero);
    }

    private void ValidarRecursoNoNulo(Recurso recurso)
    {
        if (recurso == null)
            throw new ExcepcionRecurso(MensajesErrorDominio.RecursoNullParaAgregar);
    }
}