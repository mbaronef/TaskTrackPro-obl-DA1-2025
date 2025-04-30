using Dominio.Dummies;
using Dominio.Excepciones;
using Dominio;

namespace Dominio;

public class GestorProyectos
{
    private static int _cantidadProyectos;
    public List<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

    public void CrearProyecto(Proyecto proyecto, Usuario solicitante)
    {
        if (!solicitante.EsAdministradorProyecto)
            throw new ExcepcionDominio("El usuario no tiene permiso para crear un proyecto.");
        
        if (solicitante.EstaAdministrandoProyecto)
            throw new ExcepcionDominio("El usuario ya está administrando un proyecto.");
        
        if (Proyectos.Any(p => p.Nombre == proyecto.Nombre))
        {
            throw new ExcepcionDominio("Ya existe un proyecto con ese nombre.");
        }

        _cantidadProyectos++;
        proyecto.AsignarId(_cantidadProyectos);
        Proyectos.Add(proyecto);
        
        solicitante.EstaAdministrandoProyecto = true;
        
        proyecto.NotificarMiembros($"Se creó el proyecto '{proyecto.Nombre}'.");
    }
    
    public void EliminarProyecto(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto = Proyectos.FirstOrDefault(p => p.Id == idProyecto);
        
        if (proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");

        if (!proyecto.EsAdministrador(solicitante))
            throw new ExcepcionDominio("Solo el administrador del proyecto puede eliminarlo.");
        
        solicitante.EstaAdministrandoProyecto = false;
        Proyectos.Remove(proyecto);
        
        proyecto.NotificarMiembros($"Se eliminó el proyecto '{proyecto.Nombre}'.");
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, Usuario solicitante)
    {
        Proyecto proyecto = Proyectos.FirstOrDefault(p => p.Id == idProyecto);
        
        if (proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");

        if (!proyecto.EsAdministrador(solicitante))
            throw new ExcepcionDominio("Solo el admin del proyecto puede cambiar el nombre.");
        
        if (Proyectos.Any(p => p.Nombre == nuevoNombre && p.Id != idProyecto))
            throw new ExcepcionDominio("Ya existe un proyecto con ese nombre.");
        
        string nombreAnterior = proyecto.Nombre;
        
        proyecto.ModificarNombre(nuevoNombre);
        
        proyecto.NotificarMiembros($"Se cambió el nombre del proyecto '{nombreAnterior}' a '{proyecto.Nombre}'.");
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, Usuario solicitante)
    {
        Proyecto proyecto = Proyectos.FirstOrDefault(p => p.Id == idProyecto);
        
        if (proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");

        if (!proyecto.EsAdministrador(solicitante))
            throw new ExcepcionDominio("Solo el admin del proyecto puede cambiar la descripción.");
        
        proyecto.ModificarDescripcion(descripcion);
        
        proyecto.NotificarMiembros($"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{proyecto.Descripcion}'.");
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto = Proyectos.FirstOrDefault(p => p.Id == idProyecto);
        
        if (proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");
        
        if(!proyecto.EsAdministrador(solicitante))
            throw new ExcepcionDominio("Solo el admin de proyecto puede cambiar la fecha de inicio del proyecto.");
        
        proyecto.ModificarFechaInicio(nuevaFecha);
        
        proyecto.NotificarMiembros($"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.");
    }

    public void CambiarAdministradorDeProyecto(Usuario solicitante, int idProyecto, int idNuevoAdmin)
    {
        if (!solicitante.EsAdministradorSistema)
            throw new ExcepcionDominio("Solo un administrador de sistema puede cambiar el administrador del proyecto.");

        Proyecto proyecto = Proyectos.FirstOrDefault(p => p.Id == idProyecto);

        if (proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");
        
        Usuario nuevoAdmin = proyecto.Miembros.FirstOrDefault(u => u.Id == idNuevoAdmin);
        
        if (nuevoAdmin is null) 
            throw new ExcepcionDominio("El nuevo administrador debe ser miembro del proyecto.");
        
        if (nuevoAdmin.EstaAdministrandoProyecto)
            throw new ExcepcionDominio("El usuario ya administra otro proyecto.");
        
        if (!nuevoAdmin.EsAdministradorProyecto)
            throw new ExcepcionDominio("El usuario no tiene los permisos de administrador de proyecto.");
        
        proyecto.Administrador.EstaAdministrandoProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        //nuevoAdmin.EstaAdministrandoProyecto = true;
        

    }



}