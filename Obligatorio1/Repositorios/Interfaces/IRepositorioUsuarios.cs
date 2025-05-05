using Dominio;

namespace Repositorios.Interfaces;

public interface IRepositorioUsuarios : IRepositorio<Usuario>
{ 
    Usuario ObtenerUsuarioPorEmail(string email);
    void ActualizarContrasena(int idUsuario, string contrasenaEncriptada);
    // asumimos que el usuario no puede modificar su nombre, apellido o fecha de nacimiento. Cuando se crea se ingresan correctamente.
}
