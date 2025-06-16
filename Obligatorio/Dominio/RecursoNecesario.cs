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
        Recurso = recurso;
        Cantidad = cantidad;
    }

    private void ValidarRecursoNoNulo(Recurso recurso)
    {
        if (recurso == null)
            throw new ExcepcionRecurso(MensajesErrorDominio.RecursoNullParaAgregar);
    }
}