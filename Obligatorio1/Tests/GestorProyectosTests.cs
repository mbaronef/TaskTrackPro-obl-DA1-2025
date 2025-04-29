using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class GestorProyectosTests
    {
        //crearProyecto
        [TestMethod]
        public void CrearProyecto_AsignarIdCorrectamente()
        {
            Usuario adminSistema = new Usuario();
            adminSistema.Id = 1;
            adminSistema.EsAdministradorProyecto = true;
            adminSistema.EstaAdministrandoProyecto = false;
            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario>();
            Proyecto proyecto = new Proyecto("nombre", "descripcion", adminSistema, miembros);

            gestor.CrearProyecto(proyecto, adminSistema);

            Assert.AreEqual(1, proyecto.Id);
            Assert.AreEqual(proyecto, gestor.Proyectos[0]);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CrearProyecto_LanzaExcepcionSiUsuarioNoEsAdminDeProyecto()
        {
            Usuario solicitante = new Usuario();
            solicitante.EsAdministradorProyecto = false;
            solicitante.EstaAdministrandoProyecto = false;

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto A", "Descripción", solicitante, new List<Usuario>());

            gestor.CrearProyecto(proyecto, solicitante);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CrearProyecto_LanzaExcepcionSiSolicitanteYaAdministraOtroProyecto()
        {
            Usuario solicitante = new Usuario();
            solicitante.EsAdministradorProyecto = true;
            solicitante.EstaAdministrandoProyecto = true; // Ya está administrando uno

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto A", "Descripción", solicitante, new List<Usuario>());

            gestor.CrearProyecto(proyecto, solicitante);
        }
        
        [TestMethod]
        public void CrearProyecto_EstableceEstaAdministrandoProyectoEnTrue()
        {
            Usuario solicitante = new Usuario();
            solicitante.EsAdministradorProyecto = true;
            solicitante.EstaAdministrandoProyecto = false;

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario>();
            Proyecto proyecto = new Proyecto("Proyecto X", "Una descripción", solicitante, miembros);

            gestor.CrearProyecto(proyecto, solicitante);

            Assert.IsTrue(solicitante.EstaAdministrandoProyecto);
        }
        
        //TO DO:
        // crearProyecto ( tiene que ser un usuario con el bool EsAdminProyecto en true y estaAdministrandoProyecto en false)
        // si estaAdministrandoProyecto en false y EsAdminProyecto true -> puede crearlo, usuario.estaAdministrandoProyecto cambiar a true
        // verificar que no exista un proyecto con el mismo nombre antes de ser creado (recorro la lista de proyectos)
        // verificar que se asigne bien el id al proyecto
        // manda notificacion a los miembros del proyecto que se creo
        
        // eliminarProyecto (admin de ese proyecto)
        // verificar que el proyecto exista
        // admin.estaAdministrandoProyecto cambiarlo a false
        // manda notificacion a cada uno de los miembros del proyecto
        
        // modificarNombreDelProyecto (admin de ESE proyecto)
        // verificar que el nombre nuevo no lo tenga otro proyecto
        // manda notificacion a cada uno de los miembros del proyecto
        
        // modificarDescripcionDelProyecto (admin proyecto)
        // manda notificacion a cada uno de los miembros del proyecto
        
        // modificarFechaInicioDelProyecto (admin proyecto)
        // manda notificacion a cada uno de los miembros del proyecto
        
        // cambiarAdminProyecto (admin de sistema)
        // verificar nuevoAdmin.estaAdministrandoProyecto en true  -> excepcion (no puede, ya es administrador de otro proyecto)
        // verficar nuevoAdmin.EsAdminProyecto en false -> excepcion (no tiene permiso de crear un proyecto)
        // verificar nuevoAdmin.estaAdministrandoProyecto en false y nuevoAdmin.esAdminProyecto true -> se puede cambiar de admin y nuevoAdmin.estaAdministrandoProyecto cambiar a true
        // exAdmin.estaAdministrandoProyecto cambiarlo a false 
        // manda notificacion a cada uno de los miembros del proyecto
        
        // agregarMiembroAProyecto (admin de proyecto)
        // manda notificacion a cada uno de los miembros del proyecto 
        
        // eliminarMiembroDelProyecto (admin de proyecto)
        // manda notificacion a cada uno de los miembros del proyecto 
        
        // agregarTareaDelProyecto (admin de proyecto) seria crearTarea (cuando se crea se agrega a la lista de tareas)
        // manda notificacion a cada uno de los miembros del proyecto 
        
        // eliminarTareaDelProyecto (admin de proyecto) seria eliminarTarea
        // manda notificacion a cada uno de los miembros del proyecto
        
        // obtenerTodosLosProyectos (metodo privado (?))
        
        // obtenerProyectoPorUsuario (metodo privado (?))
        
    }
}