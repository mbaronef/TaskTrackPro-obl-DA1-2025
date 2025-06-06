namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionProyecto : ExcepcionDominio
{
    public ExcepcionProyecto(string mensaje) : base(mensaje) { }
}