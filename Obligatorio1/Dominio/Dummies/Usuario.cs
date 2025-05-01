using System.Text.RegularExpressions;
using Dominio.Excepciones;

namespace Dominio.Dummies;

public class Usuario
{
    private string _contrasena;
    private static readonly int _largoMinimoContrasena = 8;
    private static readonly int _edadMinima = 18;
    private static readonly int _edadMaxima = 100;

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public List<Notificacion> Notificaciones { get; } = new List<Notificacion>();
    public bool EsAdministradorSistema { get; set; } = false;

    public bool EsAdministradorProyecto { get; set; } = false;

    public bool EstaAdministrandoUnProyecto { get; set; } = false;


    public Usuario(string nombre, string apellido, DateTime fechaNacimiento, string email, string contrasena)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(apellido, "apellido");
        ValidarEdad(fechaNacimiento);
        ValidarEmail(email);
        EstablecerContrasena(contrasena);
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        Email = email;
    }

    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrEmpty(texto))
        {
            throw new ExcepcionDominio($"El atributo {nombreAtributo} no puede ser vacío");   
        }
    }

    public void EstablecerContrasena(string contrasena)
    {
        ValidarContrasena(contrasena);
        _contrasena = Usuario.EncriptarContrasena(contrasena);
    }

    public static string EncriptarContrasena(string contrasena)
    {
        return BCrypt.Net.BCrypt.HashPassword(contrasena); //Encripta la contraseña utilizando el algoritmo BCrypt
    }

    public bool Autenticar(string contrasenaIngresada)
    {
        return BCrypt.Net.BCrypt.Verify(contrasenaIngresada, _contrasena);
    }

    private void ValidarContrasena(string contrasena)
    {
        ValidarLargoContrasena(contrasena);
        ValidarAlgunaMayuscula(contrasena);
        ValidarAlgunaMinuscula(contrasena);
        ValidarAlgunNumero(contrasena);
        ValidarAlgunCaracterEspecial(contrasena);
    }

    private void ValidarLargoContrasena(string contrasena)
    {
        if (contrasena.Length < _largoMinimoContrasena)
        {
            throw new ExcepcionDominio($"La contraseña debe tener al menos {_largoMinimoContrasena} caracteres.");
        }
    }

    private void ValidarAlgunaMayuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsUpper))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos una letra mayúscula (A-Z).");
        }
    }

    private void ValidarAlgunaMinuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsLower))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos una letra minúscula (a-z).");
        }
    }

    private void ValidarAlgunNumero(string contrasena)
    {
        if (!contrasena.Any(char.IsDigit))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos un número (0-9).");
        }
    }

    private void ValidarAlgunCaracterEspecial(string contrasena)
    {
        if (!Regex.IsMatch(contrasena, "[^a-zA-Z0-9]")) // RegEx para que haya algún caracter distinto a minúsuclas, mayúsuclas o números
        {
            throw new ExcepcionDominio(
                "La contraseña debe incluir al menos un carácter especial (como @, #, $, etc.).");
        }
    }

    private void ValidarEdad(DateTime fechaNacimiento)
    {
        if (fechaNacimiento.AddYears(_edadMinima) > DateTime.Today)
        {
            throw new ExcepcionDominio($"El usuario debe tener más de {_edadMinima} años");
        }

        if (fechaNacimiento.AddYears(_edadMaxima) <= DateTime.Today)
        {
            throw new ExcepcionDominio($"El usuario debe tener menos de {_edadMaxima} años");
        }
    }

    private void ValidarEmail(string email)
    {
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) // RegEx que valida formato con arroba en medio y extensión de dominio (ej. .com)
        {
            throw new ExcepcionDominio("El email tiene un formato inválido");
        }
    }

    public void CambiarEmail(string nuevoEmail)
    {
        ValidarEmail(nuevoEmail);
        Email = nuevoEmail;
    }

    public void RecibirNotificacion(string mensaje)
    {
        Notificacion notificacion = new Notificacion(mensaje);
        Notificaciones.Add(notificacion);
    }

    public void BorrarNotificacion(int idNotificacion)
    {
        int indice = Notificaciones.FindIndex(n => n.Id == idNotificacion);
        if (indice == -1)
        {
            throw new ExcepcionDominio("No existe la notificación");
        }
        Notificaciones.RemoveAt(indice);

    }
    public override bool Equals(object? otro){
        bool retorno = false;
        Usuario otroUsuario = otro as Usuario;
        if (otroUsuario != null)
        {
            retorno = otroUsuario.Id == Id;
        }
        return retorno;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}