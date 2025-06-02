namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionPermisos : ExcepcionDominio
{
    public ExcepcionPermisos(string mensaje) : base(mensaje) { }
}