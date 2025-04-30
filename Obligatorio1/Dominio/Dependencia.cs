namespace Dominio;
using Dominio.Excepciones;
public class Dependencia
{
    public string Tipo { get; set; }
    public Tarea Tarea { get; set; }

    private void ValidarStringNoVacio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarTipoFFoFS(string valor, string mensajeError)
    {
        if (valor != "FF" || valor != "FS")
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }
    public Dependencia(string unTipo, Tarea unaTarea)
    {
        ValidarStringNoVacio(unTipo,"No se puede ingresar un tipo vacío");
        ValidarTipoFFoFS(unTipo, "El tipo de dependencia debe ser 'FF' o 'FS'");
        this.Tipo = unTipo;
        this.Tarea = unaTarea;
    }
}