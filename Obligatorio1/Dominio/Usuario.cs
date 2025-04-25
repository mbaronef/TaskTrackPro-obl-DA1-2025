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
        if (unaContrasena.Length < 8)
        {
            throw new ExcepcionDominio("La contraseña debe tener al menos 8 caracteres.");
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
    
    /*public bool contrasenaValida()
    {
        return this.Contrasena.Any(char.IsUpper);
    }*/
}