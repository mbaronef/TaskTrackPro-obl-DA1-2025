using Dominio.Excepciones;

namespace Dominio;

using System.Text;

public class Usuario
{
    private string _contrasena;
    
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public List<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
    public bool EsAdministradorProyecto { get; set; } =  false;
    

    public Usuario(string unNombre, string unApellido, DateTime unaFechaNacimiento, string unEmail, string unaContrasena)
    {
        ValidarLargoContrasena(unaContrasena, 8);
        ValidarAlgunaMayuscula(unaContrasena);
        ValidarAlgunaMinuscula(unaContrasena);
        if (!unaContrasena.Any(char.IsDigit))
        {
            throw new ExcepcionDominio("La contraseña debe incluir al menos una número (0-9).");
        }
        
        Nombre = unNombre;
        Apellido = unApellido;
        FechaNacimiento = unaFechaNacimiento;
        Email = unEmail;
        _contrasena = Usuario.EncriptarContrasena(unaContrasena);
    }
    
    public static string EncriptarContrasena(string contrasena)
    { 
        StringBuilder resultado = new StringBuilder(); 
        foreach (char caracter in contrasena) 
        {
            resultado.Append((char)(caracter + 3));
        } 
        return resultado.ToString();
    }
    public bool Autenticar(string contrasenaIngresada)
    {
        return (_contrasena == Usuario.EncriptarContrasena(contrasenaIngresada));
    }

    private void ValidarLargoContrasena(string contrasena, int largoMinimo)
    {
        if (contrasena.Length < largoMinimo)
        {
            throw new ExcepcionDominio($"La contraseña debe tener al menos {largoMinimo} caracteres.");
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


}