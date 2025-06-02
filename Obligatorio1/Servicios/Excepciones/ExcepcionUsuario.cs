namespace Servicios.Excepciones;

using Dominio.Excepciones;

public class ExcepcionUsuario: ExcepcionDominio
{
    public ExcepcionUsuario(string mensaje) : base(mensaje) { }
}