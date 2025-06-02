using Dominio;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Notificaciones;
using Servicios.Utilidades;

namespace Servicios.Gestores;

public class GestorProyectos
{
    public RepositorioProyectos Proyectos { get; } = new RepositorioProyectos();
    private readonly INotificador _notificador;
    
    public GestorProyectos(INotificador notificador)
    {
        _notificador = notificador;
    }

    public void CrearProyecto(Proyecto proyecto, Usuario solicitante)
    {
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioNoAdministraOtroProyecto(solicitante);

        VerificarNombreNoRepetido(proyecto.Nombre);
        
        Proyectos.Agregar(proyecto);

        solicitante.EstaAdministrandoUnProyecto = true;

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.ProyectoCreado(proyecto.Nombre));
    }

    public void EliminarProyecto(int idProyecto, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        solicitante.EstaAdministrandoUnProyecto = false;
        Proyectos.Eliminar(proyecto.Id);
        foreach (Usuario miembro in proyecto.Miembros)
        {
            miembro.CantidadProyectosAsignados--;
        }

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.ProyectoEliminado(proyecto.Nombre));
    }

    public void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, Usuario solicitante)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarNombreNoRepetido(nuevoNombre);
        
        string nombreAnterior = proyecto.Nombre;

        proyecto.ModificarNombre(nuevoNombre);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.NombreProyectoModificado(nombreAnterior, proyecto.Nombre));
    }

    public void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarDescripcion(descripcion);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, proyecto.Descripcion));
    }

    public void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, Usuario solicitante)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.ModificarFechaInicio(nuevaFecha);
        
        CaminoCritico.CalcularCaminoCritico(proyecto);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha));
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
        
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre, nuevoAdmin.ToString()));
    }

    public void AgregarMiembroAProyecto(int idProyecto, Usuario solicitante, Usuario nuevoMiembro)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);

        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        proyecto.AsignarMiembro(nuevoMiembro);

        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.MiembroAgregado(proyecto.Nombre, nuevoMiembro.Id));

    }
    
    public void EliminarMiembroDelProyecto(int idProyecto, Usuario solicitante, int idMiembroAEliminar)
    {
        Proyecto proyecto =  ObtenerProyectoPorId(idProyecto);
        
        VerificarUsuarioTengaPermisosDeAdminProyecto(solicitante, "solicitante");

        VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, solicitante);
        
        VerificarUsuarioMiembroDelProyecto(idMiembroAEliminar, proyecto);

        VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idMiembroAEliminar);
        
        proyecto.EliminarMiembro(idMiembroAEliminar);
        
        _notificador.NotificarMuchos(proyecto.Miembros, MensajesNotificacion.MiembroEliminado(proyecto.Nombre, idMiembroAEliminar));
    }

    public List<Proyecto> ObtenerProyectosPorUsuario(int idUsuario)
    {
        return Proyectos.ObtenerTodos().Where(proyecto => proyecto.Miembros.Any(usuario => usuario.Id == idUsuario)).ToList();
    }
    
    public Proyecto ObtenerProyectoDelAdministrador(int idAdministrador)
    {
        Proyecto proyecto = Proyectos.ObtenerTodos().FirstOrDefault(p => p.Administrador.Id == idAdministrador);

        if (proyecto == null)
        {
            throw new ExcepcionProyecto(MensajesError.UsuarioNoAdministraProyectos);
        }

        return proyecto;
    }
    
    public Proyecto ObtenerProyectoPorId(int id)
    {
        Proyecto proyecto = Proyectos.ObtenerPorId(id);
        
        if(proyecto is null)
        {
            throw new ExcepcionPermisos(MensajesError.ProyectoNoEncontrado);
        }
        return proyecto;
    }

    public void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
        {
            throw new ExcepcionPermisos(MensajesError.NoEsAdminDelProyecto);
        } 
    }

    public void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, string tipoUsuario)
    {
        if (!solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.PermisoDenegadoPorTipo(tipoUsuario));
        }
    }

    public void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);

        if (usuario is null)
        {
            throw new ExcepcionProyecto(MensajesError.UsuarioNoMiembroDelProyecto);
        }
    }
    
    public void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar)
    {
        Proyecto proyecto = ObtenerProyectoPorId(idProyecto);
        Usuario miembroAEliminar = ObtenerMiembro(idMiembroAEliminar, proyecto);
        if (proyecto.Tareas.Any(tarea => tarea.EsMiembro(miembroAEliminar)))
        {
            throw new ExcepcionProyecto(MensajesError.UsuarioConTareas);
        }
    }

    public Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }
    public void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje)
    {
        proyectos.ForEach(proyecto => _notificador.NotificarUno(proyecto.Administrador, mensaje));
    }

    private void VerificarNombreNoRepetido(string nuevoNombre)
    {
        bool existeOtro = Proyectos.ObtenerTodos().Any(proyecto => proyecto.Nombre == nuevoNombre);

        if (existeOtro)
        {
            throw new ExcepcionProyecto(MensajesError.NombreRepetido);
        }
    }

    private void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioAdministrandoProyecto);
        }
    }

    private void VerificarUsuarioEsAdminSistema(Usuario usuario)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionPermisos(MensajesError.UsuarioNoAdminSistema);
        }
    }
}