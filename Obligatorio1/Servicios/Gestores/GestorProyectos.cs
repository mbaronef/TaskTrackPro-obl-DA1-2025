using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorProyectos
{
    public RepositorioProyectos Proyectos { get; } = new RepositorioProyectos();
    
    public void CrearProyecto(Proyecto proyecto, Usuario solicitante)
    {
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyecto.Nombre);
        
        Proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;

        proyecto.NotificarMiembros($"Se creó el proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarProyecto(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        solicitante.EstaAdministrandoUnProyecto = false;
        Proyectos.Eliminar(proyecto.Id);

        proyecto.NotificarMiembros($"Se eliminó el proyecto '{proyecto.Nombre}'.");
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarNombreNoRepetido(nuevoNombre);
        
        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);

        proyecto.NotificarMiembros($"Se cambió el nombre del proyecto '{nombreAnterior}' a '{proyecto.Nombre}'.");
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarDescripcion(descripcion);

        proyecto.NotificarMiembros($"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{proyecto.Descripcion}'.");
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaInicio(nuevaFecha);
        
        CaminoCritico.CalcularCaminoCritico(proyecto);

        proyecto.NotificarMiembros($"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.");
    }

    public void CambiarAdministradorDeProyecto(Usuario solicitante, int idProyecto, int idNuevoAdmin)
    {
        VerificarUsuarioEsAdminSistema(solicitante);

        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);
        
        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");
        
        proyecto.Administrador.EstaAdministrandoUnProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoUnProyecto = true;
        
        proyecto.NotificarMiembros($"Se cambió el administrador del proyecto '{proyecto.Nombre}'. El nuevo administrador es '{nuevoAdmin}'.");
    }

    public void AgregarMiembroAProyecto(int idProyecto, Usuario solicitante, Usuario nuevoMiembro)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AsignarMiembro(nuevoMiembro);

        proyecto.NotificarMiembros($"Se agregó a un nuevo miembro (id {nuevoMiembro.Id}) al proyecto '{proyecto.Nombre}'.");

    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, Usuario solicitante, int idMiembroAEliminar)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        proyecto.NotificarMiembros($"Se eliminó a el miembro (id {idMiembroAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }

    public List<Proyecto> ObtenerProyectosPorUsuario(int idUsuario)
    {
        return Proyectos.ObtenerTodos().Where(proyecto => proyecto.Miembros.Any(usuario => usuario.Id == idUsuario)).ToList();
    }
    
    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = Proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);
        
        if (proyecto == null)
            throw new ExcepcionServicios("No se encontró un proyecto administrado por ese usuario.");

        return proyecto;
    }
    
    public Proyecto ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = Proyectos.ObtenerPorId(id);
        
        if(proyecto is null)
            throw new ExcepcionServicios("El proyecto no existe.");
        
        return proyecto;
    }

    public void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
            throw new ExcepcionServicios("Solo el administrador del proyecto puede realizar esta acción.");
    }
    
    public void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, String tipoUsuario)
    {
        if(!solicitante.EsAdministradorProyecto)
            throw new ExcepcionServicios($"El {tipoUsuario} no tiene los permisos de administrador de proyecto.");
    }
    
    public void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);
        
        if (usuario is null)
            throw new ExcepcionServicios("El usuario no es miembro del proyecto.");
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = Proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
            throw new ExcepcionServicios($"Ya existe un proyecto con el nombre '{nuevoNombre}'.");
    }

    private void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
            throw new ExcepcionServicios("El usuario ya está administrando un proyecto.");
    }

    private void VerificarUsuarioEsAdminSistema(Usuario usuario)
    {
        if (!usuario.EsAdministradorSistema)
            throw new ExcepcionServicios("El solicitante no es administrador de sistema.");
    }

    private Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        
        return miembro;
    }
}