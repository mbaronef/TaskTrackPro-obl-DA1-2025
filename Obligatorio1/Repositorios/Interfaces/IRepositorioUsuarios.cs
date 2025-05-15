using Dominio;

namespace Repositorios.Interfaces;

public interface IRepositorioUsuarios : IRepositorio<Usuario>
{ 
    Usuario ObtenerUsuarioPorEmail(string email);
}
