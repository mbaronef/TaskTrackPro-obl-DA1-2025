using Dominio.Excepciones;

namespace Dominio;

public class Dependencia
{
    public string Tipo { get; private set; }
    public Tarea Tarea { get; private set; }

    public Dependencia(string unTipo, Tarea unaTarea)
    {
        ValidarNoVacio(unTipo,"No se puede ingresar un tipo vacío.");
        ValidarTipoValido(unTipo, "El tipo de dependencia debe ser 'FF' o 'FS'.");
        ValidarTareaNoNula(unaTarea, "Una tarea no puede ser nula.");
        this.Tipo = unTipo;
        this.Tarea = unaTarea;
    }
    private void ValidarNoVacio(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarTipoValido(string valor, string mensajeError)
    {
        if (valor != "SS" && valor != "FS")
        {
            throw new ExcepcionDominio(mensajeError);
        }
    }
    
    private void ValidarTareaNoNula(Tarea tarea, string mensajeError)
    {
        if (tarea == null)
            throw new ExcepcionDominio(mensajeError);
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Dependencia otra)
            return Tipo == otra.Tipo && Tarea.Equals(otra.Tarea);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tipo, Tarea.Id);
    }
}