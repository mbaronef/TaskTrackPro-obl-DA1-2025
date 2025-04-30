namespace Dominio;
using Dominio.Excepciones;
public class Dependencia
{
    public string Tipo { get; private set; }
    public Tarea Tarea { get; private set; }

    private void ValidarNoVacio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarTipoValido(string valor, string mensajeError)
    {
        if (valor != "FF" && valor != "FS")
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }
    public Dependencia(string unTipo, Tarea unaTarea)
    {
        ValidarNoVacio(unTipo,"No se puede ingresar un tipo vacío");
        ValidarTipoValido(unTipo, "El tipo de dependencia debe ser 'FF' o 'FS'");
        this.Tipo = unTipo;
        this.Tarea = unaTarea;
    }
}