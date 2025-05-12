using System.ComponentModel.DataAnnotations;
using Dominio;

namespace Interfaz.DTOs;

public class TareaDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La tarea debe tener un título.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una descripción.")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una duración en días.")]
    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que 0.")]
    public int DuracionEnDias { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una fecha de inicio.")]
    [CustomValidation(typeof(TareaDTO), nameof(ValidarFechaInicio))]
    public DateTime FechaInicioMasTemprana { get; set; }

    public EstadoTarea Estado { get; set; }

    public Tarea ANuevaEntidad()
    {
        return new Tarea(Titulo, Descripcion, DuracionEnDias, FechaInicioMasTemprana);
    }

    public static ValidationResult ValidarFechaInicio(DateTime fecha, ValidationContext context)
    {
        if (fecha < DateTime.Today)
        {
            return new ValidationResult("La fecha de inicio debe ser hoy o en el futuro.");
        }

        return ValidationResult.Success;
    }
}