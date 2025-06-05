using Dominio;
using DTOs;
using Repositorios;
using Servicios.Excepciones;
using Servicios.Gestores;

namespace Tests.ServiciosTests

{
    [TestClass]
    public class GestorProyectosTests
    {
        private RepositorioUsuarios _repositorioUsuarios;
        private RepositorioProyectos _repositorioProyectos;
        private GestorProyectos _gestor;
        private Usuario _admin;
        private Usuario _usuarioNoAdmin;
        private UsuarioDTO _adminDTO;
        private UsuarioDTO _adminSistema;
        private ProyectoDTO _proyecto;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorioUsuarios = new RepositorioUsuarios();
            _repositorioProyectos = new RepositorioProyectos();
            _gestor = new GestorProyectos(_repositorioUsuarios, _repositorioProyectos);
            _admin = CrearAdminProyecto(1);
            _adminDTO = UsuarioDTO.DesdeEntidad(_admin);
            _adminSistema = CrearAdminSistema(2);
            _usuarioNoAdmin = CrearMiembro(3);
            _proyecto = CrearProyectoCon(_admin);
        }
        private Usuario CrearAdminProyecto(int id)
        {
            Usuario admin = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            admin.EsAdministradorProyecto = true;
            admin.Id = id; // se hardcodea en tests por simplicidad
            return admin;
        }

        private UsuarioDTO CrearAdminSistema(int id)
        {
            Usuario adminSistema = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            adminSistema.EsAdministradorSistema = true;
            adminSistema.Id = id; // se hardcodea en tests pero en realidad el que gestiona ids es el gestor de usuarios
            return UsuarioDTO.DesdeEntidad(adminSistema);
        }

        private Usuario CrearMiembro(int id)
        {
            Usuario miembro = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            miembro.Id = id; // se hardcodea en tests pero en realidad el que gestiona ids es el gestor de usuarios
            return miembro;
        }

        private ProyectoDTO CrearProyectoCon(Usuario admin)
        {
            return new ProyectoDTO()
            {
                Nombre = "Proyecto",
                Descripcion = "Descripcion",
                FechaInicio = DateTime.Today.AddDays(1),
                Administrador = UsuarioDTO.DesdeEntidad(admin)
            };
        }

        private TareaDTO CrearTarea(int id = 1, DateTime? inicio = null, DateTime? fin = null)
        {
            string titulo = "Tarea";
            string descripcion = "Prueba de tarea";
            int duracionEnDias = 8;
            DateTime fechaInicio = new DateTime(2500, 9, 1);
        
            TareaDTO tarea = new TareaDTO(){Titulo = titulo, Descripcion = descripcion, DuracionEnDias = duracionEnDias, FechaInicioMasTemprana = fechaInicio};
            return tarea;
        }
        
        //crearProyecto
        [TestMethod]
        public void CrearProyecto_AsignarIdCorrectamente()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _adminDTO);

            Assert.AreEqual(1, proyecto.Id);
            Assert.AreEqual(proyecto.Id, _gestor.ObtenerTodos().ElementAt(0).Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CrearProyecto_LanzaExcepcionSiUsuarioNoTienePermisosDeAdminProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_usuarioNoAdmin);

            _gestor.CrearProyecto(proyecto, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CrearProyecto_LanzaExcepcionSiSolicitanteYaAdministraOtroProyecto()
        {
            _admin.EstaAdministrandoUnProyecto = true;
    
            ProyectoDTO proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _adminDTO);
        }

        [TestMethod]
        public void CrearProyecto_EstableceEstaAdministrandoProyectoEnTrue()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _adminDTO);

            Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CrearProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            ProyectoDTO proyecto1 = CrearProyectoCon(_admin);
            ProyectoDTO proyecto2 = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto1, _adminDTO);
            _gestor.CrearProyecto(proyecto2, _adminDTO);
        }

        [TestMethod]
        public void CrearProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = miembros.Select(UsuarioListarDTO.DesdeEntidad).ToList();

            _gestor.CrearProyecto(proyecto, _adminDTO);

            foreach (Usuario miembro in miembros)
            {
                Assert.AreEqual(1, miembro.Notificaciones.Count);
                Assert.AreEqual("Se creó el proyecto 'Proyecto'.", miembro.Notificaciones[0].Mensaje);
            }
        }

        // eliminarProyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarProyecto_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.EliminarProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.EliminarProyecto(1000, _adminDTO);
        }

        [TestMethod]
        public void EliminarProyecto_EliminaDeListaAlProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _adminDTO);

            Assert.AreEqual(1, _gestor.ObtenerTodos().Count);

            _gestor.EliminarProyecto(_proyecto.Id, _adminDTO);

            Assert.AreEqual(0, _gestor.ObtenerTodos().Count);
        }

        [TestMethod]
        public void EliminarProyecto_CambiaLaFLagEstaAdministrandoProyectoDelAdministrador()
        {
            _gestor.CrearProyecto(_proyecto, _adminDTO);

            Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);

            _gestor.EliminarProyecto(_proyecto.Id, _adminDTO);

            Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        public void EliminarProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            ProyectoDTO proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _adminDTO);
            _gestor.EliminarProyecto(proyecto.Id, _adminDTO);

            foreach (UsuarioListarDTO miembro in proyecto.Miembros)
            {
                Assert.AreEqual(2, miembro.Notificaciones.Count);
                Assert.AreEqual("Se eliminó el proyecto 'Proyecto'.", miembro.Notificaciones[1].Mensaje);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        public void ModificarNombreDelProyecto_ModificaNombreDelProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _adminDTO);

            _gestor.ModificarNombreDelProyecto(_proyecto.Id, "Nuevo Nombre", _adminDTO);

            Assert.AreEqual("Nuevo Nombre", _proyecto.Nombre);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            ProyectoDTO proyecto1 = CrearProyectoCon(_admin);
            ProyectoDTO proyecto2 = CrearProyectoCon(CrearAdminProyecto(4));
            proyecto2.Nombre = "Otro Nombre";

            _gestor.CrearProyecto(proyecto1, _adminDTO);
            _gestor.CrearProyecto(proyecto2, proyecto2.Administrador);

            _gestor.ModificarNombreDelProyecto(proyecto2.Id, proyecto1.Nombre, proyecto2.Administrador);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarNombreDelProyecto(1000, "nuevo", _adminDTO);
        }


        [TestMethod]
        public void ModificarNombreDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = miembros.Select(UsuarioListarDTO.DesdeEntidad).ToList();

            _gestor.CrearProyecto(proyecto, _adminDTO);
            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", _adminDTO);

            string mensajeEsperado = "Se cambió el nombre del proyecto 'Proyecto' a 'Nuevo Nombre'.";

            foreach (UsuarioListarDTO usuario in proyecto.Miembros)
            {
                Assert.AreEqual(2, usuario.Notificaciones.Count);
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones[1].Mensaje);
            }
        }


        // modificacion de la descripcion del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarDescripcionDelProyecto(1000, "Nueva descripcion", _adminDTO);

        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_ModificaDescripcionDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva descripcion", _adminDTO);

            Assert.AreEqual("Nueva descripcion", proyecto.Descripcion);
        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            string nuevaDescripcion = "Nueva descripcion";
            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, nuevaDescripcion, _adminDTO);

            string mensajeEsperado = $"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{nuevaDescripcion}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }

        // modificacion de la fecha de inicio del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(1000, nuevaFecha, _adminDTO);
        }

        [TestMethod]

        public void ModificarFechaDeInicioDelProyecto_ModificaFechaDeInicioDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _adminDTO);

            Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
        }

        [TestMethod]
        public void ModificarFechaDeInicioDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _adminDTO);

            string mensajeEsperado = $"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }
        
        [TestMethod]
        public void CambiarAdministradorDeProyecto_AsignaNuevoAdminOK()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(4);

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

            Assert.AreEqual(nuevoAdmin.Id, proyecto.Administrador.Id);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_DesactivaFlagDelAdminAnterior()
        {
            Usuario adminNuevo = CrearAdminProyecto(4);

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(adminNuevo) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_ActivaFlagDelAdminNuevo()
        {
            Usuario adminNuevo = CrearAdminProyecto(4);

            ProyectoDTO proyecto = CrearProyectoCon(_admin); 
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(adminNuevo) };
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsTrue(adminNuevo.EstaAdministrandoUnProyecto);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiSolicitanteNoEsAdminSistema()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminDTO, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.CambiarAdministradorDeProyecto(_adminSistema, 1000, 1);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiNuevoAdminNoEsMiembro()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionNuevoAdminYaAdministraOtroProyecto()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(2);
            nuevoAdmin.EstaAdministrandoUnProyecto = true;

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(nuevoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcion_NuevoAdminNoTienePermisosDeAdminProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_NotificaALosMiembros()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(4);

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(nuevoAdmin), UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

            string msg = $"Se cambió el administrador del proyecto '{proyecto.Nombre}'. El nuevo administrador es '{nuevoAdmin}'.";

            foreach (var u in new[] { nuevoAdmin, _usuarioNoAdmin })
            {
                Assert.AreEqual(2, u.Notificaciones.Count);
                Assert.AreEqual(msg, u.Notificaciones[1].Mensaje);
            }
        }

        //agregar miembro al proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarMiembro_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.AgregarMiembroAProyecto(1000, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarMiembro_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _adminDTO);
            
            UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro(4));

            _gestor.AgregarMiembroAProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin), nuevo);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void AgregarMiembro_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            UsuarioDTO solicitante = UsuarioDTO.DesdeEntidad(CrearAdminProyecto(4));
            UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro(5));
            ProyectoDTO proyecto = CrearProyectoCon(_admin); 

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, nuevo);
        }

        [TestMethod]
        public void AgregarMiembro_AgregaElMiembroALaListaOK()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro(4));

            _gestor.CrearProyecto(proyecto, _adminDTO);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, nuevo);

            Assert.IsTrue(proyecto.Miembros.Any(u => u.Id == nuevo.Id));
        }

        [TestMethod]
        public void AgregarMiembro_NotificaALosMiembros()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _adminDTO);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

            string esperado = $"Se agregó a un nuevo miembro (id {_usuarioNoAdmin.Id}) al proyecto '{proyecto.Nombre}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
            }
            Assert.IsTrue(proyecto.Administrador.Notificaciones.Any(n => n.Mensaje == esperado));
        }

        //eliminar miembro del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarMiembroDelProyecto_ProyectoNoExiste_LanzaExcepcion()
        {
            _gestor.EliminarMiembroDelProyecto(1000, _adminDTO, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };
            
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin), _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            UsuarioDTO solicitante = UsuarioDTO.DesdeEntidad(CrearAdminProyecto(4));
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, solicitante, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiUsuarioNoEsMiembroDelProyecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin); 
            _gestor.CrearProyecto(proyecto, _adminDTO);
            
            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void EliminarMiembroConTareaAsignada_LanzaExcepcion()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            TareaDTO tarea = CrearTarea(1);
            tarea.UsuariosAsignados.Add(UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin));
            
            proyecto.Tareas.Add(tarea);
            
            _gestor.CrearProyecto(proyecto, _adminDTO);
            
            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_EliminaMiembroOK()
        {
            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO>() { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);

            Assert.IsFalse(proyecto.Miembros.Any(u => u.Id == _usuarioNoAdmin.Id));
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_NotificaALosMiembros()
        {
            UsuarioListarDTO miembro1 = UsuarioListarDTO.DesdeEntidad(CrearMiembro(4));
            UsuarioListarDTO miembro2 = UsuarioListarDTO.DesdeEntidad(CrearMiembro(5));

            ProyectoDTO proyecto = CrearProyectoCon(_admin);
            proyecto.Miembros = new List<UsuarioListarDTO> { miembro1, miembro2 };
                
            _gestor.CrearProyecto(proyecto, _adminDTO);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, miembro1.Id);

            string esperado = $"Se eliminó a el miembro (id {miembro1.Id}) del proyecto '{proyecto.Nombre}'.";

            Assert.IsTrue(_adminDTO.Notificaciones.Any(n => n.Mensaje == esperado));
            Assert.IsTrue(miembro2.Notificaciones.Any(n => n.Mensaje == esperado));
        }
        
        [TestMethod]
        public void ObtenerProyectosPorUsuario_DevuelveProyectosDelMiembro()
        {
            Usuario admin1 = CrearAdminProyecto(4);
            Usuario admin2 = CrearAdminProyecto(5);
            UsuarioListarDTO miembro1 = UsuarioListarDTO.DesdeEntidad(CrearMiembro(6));
            UsuarioListarDTO miembro2 = UsuarioListarDTO.DesdeEntidad(CrearMiembro(7));

            ProyectoDTO proyecto1 = CrearProyectoCon(admin1);
            proyecto1.Miembros = new List<UsuarioListarDTO> { miembro1, miembro2 };
            
            _gestor.CrearProyecto(proyecto1, UsuarioDTO.DesdeEntidad(admin1));
            proyecto1.Nombre = "Proyecto 1";

            ProyectoDTO proyecto2 = CrearProyectoCon(admin2);
            proyecto2.Miembros = new List<UsuarioListarDTO> { miembro1 };
            
            _gestor.CrearProyecto(proyecto2, UsuarioDTO.DesdeEntidad(admin2));
            proyecto2.Nombre = "Proyecto 2";

            List<ProyectoDTO> proyectosDeMiembro1 = _gestor.ObtenerProyectosPorUsuario(miembro1.Id);
            List<ProyectoDTO> proyectosDeMiembro2 = _gestor.ObtenerProyectosPorUsuario(miembro2.Id);

            Assert.AreEqual(2, proyectosDeMiembro1.Count);
            Assert.IsTrue(proyectosDeMiembro1.Any(p => p.Id == proyecto1.Id));
            Assert.IsTrue(proyectosDeMiembro1.Any(p => p.Id == proyecto2.Id));

            Assert.AreEqual(1, proyectosDeMiembro2.Count);
            Assert.AreEqual(proyecto1.Id, proyectosDeMiembro2[0].Id);
        }
        
        [TestMethod]
        public void ObtenerProyectoDelAdministrador_DevuelveProyectoCorrecto()
        {
            ProyectoDTO proyecto = CrearProyectoCon( _admin);
            _gestor.CrearProyecto(proyecto, _adminDTO);

            Proyecto resultado = _gestor.ObtenerProyectoDelAdministrador(_admin.Id);

            Assert.AreEqual(proyecto.Id, resultado.Id);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionServicios))]
        public void ObtenerProyectoDelAdministrador_LanzaExcepcionSiNoExisteProyectoConEseAdmin()
        {
            _gestor.ObtenerProyectoDelAdministrador(_admin.Id);
        }

        [TestMethod]
        public void NotificarAdministradoresDeProyectos_NotificaAdministradores()
        {
            Proyecto proyecto1 =new Proyecto("Proyecto 1", "Descripción 1", DateTime.Today.AddDays(1), _admin, new List<Usuario>());
            Usuario otroAdmin = CrearAdminProyecto(3);
            Proyecto proyecto2 = new Proyecto("Proyecto 2", "Descripción 2", DateTime.Today.AddDays(2), otroAdmin, new List<Usuario>());
            
            List<Proyecto> proyectos = new List<Proyecto> { proyecto1, proyecto2 };
            _gestor.NotificarAdministradoresDeProyectos(proyectos, "notificación");

            Notificacion ultimaNotificacionAdmin = _admin.Notificaciones.Last();
            Notificacion ultimaNotificacionOtroAdmin = otroAdmin.Notificaciones.Last();
            Assert.AreEqual("notificación", ultimaNotificacionAdmin.Mensaje);
            Assert.AreEqual("notificación", ultimaNotificacionOtroAdmin.Mensaje);
        }

    }
}