using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOS_;
public class ProyectoDTO

{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(400, ErrorMessage = "La descripción no puede tener más de 400 caracteres")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [CustomValidation(typeof(ProyectoDTO), nameof(ValidarFechaInicio))]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    public Proyecto ANuevaEntidad(Usuario administrador)
    {
        return new Proyecto(Nombre, Descripcion, FechaInicio, administrador, new List<Usuario>());
    }
    
    public static ProyectoDTO DesdeEntidad(Proyecto proyecto)
    {
        return new ProyectoDTO
        {
            Id = proyecto.Id,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            FechaInicio = proyecto.FechaInicio
        };
    }
        
    public static ValidationResult ValidarFechaInicio(DateTime fecha, ValidationContext context)
    {
        if (fecha < DateTime.Today)
        {
            return new ValidationResult("La fecha de inicio debe ser la actual o posterior.");
        }

        return ValidationResult.Success;
    }
}