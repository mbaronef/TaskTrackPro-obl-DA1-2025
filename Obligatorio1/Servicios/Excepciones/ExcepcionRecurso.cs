namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionRecurso : ExcepcionDominio
{
    public ExcepcionRecurso(string mensaje) : base(mensaje) { }
}