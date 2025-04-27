using System.Text.RegularExpressions;
using Dominio.Excepciones;

namespace Dominio;

public class Usuario
{
    private string _contrasena;

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public List<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
    public bool EsAdministradorProyecto { get; set; } = false;


    public Usuario(string unNombre, string unApellido, DateTime unaFechaNacimiento, string unEmail,
        string unaContrasena)
    {
        ValidarEdad(unaFechaNacimiento);
        ValidarEmail(unEmail);
        SetContrasenaEncriptada(unaContrasena);
        Nombre = unNombre;
        Apellido = unApellido;
        FechaNacimiento = unaFechaNacimiento;
        Email = unEmail;
    }

    private void SetContrasenaEncriptada(string contrasena)
    {
        ValidarContrasena(contrasena);
        _contrasena = Usuario.EncriptarContrasena(contrasena);
    }

    public static string EncriptarContrasena(string contrasena)
    {
        return BCrypt.Net.BCrypt.HashPassword(contrasena);
    }

    public bool Autenticar(string contrasenaIngresada)
    {
        return BCrypt.Net.BCrypt.Verify(contrasenaIngresada, _contrasena);
    }

    private void ValidarContrasena(string contrasena)
    {
        ValidarLargoContrasena(contrasena, 8);
        ValidarAlgunaMayusculaContrasena(contrasena);
        ValidarAlgunaMinusculaContrasena(contrasena);
        ValidarAlgunNumeroContrasena(contrasena);
        ValidarAlgunCaracterEspecialContrasena(contrasena);
    }

    private void ValidarLargoContrasena(string contrasena, int largoMinimo)
    {
        if (contrasena.Length < largoMinimo)
        {
            throw new ExcepcionDominio($"La contraseña debe tener al menos {largoMinimo} caracteres.");
        }
    }

    private void ValidarAlgunaMayusculaContrasena(string contrasena)
    {
        if (!contrasena.Any(char.IsUpper))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos una letra mayúscula (A-Z).");
        }
    }

    private void ValidarAlgunaMinusculaContrasena(string contrasena)
    {
        if (!contrasena.Any(char.IsLower))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos una letra minúscula (a-z).");
        }
    }

    private void ValidarAlgunNumeroContrasena(string contrasena)
    {
        if (!contrasena.Any(char.IsDigit))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos un número (0-9).");
        }
    }

    private void ValidarAlgunCaracterEspecialContrasena(string contrasena)
    {
        if (!Regex.IsMatch(contrasena, "[^a-zA-Z0-9]"))
        {
            throw new ExcepcionDominio(
                "La contraseña debe incluir al menos un carácter especial (como @, #, $, etc.).");
        }
    }

    private void ValidarEdad(DateTime fechaNacimiento)
    {
        if (fechaNacimiento.AddYears(18) > DateTime.Today)
        {
            throw new ExcepcionDominio("El usuario debe ser mayor de edad (18 años o más)");
        }
    }

    private void ValidarEmail(string email)
    {
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            throw new ExcepcionDominio("El email tiene un formato inválido");
        }
    }

    public void CambiarContrasena(string nuevaContrasena)
    {
        SetContrasenaEncriptada(nuevaContrasena);
    }

    public void RecibirNotificacion(Notificacion notificacion)
    {
        Notificaciones.Add(notificacion);
    }

    public void BorrarNotificacion(int idNotificacion)
    {
        if(! Notificaciones.Any(n => n.Id == idNotificacion))
        {
            throw new ExcepcionDominio("No existe la notificación");
        }
        Notificaciones.RemoveAll(n => n.Id == idNotificacion);
    }
    public override bool Equals(object? obj)
    {
        return obj is Usuario usuario && usuario.Id == Id;
    }
}
