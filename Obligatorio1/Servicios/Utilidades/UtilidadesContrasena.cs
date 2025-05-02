using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Servicios.Excepciones;

namespace Servicios.Utilidades;

public static class UtilidadesContrasena
{
    private static readonly int _largoMinimoContrasena = 8;
    private static readonly int _largoMaximoContrasena = 15; //Se define para no autogenerar una contraseña demasiado larga
    
    public static string ValidarYEncriptarContrasena(string contrasena)
    {
        ValidarFormatoContrasena(contrasena);
        return BCrypt.Net.BCrypt.HashPassword(contrasena); //Encripta la contraseña utilizando el algoritmo BCrypt
    }
    
    public static string AutogenerarContrasenaValida()
    {
        string minusculas = "abcdefghijklmnñopqrstuvwxyz";
        string mayusculas = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        string numeros = "1234567890";
        string simbolos = "!@#$%^&*()-_=+<>?{}[].,:;¡¿?'/~|°";
        string todosLosCaracteres = minusculas + mayusculas + numeros + simbolos;
        
        StringBuilder contrasenaAutogenerada = new StringBuilder();
        
        RandomNumberGenerator generadorDeNumerosAleatorio = RandomNumberGenerator.Create(); // generador de números aleatorios criptográficamente seguros
        
        int largo = GenerarNumeroAleatorio(_largoMinimoContrasena, _largoMaximoContrasena, generadorDeNumerosAleatorio); // el largo es un número random entre 8 y 15
        // agregar manualmente una mayúscula, una minúscula, un número y un caracter especial (para asegurar restricciones de contraseña)
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(minusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(mayusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(numeros, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(simbolos, generadorDeNumerosAleatorio));
            
        while (contrasenaAutogenerada.Length < largo)
            contrasenaAutogenerada.Append(GenerarCaracterAleatorio(todosLosCaracteres, generadorDeNumerosAleatorio)); // agrega a la contraseña caracteres random hasta cumplir longitud
        return MezclarCaracteres(contrasenaAutogenerada.ToString(), generadorDeNumerosAleatorio);
    }
    
    private static void ValidarFormatoContrasena(string contrasena)
    {
        ValidarLargoContrasena(contrasena);
        ValidarAlgunaMayuscula(contrasena);
        ValidarAlgunaMinuscula(contrasena);
        ValidarAlgunNumero(contrasena);
        ValidarAlgunCaracterEspecial(contrasena);
    }
    private static void ValidarLargoContrasena(string contrasena)
    {
        if (contrasena.Length < _largoMinimoContrasena)
        {
            throw new ExcepcionServicios($"La contraseña debe tener al menos {_largoMinimoContrasena} caracteres.");
        }
    } 
    private static void ValidarAlgunaMayuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsUpper))
        {
            throw new ExcepcionServicios("La contraseña debe incluir al menos una letra mayúscula (A-Z).");
        }
    } 
    private static void ValidarAlgunaMinuscula(string contrasena)
    {
        if (!contrasena.Any(char.IsLower))
        {
            throw new ExcepcionServicios("La contraseña debe incluir al menos una letra minúscula (a-z).");
        }
    } 
    private static void ValidarAlgunNumero(string contrasena)
    {
        if (!contrasena.Any(char.IsDigit))
        {
            throw new ExcepcionServicios("La contraseña debe incluir al menos un número (0-9).");
        }
    } 
    private static void ValidarAlgunCaracterEspecial(string contrasena)
    {
        if (!Regex.IsMatch(contrasena, "[^a-zA-Z0-9]")) // RegEx para que haya algún caracter distinto a minúsuclas, mayúsuclas o números
        {
            throw new ExcepcionServicios(
                "La contraseña debe incluir al menos un carácter especial (como @, #, $, etc.).");
        }
    } 
    
    private static int GenerarNumeroAleatorio(int min, int max, RandomNumberGenerator rng)
    {
        byte[] buffer = new byte[sizeof(uint)]; // uint: entero de 32 bits sin signo. En el buffer se almacena un número aleatorio.
        rng.GetBytes(buffer); // llena el buffer con números aleatorios
        uint numero = BitConverter.ToUInt32(buffer, 0); // se convierte el buffer en número de tipo uint
        // asegurar que el número esté en el rango
        int rango = max - min + 1;
        return (int)(numero % rango) + min;
    } 
    private static char GenerarCaracterAleatorio(string caracteres, RandomNumberGenerator rng)
    {
        int indice = GenerarNumeroAleatorio(0, caracteres.Length - 1, rng);
        return caracteres[indice];
    } 
    private static string MezclarCaracteres(string input, RandomNumberGenerator rng)
    {
        char[] array = input.ToCharArray(); // Convierte la cadena en un array de caracteres para poder recorrerla char a char
        for (int i = array.Length - 1; i > 0; i--)
        { //shuffle
            int j = GenerarNumeroAleatorio(0, i, rng);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array); // Convierte el array de nuevo a una cadena
    }
}