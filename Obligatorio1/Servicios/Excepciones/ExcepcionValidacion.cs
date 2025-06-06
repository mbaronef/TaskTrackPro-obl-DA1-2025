namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionValidacion : ExcepcionDominio
{
    public ExcepcionValidacion(string mensaje) : base(mensaje) { }
}