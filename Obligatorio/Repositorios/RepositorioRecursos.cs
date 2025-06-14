using System.Security.Cryptography;
using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioRecursos : IRepositorio<Recurso>
{
    private SqlContext _contexto;

    public RepositorioRecursos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Recurso objeto)
    {
        _contexto.Recursos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Recurso ObtenerPorId(int id)
    {
        return _contexto.Recursos.
            Include(r => r.ProyectoAsociado).
            FirstOrDefault(recurso => recurso.Id == id);
    }

    public void Eliminar(int id)
    {
        Recurso recursoAEliminar = _contexto.Recursos.FirstOrDefault(recurso => recurso.Id == id);
        _contexto.Remove(recursoAEliminar);
        _contexto.SaveChanges();
    }

    public List<Recurso> ObtenerTodos()
    {
        return _contexto.Recursos.
            Include(r=> r.ProyectoAsociado)
            .ToList();
    }
    
    public void Actualizar(Recurso recurso)
    {
        Recurso recursoContexto = ObtenerPorId(recurso.Id);
        
        if (recursoContexto != null)
        {
            if (recurso.ProyectoAsociado != null)
            {
                Proyecto proyectoAsociadoContexto = _contexto.Proyectos
                    .FirstOrDefault(p => p.Id == recurso.ProyectoAsociado.Id);
    
                recursoContexto.ProyectoAsociado = proyectoAsociadoContexto;
            }
            else
            {
                recursoContexto.ProyectoAsociado = null;
            }
            
            recursoContexto.Nombre = recurso.Nombre;
            recursoContexto.Tipo = recurso.Tipo;
            recursoContexto.Descripcion = recurso.Descripcion;
            recursoContexto.CantidadDeTareasUsandolo = recurso.CantidadDeTareasUsandolo;
            _contexto.SaveChanges();
        }
    }
}