using System.Security.Cryptography;
using System.Text;
using Dominio.Excepciones;

namespace Dominio;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public List<Usuario> Usuarios { get; } = new List<Usuario>();

    public GestorUsuarios() { }
    
    public void AgregarUsuario(Usuario usuario)
    {
        usuario.Id = ++_cantidadUsuarios;
        Usuarios.Add(usuario);
    }

    public void EliminarUsuario(int id)
    {
        Usuario usuario = ObtenerUsuario(id);
        Usuarios.Remove(usuario);
    }

    public Usuario ObtenerUsuario(int idUsuario)
    {
        Usuario usuario = Usuarios.FirstOrDefault(u => u.Id == idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionDominio("El usuario no existe");
        }
        return usuario;
    }

    public void AgregarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = ObtenerUsuario(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void EliminarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = ObtenerUsuario(idUsuario);
        usuario.EsAdministradorSistema = false;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, Usuario nuevoAdministradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para asignar administradores de proyectos.");
        }
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void EliminarAdministradorProyecto(Usuario solicitante, Usuario administradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para eliminar administradores de proyectos.");
        }

        if (!administradorProyecto.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("El usuario a eliminar no es administrador de proyectos.");
        }

        if (administradorProyecto.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionDominio("No se puede quitar permisos de proyecto a un usuario que tiene un proyecto a su cargo.");
        }
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario solicitante, Usuario usuarioObjetivo)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto && !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para reiniciar la contraseña del usuario.");
        }
        usuarioObjetivo.CambiarContrasena(_contrasenaPorDefecto);
        Notificar(usuarioObjetivo, $"Se reinició su contraseña. La nueva contraseña es {_contrasenaPorDefecto}");
    }

    public void AutogenerarContrasena(Usuario administrador, Usuario usuarioObjetivo)
    {
        if (!administrador.EsAdministradorSistema && !administrador.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para autogenerar la contraseña del usuario.");
        }

        string minusculas = "abcdefghijklmnñopqrstuvwxyz";
        string mayusculas = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        string numeros = "1234567890";
        string simbolos = "!@#$%^&*()-_=+<>?{}[].,:;¡¿?'/~|°";
        string todosLosCaracteres = minusculas + mayusculas + numeros + simbolos;
        
        StringBuilder contrasenaAutogenerada = new StringBuilder();
        
        RandomNumberGenerator rng = RandomNumberGenerator.Create(); // generador de números aleatorios criptográficamente seguros
        
        int largo = ObtenerNumeroAleatorio(8, 15, rng); // el largo es un número random entre 8 y 15
        // agregar manualmente una mayúscula, una minúscula, un número y un caracter especial (para asegurar restricciones de contraseña)
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(minusculas, rng));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(mayusculas, rng));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(numeros, rng));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(simbolos, rng));
            
        while (contrasenaAutogenerada.Length < largo)
            contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(todosLosCaracteres, rng)); // agrega a la contraseña caracteres random hasta cumplir longitud
            
        string nuevaContrasena = MezclarCaracteres(contrasenaAutogenerada.ToString(), rng);
        usuarioObjetivo.CambiarContrasena(nuevaContrasena);
        Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
    }
    private static int ObtenerNumeroAleatorio(int min, int max, RandomNumberGenerator rng)
    {
        byte[] buffer = new byte[sizeof(uint)]; // uint: entero de 32 bits sin signo. En el buffer se almacena un número aleatorio.
        rng.GetBytes(buffer); // llena el buffer con números aleatorios
        uint numero = BitConverter.ToUInt32(buffer, 0); // se convierte el buffer en número de tipo uint
        // asegurar que el número esté en el rango
        int rango = max - min + 1;
        return (int)(numero % rango) + min;
    }
    private static char ObtenerCaracterAleatorio(string caracteres, RandomNumberGenerator rng)
    {
        int indice = ObtenerNumeroAleatorio(0, caracteres.Length - 1, rng);
        return caracteres[indice];
    }
    private static string MezclarCaracteres(string input, RandomNumberGenerator rng)
    {
        char[] array = input.ToCharArray(); // Convierte la cadena en un array de caracteres para poder recorrerla char a char
        for (int i = array.Length - 1; i > 0; i--)
        { //shuffle
            int j = ObtenerNumeroAleatorio(0, i, rng);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array); // Convierte el array de nuevo a una cadena
    }

    public void ModificarContrasena(Usuario solicitante, Usuario usuarioObjetivo, string nuevaContrasena)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto && !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para modificar la contraseña del usuario.");
        }
        usuarioObjetivo.CambiarContrasena(nuevaContrasena);
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
        }
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        usuario.RecibirNotificacion(mensajeNotificacion);
    }

    public Usuario LogIn(string email, string contrasena)
    {
        Usuario usuario = Usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario == null)
        {
            throw new ExcepcionDominio("Correo electrónico no registrado.");
        }
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionDominio("La contraseña ingresada es incorrecta.");
        }
        return usuario;
    }
}