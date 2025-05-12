using System.ComponentModel.DataAnnotations;


namespace Interfaz.DTOs;

public class UsuarioDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre no puede ser vacío.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido no puede ser vacío.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El email no puede ser vacío")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña no puede ser vacía.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una minúscula, una mayúscula, un número y un carácter especial.")]
    public string Contrasena { get; set; }

    public bool EsAdministradorSistema { get; set; }
    public bool EsAdministradorProyecto { get; set; }
}