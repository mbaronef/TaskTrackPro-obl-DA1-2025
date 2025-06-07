namespace Servicios.Excepciones
{
    public class ExcepcionContrasena : Exception
    {
        public ExcepcionContrasena(string mensaje) : base(mensaje)
        {
        }
    }
}