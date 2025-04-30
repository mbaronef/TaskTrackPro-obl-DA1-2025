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
    public Dependencia(string unTipo, Tarea unaTarea)
    {
        this.Tipo = unTipo;
        this.Tarea = unaTarea;
    }
}