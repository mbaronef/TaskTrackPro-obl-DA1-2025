namespace Servicios.Excepciones
{
    public class ExcepcionCaminoCritico : Exception
    {
        public ExcepcionCaminoCritico(string mensaje) : base(mensaje)
        {
        }
    }
}