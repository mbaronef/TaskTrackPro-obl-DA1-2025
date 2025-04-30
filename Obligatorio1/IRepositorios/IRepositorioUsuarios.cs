using Dominio.Dummies;

namespace IRepositorios;

public interface IRepositorioUsuarios : IRepositorio<Usuario>
{ 
    Usuario ObtenerUsuarioPorEmail(string email);
    void ActualizarContrasena(int idUsuario, string contrasena);
}
