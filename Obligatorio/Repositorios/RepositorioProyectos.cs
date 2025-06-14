using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorios.Interfaces;

namespace Repositorios;

public class RepositorioProyectos : IRepositorioProyectos
{
    private SqlContext _contexto;

    public RepositorioProyectos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Proyecto objeto)
    {
        _contexto.Proyectos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Proyecto ObtenerPorId(int id)
    {
        return _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.Dependencias)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.UsuariosAsignados)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.RecursosNecesarios)
            .FirstOrDefault(proyecto => proyecto.Id == id);
    }

    public void Eliminar(int id)
    {
        Proyecto proyecto = _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
        _contexto.Proyectos.Remove(proyecto);
        _contexto.SaveChanges();
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.Dependencias)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.UsuariosAsignados)
            .Include(p => p.Tareas)
            .ThenInclude(t => t.RecursosNecesarios)
            .ToList();
    }

    public void Actualizar(Proyecto proyecto)
    {
        var proyectoContexto = _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
            .FirstOrDefault(p => p.Id == proyecto.Id);

        if (proyectoContexto != null)
        {
            // Atributos escalares
            proyectoContexto.ModificarNombre(proyecto.Nombre);
            proyectoContexto.ModificarDescripcion(proyecto.Descripcion);
            proyectoContexto.ModificarFechaFinMasTemprana(proyecto.FechaFinMasTemprana);
            proyectoContexto.ModificarFechaInicio(proyecto.FechaInicio);

            // ✅ Cambiar administrador si es diferente
            if (proyecto.Administrador.Id != proyectoContexto.Administrador.Id)
            {
                var nuevoAdmin = _contexto.Usuarios.FirstOrDefault(u => u.Id == proyecto.Administrador.Id);
                proyectoContexto.Administrador = nuevoAdmin;
            }

            // ✅ Actualizar Miembros (agregar/eliminar)
            var miembrosContextoIds = proyectoContexto.Miembros.Select(m => m.Id).ToHashSet();
            var nuevosMiembrosIds = proyecto.Miembros.Select(m => m.Id).ToHashSet();

            // eliminar miembros que ya no están
            var miembrosAEliminar = proyectoContexto.Miembros
                .Where(m => !nuevosMiembrosIds.Contains(m.Id))
                .ToList(); // hacemos copia para evitar modificar la colección mientras iteramos

            foreach (var miembro in miembrosAEliminar)
            {
                proyectoContexto.Miembros.Remove(miembro);
            }

            // Agregar miembros nuevos
            foreach (var miembro in proyecto.Miembros)
            {
                if (!miembrosContextoIds.Contains(miembro.Id))
                {
                    var miembroDb = _contexto.Usuarios.FirstOrDefault(u => u.Id == miembro.Id);
                    if (miembroDb != null)
                        proyectoContexto.Miembros.Add(miembroDb);
                }
            }

            // ✅ Actualizar Tareas (agregar/eliminar)
            var tareasContextoIds = proyectoContexto.Tareas.Select(t => t.Id).ToHashSet();
            var nuevasTareasIds = proyecto.Tareas.Select(t => t.Id).ToHashSet();

            var tareasAEliminar = proyectoContexto.Tareas
                .Where(t => !nuevasTareasIds.Contains(t.Id))
                .ToList();

            foreach (var tarea in tareasAEliminar)
            {
                proyectoContexto.Tareas.Remove(tarea);
            }
            

            _contexto.SaveChanges();
        }
    }

    public void ActualizarTarea(Tarea tarea)
    {
        var tareaContexto = _contexto.Set<Tarea>()
            .Include(t => t.RecursosNecesarios)
            .Include(t => t.UsuariosAsignados)
            .Include(t => t.Dependencias)
            .FirstOrDefault(t => t.Id == tarea.Id);
        if (tareaContexto != null)
        {
            // Atributos escalares
            tareaContexto.ModificarTitulo(tarea.Titulo);
            tareaContexto.ModificarDescripcion(tarea.Descripcion);
            tareaContexto.ModificarDuracion(tarea.DuracionEnDias);
            tareaContexto.ModificarFechaInicioMasTemprana(tarea.FechaInicioMasTemprana);
            tareaContexto.CambiarEstado(tarea.Estado);

            // ✅ Actualizar usuarios asignados
            var usuariosActualesIds = tareaContexto.UsuariosAsignados.Select(u => u.Id).ToHashSet();
            var nuevosUsuariosIds = tarea.UsuariosAsignados.Select(u => u.Id).ToHashSet();

            // Eliminar
            var usuariosAEliminar = tareaContexto.UsuariosAsignados
                .Where(u => !nuevosUsuariosIds.Contains(u.Id))
                .ToList();

            foreach (var usuario in usuariosAEliminar)
                tareaContexto.UsuariosAsignados.Remove(usuario);

            // Agregar
            foreach (var usuario in tarea.UsuariosAsignados)
            {
                if (!usuariosActualesIds.Contains(usuario.Id))
                {
                    var usuarioDb = _contexto.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);
                    if (usuarioDb != null)
                        tareaContexto.UsuariosAsignados.Add(usuarioDb);
                }
            }

            // ✅ Actualizar recursos necesarios
            var recursosActualesIds = tareaContexto.RecursosNecesarios.Select(r => r.Id).ToHashSet();
            var nuevosRecursosIds = tarea.RecursosNecesarios.Select(r => r.Id).ToHashSet();

            var recursosAEliminar = tareaContexto.RecursosNecesarios
                .Where(r => !nuevosRecursosIds.Contains(r.Id))
                .ToList();

            foreach (var recurso in recursosAEliminar)
                tareaContexto.RecursosNecesarios.Remove(recurso);

            foreach (var recurso in tarea.RecursosNecesarios)
            {
                if (!recursosActualesIds.Contains(recurso.Id))
                {
                    var recursoDb = _contexto.Recursos.FirstOrDefault(r => r.Id == recurso.Id);
                    if (recursoDb != null)
                        tareaContexto.RecursosNecesarios.Add(recursoDb);
                }
            }

            // ✅ Actualizar dependencias
            var dependenciasActualesIds = tareaContexto.Dependencias.Select(d => d.Tarea.Id).ToHashSet();
            var nuevasDependenciasIds = tarea.Dependencias.Select(d => d.Tarea.Id).ToHashSet();

            var depsAEliminar = tareaContexto.Dependencias
                .Where(d => !nuevasDependenciasIds.Contains(d.Tarea.Id))
                .ToList();

            foreach (var dep in depsAEliminar)
                tareaContexto.Dependencias.Remove(dep);

            foreach (var dependenciaNueva in tarea.Dependencias)
            {
                if (!tareaContexto.Dependencias.Any(d => d.Tarea.Id == dependenciaNueva.Tarea.Id))
                {
                    // Primero obtenemos la tarea dependiente del contexto (bd)
                    var tareaDep = _contexto.Set<Tarea>().FirstOrDefault(t => t.Id == dependenciaNueva.Tarea.Id);
                    if (tareaDep != null)
                    {
                        // Agregamos una nueva instancia de Dependencia para crear la relación
                        tareaContexto.Dependencias.Add(new Dependencia
                        {
                            Tipo = dependenciaNueva.Tipo,
                            Tarea = tareaDep
                        });
                    }
                }
            }
            _contexto.SaveChanges();
        }
    }
    
    public void AgregarTarea(Proyecto proyecto, Tarea nuevaTarea)
    {
        var proyectoDb = _contexto.Proyectos
            .Include(p => p.Tareas)
            .FirstOrDefault(p => p.Id == proyecto.Id);

        if (proyectoDb != null)
        {
            _contexto.Set<Tarea>().Add(nuevaTarea);
            _contexto.SaveChanges();
        }
    }
}