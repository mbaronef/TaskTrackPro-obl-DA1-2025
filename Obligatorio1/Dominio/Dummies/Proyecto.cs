using Dominio.Excepciones;

namespace Dominio.Dummies;

public class Proyecto
{
    private const int MaximoCaracteresDescripcion = 400;
    public int Id { get; private set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public List<Tarea> Tareas { get; set; }
    public Usuario Administrador { get; set; }
    public List<Usuario> Miembros { get; set; }

    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public DateTime FechaFinMasTemprana { get; set; } = DateTime.MinValue;

    private void ValidarStringNoVacioNiNull(string valor, string mensajeError)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(mensajeError);
    }

    private void ValidarObjetoNoNull(object objeto, string mensajeError)
    {
        if (objeto is null)
            throw new ExcepcionDominio(mensajeError);
    }

    public Proyecto(string nombre, string descripcion, Usuario administrador, List<Usuario> miembros)
    {
        ValidarStringNoVacioNiNull(nombre, "El nombre del proyecto no puede estar vacío o null.");
        ValidarStringNoVacioNiNull(descripcion, "La descripción del proyecto no puede estar vacía o null.");
        ValidarObjetoNoNull(administrador, "El proyecto debe tener un administrador.");
        ValidarObjetoNoNull(miembros,"La lista de miembros no puede ser null.");
        if (!miembros.Contains(administrador))
        {
            miembros.Add(administrador); // ASEGURA QUE EL ADMIN SIEMPRE ESTE EN MIEMBROS DEL PROYECTO
        }
        if (descripcion.Length > MaximoCaracteresDescripcion)
            throw new ExcepcionDominio("La descripción del proyecto no puede superar los 400 caracteres.");
        Nombre = nombre;
        Descripcion = descripcion;
        Tareas = new List<Tarea>();
        Administrador = administrador;
        Miembros = miembros;
    }
    
}