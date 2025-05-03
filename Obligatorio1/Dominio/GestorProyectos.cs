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
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyecto.Nombre);

        _cantidadProyectos++;
        proyecto.AsignarId(_cantidadProyectos);
        Proyectos.Add(proyecto);

        solicitante.EstaAdministrandoProyecto = true;

        proyecto.NotificarMiembros($"Se creó el proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarProyecto(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        solicitante.EstaAdministrandoProyecto = false;
        Proyectos.Remove(proyecto);

        proyecto.NotificarMiembros($"Se eliminó el proyecto '{proyecto.Nombre}'.");
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyecto(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarNombreNoRepetido(nuevoNombre);
        
        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);

        proyecto.NotificarMiembros($"Se cambió el nombre del proyecto '{nombreAnterior}' a '{proyecto.Nombre}'.");
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarDescripcion(descripcion);

        proyecto.NotificarMiembros($"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{proyecto.Descripcion}'.");
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaInicio(nuevaFecha);

        proyecto.NotificarMiembros($"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.");
    }
    
    public void ModificarFechaFinMasTempranaDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyecto(idProyecto);
        
        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaFinMasTemprana(nuevaFecha);
        
        proyecto.NotificarMiembros($"Se cambió la fecha de fin más temprana del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.");
    }

    public void CambiarAdministradorDeProyecto(Usuario solicitante, int idProyecto, int idNuevoAdmin)
    {
        VerificarUsuarioEsAdminSistema(solicitante);

        Proyecto proyecto = ObtenerProyecto(idProyecto);

        VerificarUsuarioMiembroDelProyecto(idNuevoAdmin, proyecto);
        
        Usuario nuevoAdmin = ObtenerMiembro(idNuevoAdmin, proyecto);

        VerificarUsuarioNoAdministraOtroProyecto(nuevoAdmin);

        VerificarUsuarioTengaPermisosDeAdminProyecto(nuevoAdmin, "el nuevo administrador");
        
        proyecto.Administrador.EstaAdministrandoProyecto = false;
        proyecto.Administrador = nuevoAdmin;
        nuevoAdmin.EstaAdministrandoProyecto = true;
        
        proyecto.NotificarMiembros($"Se cambió el administrador del proyecto '{proyecto.Nombre}'. El nuevo administrador es '{nuevoAdmin}'.");
    }

    public void AgregarMiembroAProyecto(int idProyecto, Usuario solicitante, Usuario nuevoMiembro)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);

        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AsignarMiembro(nuevoMiembro);

        proyecto.NotificarMiembros($"Se agregó a un nuevo miembro (id {nuevoMiembro.Id}) al proyecto '{proyecto.Nombre}'.");

    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, Usuario solicitante, int idMiembroAEliminar)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        proyecto.NotificarMiembros($"Se eliminó a el miembro (id {idMiembroAEliminar}) del proyecto '{proyecto.Nombre}'.");
    }

    public void AgregarTareaAlProyecto(int idProyecto,  Usuario solicitante, Tarea nuevaTarea)
    {
        Proyecto proyecto =  ObtenerProyecto(idProyecto);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        
        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AgregarTarea(nuevaTarea);
        
        proyecto.NotificarMiembros($"Se agregó la tarea (id {nuevaTarea.Id}) al proyecto '{proyecto.Nombre}'.");
    }

    public void EliminarTareaDelProyecto(int idProyecto, Usuario solicitante, int idTareaAEliminar)
    {
        Proyecto proyecto = ObtenerProyecto(idProyecto);
         
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");
        
        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.EliminarTarea(idTareaAEliminar);
        
        proyecto.NotificarMiembros($"Se eliminó la tarea (id {idTareaAEliminar}) del proyecto '{proyecto.Nombre}'.");
        
    }

    public List<Proyecto> ObtenerProyectosPorUsuario(int idUsuario)
    {
        return Proyectos.Where(proyecto => proyecto.Miembros.Any(usuario => usuario.Id == idUsuario)).ToList();
    }

    private Proyecto ObtenerProyecto(int id)
    {
        Proyecto proyecto = Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
        
        if(proyecto is null)
            throw new ExcepcionDominio("El proyecto no existe.");
        
        return proyecto;
    }
    
    private void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
            throw new ExcepcionDominio("Solo el administrador del proyecto puede realizar esta acción.");
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = Proyectos.Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
            throw new ExcepcionDominio($"Ya existe un proyecto con el nombre '{nuevoNombre}'.");
    }

    private void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoProyecto)
            throw new ExcepcionDominio("El usuario ya está administrando un proyecto.");
    }

    private void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, String tipoUsuario)
    {
        if(!solicitante.EsAdministradorProyecto)
            throw new ExcepcionDominio($"El {tipoUsuario} no tiene los permisos de administrador de proyecto.");
    }

    private void VerificarUsuarioEsAdminSistema(Usuario usuario)
    {
        if (!usuario.EsAdministradorSistema)
            throw new ExcepcionDominio("El solicitante no es administrador de sistema.");
    }

    private void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);
        
        if (usuario is null)
            throw new ExcepcionDominio("El usuario no es miembro del proyecto.");
    }

    private Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        
        return miembro;
    }

}