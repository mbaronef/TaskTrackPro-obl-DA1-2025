using Dominio;

namespace Interfaces.InterfacesRepositorios;

public interface IRepositorioUsuarios : IRepositorio<Usuario>
{ 
    Usuario ObtenerUsuarioPorEmail(string email);
}
