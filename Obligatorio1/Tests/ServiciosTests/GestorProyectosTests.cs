using Dominio;
using DTOS_;
using Servicios.Excepciones;
using Servicios.Gestores;
using Servicios.Notificaciones;

namespace Tests.ServiciosTests

{
    [TestClass]
    public class GestorProyectosTests
    {
        
        private GestorProyectos _gestor;
        private Usuario _admin;
        private Usuario _usuarioNoAdmin;
        private Usuario _adminSistema;
        private Proyecto _proyecto;
        private MockNotificador _mockNotificador;

        [TestInitialize]
        public void Inicializar()
        {
            _mockNotificador = new MockNotificador();
            _gestor = new GestorProyectos(_mockNotificador);
            _admin = CrearAdminProyecto(1);
            _adminSistema = CrearAdminSistema(2);
            _usuarioNoAdmin = CrearMiembro(3);
            _proyecto = CrearProyectoCon(_admin);
        }
        
        private ProyectoDTO CrearDTOProyecto(string nombre, DateTime? fechaInicio = null)
        {
            return new ProyectoDTO
            {
                Nombre = nombre,
                Descripcion = "Descripción de prueba",
                FechaInicio = fechaInicio ?? DateTime.Today.AddDays(1)
            };
        }
        
        private Usuario CrearAdminProyecto(int id)
        {
            Usuario admin = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            admin.EsAdministradorProyecto = true;
            admin.Id = id; // se hardcodea en tests pero en realidad el que gestiona ids es el gestor de usuarios
            return admin;
        }

        private Usuario CrearAdminSistema(int id)
        {
            Usuario adminSistema = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            adminSistema.EsAdministradorSistema = true;
            adminSistema.Id = id; // se hardcodea en tests pero en realidad el que gestiona ids es el gestor de usuarios
            return adminSistema;
        }

        private Usuario CrearMiembro(int id)
        {
            Usuario miembro = new Usuario("Juan", "Perez", new DateTime(1999,2,2), "unemail@gmail.com", "Contrase#a3");
            miembro.Id = id; // se hardcodea en tests pero en realidad el que gestiona ids es el gestor de usuarios
            return miembro;
        }

        private Proyecto CrearProyectoCon(Usuario admin, List<Usuario> miembros = null)
        {
            miembros ??= new List<Usuario>();
            DateTime fechaInicio = DateTime.Today.AddDays(1);
            return new Proyecto("Proyecto", "Descripcion",fechaInicio, admin, miembros);
        }

        private Tarea CrearTarea(int id = 1, DateTime? inicio = null, DateTime? fin = null)
        {
            string titulo = "Tarea";
            string descripcion = "Prueba de tarea";
            int duracionEnDias = 8;
            DateTime fechaInicio = new DateTime(2500, 9, 1);
        
            Tarea tarea = new Tarea(titulo, descripcion, duracionEnDias, fechaInicio);
            return tarea;
        }
        
        //crearProyecto
        [TestMethod]
        public void CrearProyecto_AsignarIdCorrectamente()
        {
            
            Proyecto proyecto = CrearProyectoCon(_admin);
    
            _gestor.CrearProyecto(proyecto, _admin);

            Assert.AreEqual(1, proyecto.Id);
            Assert.AreEqual(proyecto, _gestor.Proyectos.ObtenerTodos().ElementAt(0));
            
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CrearProyecto_LanzaExcepcionSiUsuarioNoTienePermisosDeAdminProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_usuarioNoAdmin);

            _gestor.CrearProyecto(proyecto, _usuarioNoAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CrearProyecto_LanzaExcepcionSiSolicitanteYaAdministraOtroProyecto()
        {
            _admin.EstaAdministrandoUnProyecto = true;
    
            Proyecto proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _admin);
        }

        [TestMethod]
        public void CrearProyecto_EstableceEstaAdministrandoProyectoEnTrue()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _admin);

            Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CrearProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            Proyecto proyecto1 = CrearProyectoCon(_admin);
            Proyecto proyecto2 = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto1, _admin);
            _gestor.CrearProyecto(proyecto2, _admin);
        }

        [TestMethod]
        public void CrearProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            Proyecto proyecto = CrearProyectoCon(_admin, miembros);

            _gestor.CrearProyecto(proyecto, _admin);

            foreach (Usuario miembro in miembros)
            {
                Assert.AreEqual(1, miembro.Notificaciones.Count);
                Assert.AreEqual(MensajesNotificacion.ProyectoCreado(proyecto.Nombre), miembro.Notificaciones[0].Mensaje);
            }
        }

        // eliminarProyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void EliminarProyecto_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarProyecto(proyecto.Id, _usuarioNoAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void EliminarProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.EliminarProyecto(1000, _admin);
        }

        [TestMethod]
        public void EliminarProyecto_EliminaDeListaAlProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            Assert.AreEqual(1, _gestor.Proyectos.ObtenerTodos().Count);

            _gestor.EliminarProyecto(_proyecto.Id, _admin);

            Assert.AreEqual(0, _gestor.Proyectos.ObtenerTodos().Count);
        }

        [TestMethod]
        public void EliminarProyecto_CambiaLaFLagEstaAdministrandoProyectoDelAdministrador()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);

            _gestor.EliminarProyecto(_proyecto.Id, _admin);

            Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        public void EliminarProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            Proyecto proyecto = CrearProyectoCon(_admin, miembros);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.EliminarProyecto(proyecto.Id, _admin);

            foreach (Usuario miembro in proyecto.Miembros)
            {
                Assert.AreEqual(2, miembro.Notificaciones.Count);
                Assert.AreEqual(MensajesNotificacion.ProyectoEliminado(proyecto.Nombre), miembro.Notificaciones[1].Mensaje);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", _usuarioNoAdmin);
        }

        [TestMethod]
        public void ModificarNombreDelProyecto_ModificaNombreDelProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            _gestor.ModificarNombreDelProyecto(_proyecto.Id, "Nuevo Nombre", _admin);

            Assert.AreEqual("Nuevo Nombre", _proyecto.Nombre);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            Proyecto proyecto1 = CrearProyectoCon(_admin);
            Proyecto proyecto2 = CrearProyectoCon(CrearAdminProyecto(4));
            proyecto2.ModificarNombre("Otro Nombre");

            _gestor.CrearProyecto(proyecto1, _admin);
            _gestor.CrearProyecto(proyecto2, proyecto2.Administrador);

            _gestor.ModificarNombreDelProyecto(proyecto2.Id, proyecto1.Nombre, proyecto2.Administrador);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarNombreDelProyecto(1000, "nuevo", _admin);
        }


        [TestMethod]
        public void ModificarNombreDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);
            List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };
            Proyecto proyecto = CrearProyectoCon(_admin, miembros);
            string anterior = proyecto.Nombre;

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", _admin);

            string mensajeEsperado = MensajesNotificacion.NombreProyectoModificado(anterior, proyecto.Nombre);

            foreach (Usuario usuario in proyecto.Miembros)
            {
                Assert.AreEqual(2, usuario.Notificaciones.Count);
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones[1].Mensaje);
            }
        }


        // modificacion de la descripcion del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", _usuarioNoAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarDescripcionDelProyecto(1000, "Nueva descripcion", _admin);

        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_ModificaDescripcionDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva descripcion", _admin);

            Assert.AreEqual("Nueva descripcion", proyecto.Descripcion);
        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            string nuevaDescripcion = "Nueva descripcion";
            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, nuevaDescripcion, _admin);

            string mensajeEsperado = MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, nuevaDescripcion);

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }

        // modificacion de la fecha de inicio del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _usuarioNoAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(1000, nuevaFecha, _admin);
        }

        [TestMethod]

        public void ModificarFechaDeInicioDelProyecto_ModificaFechaDeInicioDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _admin);

            Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
        }

        [TestMethod]
        public void ModificarFechaDeInicioDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _admin);

            string mensajeEsperado = MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha);

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }
        
        [TestMethod]
        public void CambiarAdministradorDeProyecto_AsignaNuevoAdminOK()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(4);

            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { nuevoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

            Assert.AreSame(nuevoAdmin, proyecto.Administrador);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_DesactivaFlagDelAdminAnterior()
        {
            Usuario adminNuevo = CrearAdminProyecto(4);

            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { adminNuevo });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_ActivaFlagDelAdminNuevo()
        {
            Usuario adminNuevo = CrearAdminProyecto(4);

            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { adminNuevo });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsTrue(adminNuevo.EstaAdministrandoUnProyecto);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiSolicitanteNoEsAdminSistema()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_admin, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.CambiarAdministradorDeProyecto(_adminSistema, 1000, 1);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiNuevoAdminNoEsMiembro()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionNuevoAdminYaAdministraOtroProyecto()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(2);
            nuevoAdmin.EstaAdministrandoUnProyecto = true;

            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { nuevoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcion_NuevoAdminNoTienePermisosDeAdminProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_NotificaALosMiembros()
        {
            Usuario nuevoAdmin = CrearAdminProyecto(4);

            Proyecto proyecto = CrearProyectoCon(_admin, new() { nuevoAdmin, _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

            string msg = MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre, nuevoAdmin.ToString());

            foreach (var u in new[] { nuevoAdmin, _usuarioNoAdmin })
            {
                Assert.AreEqual(2, u.Notificaciones.Count);
                Assert.AreEqual(msg, u.Notificaciones[1].Mensaje);
            }
        }

        //agregar miembro al proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void AgregarMiembro_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.AgregarMiembroAProyecto(1000, _admin, _usuarioNoAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void AgregarMiembro_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);
            
            Usuario nuevo = CrearMiembro(4);

            _gestor.AgregarMiembroAProyecto(proyecto.Id, _usuarioNoAdmin, nuevo);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void AgregarMiembro_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            Usuario solicitante = CrearAdminProyecto(4);
            Usuario nuevo = CrearMiembro(5);
            Proyecto proyecto = CrearProyectoCon(_admin); 

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, nuevo);
        }

        [TestMethod]
        public void AgregarMiembro_AgregaElMiembroALaListaOK()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            Usuario nuevo = CrearMiembro(4);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _admin, nuevo);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevo));
        }

        [TestMethod]
        public void AgregarMiembro_NotificaALosMiembros()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _admin, _usuarioNoAdmin);

            string esperado = MensajesNotificacion.MiembroAgregado(proyecto.Nombre, _usuarioNoAdmin.Id);
            
            foreach (var usuario in proyecto.Miembros.Concat(new[] { proyecto.Administrador }))
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
            }
        }

        //eliminar miembro del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void EliminarMiembroDelProyecto_ProyectoNoExiste_LanzaExcepcion()
        {
            _gestor.EliminarMiembroDelProyecto(1000, _admin, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario>() { _usuarioNoAdmin });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _usuarioNoAdmin, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            Usuario solicitante = CrearAdminProyecto(4);
            Proyecto proyecto = CrearProyectoCon(_admin, new() { _usuarioNoAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, solicitante, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiUsuarioNoEsMiembroDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin); 
            _gestor.CrearProyecto(proyecto, _admin);
            
            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void EliminarMiembroConTareaAsignada_LanzaExcepcion()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            proyecto.AsignarMiembro(_usuarioNoAdmin);
            
            Tarea tarea = CrearTarea(1);
            tarea.AsignarUsuario(_usuarioNoAdmin);
            
            proyecto.AgregarTarea(tarea);
            
            _gestor.CrearProyecto(proyecto, _admin);
            
            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, _usuarioNoAdmin.Id);
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_EliminaMiembroOK()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new() { _usuarioNoAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, _usuarioNoAdmin.Id);

            Assert.IsFalse(proyecto.Miembros.Contains(_usuarioNoAdmin));
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_NotificaALosMiembros()
        {
            Usuario miembro1 = CrearMiembro(4);
            Usuario miembro2 = CrearMiembro(5);

            Proyecto proyecto = CrearProyectoCon(_admin, new() { miembro1, miembro2 });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, miembro1.Id);

            string esperado = MensajesNotificacion.MiembroEliminado(proyecto.Nombre, miembro1.Id);

            foreach (var usuario in new[] { _admin, miembro2 })
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
            }
        }
        
        [TestMethod]
        public void ObtenerProyectosPorUsuario_DevuelveProyectosDelMiembro()
        {
            Usuario admin1 = CrearAdminProyecto(4);
            Usuario admin2 = CrearAdminProyecto(5);
            Usuario miembro1 = CrearMiembro(6);
            Usuario miembro2 = CrearMiembro(7);

            Proyecto proyecto1 = CrearProyectoCon(admin1, new List<Usuario> { miembro1, miembro2 });
            _gestor.CrearProyecto(proyecto1, admin1);
            
            proyecto1.ModificarNombre("Proyecto 1");

            Proyecto proyecto2 = CrearProyectoCon(admin2, new List<Usuario> { miembro1 });
            _gestor.CrearProyecto(proyecto2, admin2);

            proyecto2.ModificarNombre("Proyecto 2");

            List<Proyecto> proyectosDeMiembro1 = _gestor.ObtenerProyectosPorUsuario(miembro1.Id);
            List<Proyecto> proyectosDeMiembro2 = _gestor.ObtenerProyectosPorUsuario(miembro2.Id);

            Assert.AreEqual(2, proyectosDeMiembro1.Count);
            CollectionAssert.Contains(proyectosDeMiembro1, proyecto1);
            CollectionAssert.Contains(proyectosDeMiembro1, proyecto2);

            Assert.AreEqual(1, proyectosDeMiembro2.Count);
            Assert.AreEqual(proyecto1, proyectosDeMiembro2[0]);
        }
        
        [TestMethod]
        public void ObtenerProyectoDelAdministrador_DevuelveProyectoCorrecto()
        {
            Proyecto proyecto = CrearProyectoCon( _admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Proyecto resultado = _gestor.ObtenerProyectoDelAdministrador(_admin.Id);

            Assert.AreEqual(proyecto, resultado);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionProyecto))]
        public void ObtenerProyectoDelAdministrador_LanzaExcepcionSiNoExisteProyectoConEseAdmin()
        {
            _gestor.ObtenerProyectoDelAdministrador(_admin.Id);
        }

        [TestMethod]
        public void NotificarAdministradoresDeProyectos_NotificaAdministradores()
        {
            Proyecto proyecto1 = CrearProyectoCon(_admin);
            Usuario otroAdmin = CrearAdminProyecto(3);
            Proyecto proyecto2 = CrearProyectoCon(otroAdmin);
            
            List<Proyecto> proyectos = new List<Proyecto> { proyecto1, proyecto2 };
            string mensajeEsperado = "notificación";
            _gestor.NotificarAdministradoresDeProyectos(proyectos, "notificación");

            Notificacion ultimaNotificacionAdmin = _admin.Notificaciones.Last();
            Notificacion ultimaNotificacionOtroAdmin = otroAdmin.Notificaciones.Last();
            Assert.IsTrue(_mockNotificador.Notificaciones.Any(n =>
                    n.Item1 == _admin && n.Item2 == mensajeEsperado));

            Assert.IsTrue(_mockNotificador.Notificaciones.Any(n =>
                    n.Item1 == otroAdmin && n.Item2 == mensajeEsperado));
        }
        
        [TestMethod]
        public void ObtenerTodosDTO_DevuelveListaDeProyectosDTO()
        {
            Usuario admin1 = CrearAdminProyecto(4);
            Usuario admin2 = CrearAdminProyecto(5);

            Proyecto proyecto1 = CrearProyectoCon(admin1);
            proyecto1.ModificarNombre("Proyecto 1");

            Proyecto proyecto2 = CrearProyectoCon(admin2);
            proyecto2.ModificarNombre("Proyecto 2");

            _gestor.CrearProyecto(proyecto1, admin1);
            _gestor.CrearProyecto(proyecto2, admin2);
            
            List<ProyectoDTO> proyectosDTO = _gestor.ObtenerTodosDTO();
            
            Assert.AreEqual(2, proyectosDTO.Count);
            Assert.IsTrue(proyectosDTO.Any(p => p.Nombre == "Proyecto 1"));
            Assert.IsTrue(proyectosDTO.Any(p => p.Nombre == "Proyecto 2"));
        }
        
        [TestMethod]
        public void ObtenerTodosDTO_CopiaIdCorrectamente()
        {
            Usuario admin = CrearAdminProyecto(10);//
            Proyecto proyecto = CrearProyectoCon(admin);
            proyecto.ModificarNombre("Proyecto A");

            _gestor.CrearProyecto(proyecto, admin);

            var dto = _gestor.ObtenerTodosDTO().FirstOrDefault();

            Assert.IsNotNull(dto);
            Assert.AreEqual(proyecto.Id, dto.Id);
        }
        
        [TestMethod]
        public void ObtenerProyectosPorUsuarioDTO_DevuelveProyectosCorrectos()
        {
            Usuario admin1 = CrearAdminProyecto(4);
            Usuario admin2 = CrearAdminProyecto(5);
            Usuario miembro = CrearMiembro(6);

            Proyecto proyecto1 = CrearProyectoCon(admin1, new List<Usuario> { miembro });
            proyecto1.ModificarNombre("Proyecto 1");
            _gestor.CrearProyecto(proyecto1, admin1);

            Proyecto proyecto2 = CrearProyectoCon(admin2); // no incluye al miembro
            proyecto2.ModificarNombre("Proyecto 2");
            _gestor.CrearProyecto(proyecto2, admin2);
            
            var resultado = _gestor.ObtenerProyectosPorUsuarioDTO(miembro.Id);
            
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(proyecto1.Nombre, resultado[0].Nombre);
            Assert.AreEqual(proyecto1.Id, resultado[0].Id);
        }
        
        
        [TestMethod]
        public void CrearProyectoDesdeDTO_CreaProyectoCorrectamente()
        {
            ProyectoDTO dto = CrearDTOProyecto("Proyecto A");

            UsuarioDTO solicitanteDTO = UsuarioDTO.DesdeEntidad(_admin);
            
            _gestor.CrearProyectoDesdeDTO(dto, solicitanteDTO);
            
            List<ProyectoDTO> proyectos = _gestor.ObtenerTodosDTO();
            Assert.AreEqual(1, proyectos.Count);
            Assert.AreEqual(dto.Nombre, proyectos[0].Nombre);
            Assert.AreEqual(dto.Descripcion, proyectos[0].Descripcion);
            Assert.AreEqual(dto.FechaInicio, proyectos[0].FechaInicio);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void CrearProyectoDesdeDTO_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
        {
            ProyectoDTO dto = CrearDTOProyecto("Proyecto A");

            Usuario noAdmin = CrearMiembro(1);
            UsuarioDTO dtoSolicitante = UsuarioDTO.DesdeEntidad(noAdmin);
            
            _gestor.CrearProyectoDesdeDTO(dto, dtoSolicitante);
        }


    }
    
}