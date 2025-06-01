using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class RecursoDTO
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }
        
    [Required(ErrorMessage = "El tipo es obligatorio.")]
    public string Tipo { get; set; }
        
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; }

    public ProyectoDTO ProyectoAsociado { get; set; }
    
    public Recurso AEntidad()
    {
        return new Recurso(Nombre, Tipo, Descripcion);
    }
    
    public static RecursoDTO DesdeEntidad(Recurso recurso)
    {
        return new RecursoDTO()
        {
            Id = recurso.Id,
            Nombre = recurso.Nombre,
            Tipo = recurso.Tipo,
            Descripcion = recurso.Descripcion,
            ProyectoAsociado = recurso.ProyectoAsociado != null ? ProyectoDTO.DesdeEntidad(recurso.ProyectoAsociado) : null
        };
    }
}