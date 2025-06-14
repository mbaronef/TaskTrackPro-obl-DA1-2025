using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private SqlContext _contexto;

    public RepositorioUsuarios(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Usuario objeto)
    {
        _contexto.Usuarios.Add(objeto);
        _contexto.SaveChanges();
    }

    public Usuario ObtenerPorId(int id)
    {
        return _contexto.Usuarios.
            Include(u=>u.Notificaciones)
            .FirstOrDefault(usuario => usuario.Id == id);
    }
    
    public Usuario ObtenerUsuarioPorEmail(string email)
    {
        return _contexto.Usuarios.
            Include(u=> u.Notificaciones)
            .FirstOrDefault(usuario => usuario.Email == email);
    }

    public void Eliminar(int id)
    {
        Usuario usuarioAEliminar = _contexto.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
        _contexto.Usuarios.Remove(usuarioAEliminar);
        _contexto.SaveChanges();
    }
    
    public List<Usuario> ObtenerTodos()
    {
        return _contexto.Usuarios
            .Include(u=>u.Notificaciones)
            .ToList();
    }
    
    public void Actualizar(Usuario usuario)
    {
        Usuario usuarioContexto = ObtenerPorId(usuario.Id);
        
        
        if (usuarioContexto != null)
        {
            usuarioContexto.CambiarEmail(usuario.Email);
            usuarioContexto.EsAdministradorProyecto = usuario.EsAdministradorProyecto;
            usuarioContexto.EsAdministradorSistema = usuario.EsAdministradorSistema;
            usuarioContexto.EstaAdministrandoUnProyecto = usuario.EstaAdministrandoUnProyecto;
            usuarioContexto.EstablecerContrasenaEncriptada(usuario.ObtenerContrasenaEncriptada());
            
            List<Notificacion> notisEnContexto = usuarioContexto.Notificaciones.ToList();
            
            foreach (Notificacion notiExistente in notisEnContexto)
            {
                if (!usuario.Notificaciones.Any(n => n.Id == notiExistente.Id))
                {
                    usuarioContexto.Notificaciones.Remove(notiExistente);
                }
            }
            
            foreach (Notificacion nuevaNoti in usuario.Notificaciones)
            {
                if (!usuarioContexto.Notificaciones.Any(n => n.Id == nuevaNoti.Id))
                {
                    usuarioContexto.Notificaciones.Add(nuevaNoti);
                }
            }
            
            _contexto.SaveChanges();
        }
    }
}