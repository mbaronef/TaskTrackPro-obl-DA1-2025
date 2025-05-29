namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionTarea : ExcepcionDominio
{
    public ExcepcionTarea(string mensaje) : base(mensaje) { }
}