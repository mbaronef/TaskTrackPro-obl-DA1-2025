using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;
using NuGet.Frameworks;

namespace Tests

{
    [TestClass]
    public class GestorProyectosTests
    {
        
        private GestorProyectos _gestor;
        private Usuario _admin;
        private Usuario _miembro;
        private Usuario _adminSistema;
        private Proyecto _proyecto;

        [TestInitialize]
        public void Inicializar()
        {
            _gestor = new GestorProyectos();
            _admin = CrearAdminProyecto();
            _adminSistema = CrearAdminSistema();
            _miembro = CrearMiembro();
            _proyecto = CrearProyectoCon(_admin);
        }

        private Usuario CrearAdminProyecto(int id = 1)
        {
            Usuario admin = new Usuario { Id = id, EsAdministradorProyecto = true };
            return admin;
        }

        private Usuario CrearAdminSistema(int id = 99)
        {
            Usuario adminSistema = new Usuario { Id = id, EsAdministradorSistema = true };
            return adminSistema;
        }

        private Usuario CrearMiembro(int id = 2)
        {
            Usuario miembro = new Usuario { Id = id };
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
            Tarea tarea = new Tarea
            {
                Id = id,
                FechaInicio = inicio ?? DateTime.Today,
                FechaFinMasTemprana = fin ?? DateTime.Today.AddDays(1)
            };
            return tarea;
        }
        
        //crearProyecto
        [TestMethod]
        public void CrearProyecto_AsignarIdCorrectamente()
        {
            
            Usuario admin = CrearAdminProyecto();
            Proyecto proyecto = CrearProyectoCon(admin);
    
            _gestor.CrearProyecto(proyecto, admin);

            Assert.AreEqual(1, proyecto.Id);
            Assert.AreEqual(proyecto, _gestor.Proyectos[0]);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CrearProyecto_LanzaExcepcionSiUsuarioNoTienePermisosDeAdminProyecto()
        {
            Usuario solicitante = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(solicitante);

            _gestor.CrearProyecto(proyecto, solicitante);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CrearProyecto_LanzaExcepcionSiSolicitanteYaAdministraOtroProyecto()
        {
            Usuario solicitante = CrearAdminProyecto();
            solicitante.EstaAdministrandoProyecto = true;
    
            Proyecto proyecto = CrearProyectoCon(solicitante);

            _gestor.CrearProyecto(proyecto, solicitante);
        }

        [TestMethod]
        public void CrearProyecto_EstableceEstaAdministrandoProyectoEnTrue()
        {
            Usuario solicitante = CrearAdminProyecto();

            Proyecto proyecto = CrearProyectoCon(solicitante);

            _gestor.CrearProyecto(proyecto, solicitante);

            Assert.IsTrue(solicitante.EstaAdministrandoProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
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
            Usuario miembro1 = CrearMiembro(2);
            Usuario miembro2 = CrearMiembro(3);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            Proyecto proyecto = CrearProyectoCon(_admin, miembros);

            _gestor.CrearProyecto(proyecto, _admin);

            foreach (Usuario miembro in miembros)
            {
                Assert.AreEqual(1, miembro.Notificaciones.Count);
                Assert.AreEqual("Se creó el proyecto 'Proyecto'.", miembro.Notificaciones[0].Mensaje);
            }
        }

        // eliminarProyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarProyecto_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
        {
            Usuario noAdmin = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { noAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarProyecto(proyecto.Id, noAdmin);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.EliminarProyecto(1000, _admin);
        }

        [TestMethod]
        public void EliminarProyecto_EliminaDeListaAlProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            Assert.AreEqual(1, _gestor.Proyectos.Count);

            _gestor.EliminarProyecto(_proyecto.Id, _admin);

            Assert.AreEqual(0, _gestor.Proyectos.Count);
        }

        [TestMethod]
        public void EliminarProyecto_CambiaLaFLagEstaAdministrandoProyectoDelAdministrador()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            Assert.IsTrue(_admin.EstaAdministrandoProyecto);

            _gestor.EliminarProyecto(_proyecto.Id, _admin);

            Assert.IsFalse(_admin.EstaAdministrandoProyecto);
        }

        [TestMethod]
        public void EliminarProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(2);
            Usuario miembro2 = CrearMiembro(3);
            List<Usuario> miembros = new() { miembro1, miembro2 };

            Proyecto proyecto = CrearProyectoCon(_admin, miembros);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.EliminarProyecto(proyecto.Id, _admin);

            foreach (Usuario miembro in proyecto.Miembros)
            {
                Assert.AreEqual(2, miembro.Notificaciones.Count);
                Assert.AreEqual("Se eliminó el proyecto 'Proyecto'.", miembro.Notificaciones[1].Mensaje);
            }
        }

        //modificacion de nombre del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario noAdmin = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { noAdmin });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", noAdmin);
        }

        [TestMethod]
        public void ModificarNombreDelProyecto_ModificaNombreDelProyecto()
        {
            _gestor.CrearProyecto(_proyecto, _admin);

            _gestor.ModificarNombreDelProyecto(_proyecto.Id, "Nuevo Nombre", _admin);

            Assert.AreEqual("Nuevo Nombre", _proyecto.Nombre);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            Proyecto proyecto1 = CrearProyectoCon(_admin);
            Proyecto proyecto2 = CrearProyectoCon(CrearAdminProyecto(2));
            proyecto2.ModificarNombre("Otro Nombre");

            _gestor.CrearProyecto(proyecto1, _admin);
            _gestor.CrearProyecto(proyecto2, proyecto2.Administrador);

            _gestor.ModificarNombreDelProyecto(proyecto2.Id, proyecto1.Nombre, proyecto2.Administrador);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarNombreDelProyecto(1000, "nuevo", _admin);
        }


        [TestMethod]
        public void ModificarNombreDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario miembro1 = CrearMiembro(2);
            Usuario miembro2 = CrearMiembro(3);
            List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };
            Proyecto proyecto = CrearProyectoCon(_admin, miembros);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", _admin);

            string mensajeEsperado = "Se cambió el nombre del proyecto 'Proyecto' a 'Nuevo Nombre'.";

            foreach (Usuario usuario in proyecto.Miembros)
            {
                Assert.AreEqual(2, usuario.Notificaciones.Count);
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones[1].Mensaje);
            }
        }


        // modificacion de la descripcion del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", _miembro);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            _gestor.ModificarDescripcionDelProyecto(1000, "Nueva descripcion", _admin);

        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_ModificaDescripcionDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva descripcion", _admin);

            Assert.AreEqual("Nueva descripcion", proyecto.Descripcion);
        }

        [TestMethod]
        public void ModificarDescripcionDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            string nuevaDescripcion = "Nueva descripcion";
            _gestor.ModificarDescripcionDelProyecto(proyecto.Id, nuevaDescripcion, _admin);

            string mensajeEsperado = $"Se cambió la descripción del proyecto '{proyecto.Nombre}' a '{nuevaDescripcion}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }

        // modificacion de la fecha de inicio del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _miembro);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            DateTime nuevaFecha = DateTime.Now;
            _gestor.ModificarFechaDeInicioDelProyecto(1000, nuevaFecha, _admin);
        }

        [TestMethod]

        public void ModificarFechaDeInicioDelProyecto_ModificaFechaDeInicioDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _admin);

            Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
        }

        [TestMethod]
        public void ModificarFechaDeInicioDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            DateTime nuevaFecha = new DateTime(2026, 5, 1);
            _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _admin);

            string mensajeEsperado = $"Se cambió la fecha de inicio del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
            }
        }
        
        // modificacion de la fecha de fin mas temprana del proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaFinMasTempranaDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario admin = CrearAdminProyecto();
            DateTime nuevaFecha = DateTime.Today.AddDays(1000);

            _gestor.ModificarFechaFinMasTempranaDelProyecto(1000, nuevaFecha, admin);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaFinMasTempranaDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin = CrearAdminProyecto();
            Usuario noAdmin = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(admin);

            _gestor.CrearProyecto(proyecto, admin);

            DateTime nuevaFecha = DateTime.Today.AddDays(1000);
            _gestor.ModificarFechaFinMasTempranaDelProyecto(proyecto.Id, nuevaFecha, noAdmin);
        }
        
        [TestMethod]
        public void ModificarFechaFinMasTempranaDelProyecto_ModificaCorrectamente()
        {
            Usuario admin = CrearAdminProyecto();
            Proyecto proyecto = CrearProyectoCon(admin);
            _gestor.CrearProyecto(proyecto, admin);

            DateTime nuevaFecha = DateTime.Today.AddDays(15);
            _gestor.ModificarFechaFinMasTempranaDelProyecto(proyecto.Id, nuevaFecha, admin);

            Assert.AreEqual(nuevaFecha, proyecto.FechaFinMasTemprana);
        }
        
        [TestMethod]
        public void ModificarFechaFinMasTempranaDelProyecto_NotificaAMiembros()
        {
            Usuario admin = CrearAdminProyecto();
            Usuario miembro = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(admin, new List<Usuario> { miembro });

            _gestor.CrearProyecto(proyecto, admin);
            DateTime nuevaFecha = DateTime.Today.AddDays(10);

            _gestor.ModificarFechaFinMasTempranaDelProyecto(proyecto.Id, nuevaFecha, admin);

            string esperado = $"Se cambió la fecha de fin más temprana del proyecto '{proyecto.Nombre}' a '{nuevaFecha:dd/MM/yyyy}'.";

            foreach (var usuario in proyecto.Miembros)
            {
                Assert.AreEqual(2, usuario.Notificaciones.Count);
                Assert.AreEqual(esperado, usuario.Notificaciones[1].Mensaje);
            }
        }
        
        

        // cambiar administrador de proyecto
        [TestMethod]
        public void CambiarAdministradorDeProyecto_AsignaNuevoAdminOK()
        {
            Usuario adminSistema = CrearAdminSistema();
            Usuario adminActual = CrearAdminProyecto(1);
            Usuario nuevoAdmin = CrearAdminProyecto(2);

            Proyecto proyecto = CrearProyectoCon(adminActual, new List<Usuario> { nuevoAdmin });
            _gestor.CrearProyecto(proyecto, adminActual);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, nuevoAdmin.Id);

            Assert.AreSame(nuevoAdmin, proyecto.Administrador);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_DesactivaFlagDelAdminAnterior()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminViejo = CrearAdminProyecto(2);
            Usuario adminNuevo = CrearAdminProyecto(3);

            Proyecto proyecto = CrearProyectoCon(adminViejo, new List<Usuario> { adminNuevo });
            _gestor.CrearProyecto(proyecto, adminViejo);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsFalse(adminViejo.EstaAdministrandoProyecto);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_ActivaFlagDelAdminNuevo()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminViejo = CrearAdminProyecto(2);
            Usuario adminNuevo = CrearAdminProyecto(3);

            Proyecto proyecto = CrearProyectoCon(adminViejo, new List<Usuario> { adminNuevo });
            _gestor.CrearProyecto(proyecto, adminViejo);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, adminNuevo.Id);

            Assert.IsTrue(adminNuevo.EstaAdministrandoProyecto);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiSolicitanteNoEsAdminSistema()
        {
            Usuario noEsAdminSistema = CrearAdminProyecto(1);
            Usuario nuevoAdmin = CrearMiembro(2);
            Proyecto proyecto = CrearProyectoCon(noEsAdminSistema, new List<Usuario> { nuevoAdmin });

            _gestor.CrearProyecto(proyecto, noEsAdminSistema);

            _gestor.CambiarAdministradorDeProyecto(noEsAdminSistema, proyecto.Id, nuevoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminSistema = CrearAdminSistema();

            _gestor.CambiarAdministradorDeProyecto(adminSistema, 1000, 1);
        }


        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiNuevoAdminNoEsMiembro()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminProyecto = CrearAdminProyecto(2);
            Usuario externo = CrearMiembro(3); 

            Proyecto proyecto = CrearProyectoCon(adminProyecto);
            _gestor.CrearProyecto(proyecto, adminProyecto);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, externo.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionNuevoAdminYaAdministraOtroProyecto()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminProyecto = CrearAdminProyecto(2);
            Usuario nuevoAdmin = CrearAdminProyecto(3);
            nuevoAdmin.EstaAdministrandoProyecto = true;

            Proyecto proyecto = CrearProyectoCon(adminProyecto, new List<Usuario> { nuevoAdmin });
            _gestor.CrearProyecto(proyecto, adminProyecto);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, nuevoAdmin.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcion_NuevoAdminNoTienePermisosDeAdminProyecto()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminActual = CrearAdminProyecto(2);
            Usuario candidato = CrearMiembro(3); // No es admin

            Proyecto proyecto = CrearProyectoCon(adminActual, new List<Usuario> { candidato });
            _gestor.CrearProyecto(proyecto, adminActual);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, candidato.Id);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_NotificaALosMiembros()
        {
            Usuario adminSistema = CrearAdminSistema(1);
            Usuario adminActual = CrearAdminProyecto(2);
            Usuario nuevoAdmin = CrearAdminProyecto(3);
            Usuario miembro = CrearMiembro(4);

            Proyecto proyecto = CrearProyectoCon(adminActual, new() { nuevoAdmin, miembro });
            _gestor.CrearProyecto(proyecto, adminActual);

            _gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, nuevoAdmin.Id);

            string msg = $"Se cambió el administrador del proyecto '{proyecto.Nombre}'. El nuevo administrador es '{nuevoAdmin}'.";

            foreach (var u in new[] { nuevoAdmin, miembro })
            {
                Assert.AreEqual(2, u.Notificaciones.Count);
                Assert.AreEqual(msg, u.Notificaciones[1].Mensaje);
            }
        }

        //agregar miembro al proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario admin = CrearAdminProyecto();
            Usuario nuevoMiembro = CrearMiembro();

            _gestor.AgregarMiembroAProyecto(1000, admin, nuevoMiembro);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Usuario solicitanteSinPermisos = CrearMiembro();
            Usuario nuevo = CrearMiembro();

            _gestor.AgregarMiembroAProyecto(proyecto.Id, solicitanteSinPermisos, nuevo);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            Usuario solicitante = CrearAdminProyecto(2);
            Usuario nuevo = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin); 

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, nuevo);
        }

        [TestMethod]
        public void AgregarMiembro_AgregaElMiembroALaListaOK()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            Usuario nuevo = CrearMiembro();

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _admin, nuevo);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevo));
        }

        [TestMethod]
        public void AgregarMiembro_NotificaALosMiembros()
        {
            Usuario nuevo = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin);

            _gestor.CrearProyecto(proyecto, _admin);
            _gestor.AgregarMiembroAProyecto(proyecto.Id, _admin, nuevo);

            string esperado = $"Se agregó a un nuevo miembro (id {nuevo.Id}) al proyecto '{proyecto.Nombre}'.";

            foreach (var usuario in proyecto.Miembros.Concat(new[] { proyecto.Administrador }))
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
            }
        }

        //eliminar miembro del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_ProyectoNoExiste_LanzaExcepcion()
        {
            Usuario admin = CrearAdminProyecto();
            Usuario miembro = CrearMiembro();

            _gestor.EliminarMiembroDelProyecto(1000, admin, miembro.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario>() { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            Usuario noAdmin = CrearMiembro();

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, noAdmin, _miembro.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        {
            Usuario solicitante = CrearAdminProyecto(2);
            Proyecto proyecto = CrearProyectoCon(_admin, new() { _miembro });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, solicitante, _miembro.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiUsuarioNoEsMiembroDelProyecto()
        {
            Proyecto proyecto = CrearProyectoCon(_admin); 
            _gestor.CrearProyecto(proyecto, _admin);

            Usuario noMiembro = CrearMiembro();

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, noMiembro.Id);
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_EliminaMiembroOK()
        {
            Usuario miembro = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin, new() { miembro });

            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, miembro.Id);

            Assert.IsFalse(proyecto.Miembros.Contains(miembro));
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_NotificaALosMiembros()
        {
            Usuario miembro1 = CrearMiembro(2);
            Usuario miembro2 = CrearMiembro(3);

            Proyecto proyecto = CrearProyectoCon(_admin, new() { miembro1, miembro2 });
            _gestor.CrearProyecto(proyecto, _admin);

            _gestor.EliminarMiembroDelProyecto(proyecto.Id, _admin, miembro1.Id);

            string esperado = $"Se eliminó a el miembro (id {miembro1.Id}) del proyecto '{proyecto.Nombre}'.";

            foreach (var usuario in new[] { _admin, miembro2 })
            {
                Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
            }
        }

        // obtenerProyectosPorUsuario

        [TestMethod]
        public void ObtenerProyectosPorUsuario_DevuelveProyectosDelMiembro()
        {
            Usuario admin1 = CrearAdminProyecto(1);
            Usuario admin2 = CrearAdminProyecto(2);
            Usuario miembro1 = CrearMiembro(10);
            Usuario miembro2 = CrearMiembro(11);

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

        //agregr tarea al proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Tarea tarea = CrearTarea();
            _gestor.AgregarTareaAlProyecto(1000, _admin, tarea);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario noAdmin = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea();
            _gestor.AgregarTareaAlProyecto(proyecto.Id, noAdmin, tarea);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoElAdministradorDelProyecto()
        {
            Usuario otroAdmin = CrearAdminProyecto(id: 9);
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea();
            _gestor.AgregarTareaAlProyecto(proyecto.Id, otroAdmin, tarea);
        }

        [TestMethod]
        public void AgregarTareaAlProyecto_AgregaTareaOK()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea(10);
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

            Assert.AreEqual(1, proyecto.Tareas.Count);
            Assert.IsTrue(proyecto.Tareas.Contains(tarea));
        }

        [TestMethod]
        public void AgregarTareaAlProyecto_NotificaAMiembros()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea(10);
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

            string esperado = $"Se agregó la tarea (id {tarea.Id}) al proyecto '{proyecto.Nombre}'.";

            foreach (var miembro in proyecto.Miembros)
            {
                Assert.AreEqual(2, miembro.Notificaciones.Count);
                Assert.AreEqual(esperado, miembro.Notificaciones[1].Mensaje);
            }
        }

        //eliminar tarea del proyecto

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTareaDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Tarea tarea = CrearTarea(10);
            _gestor.EliminarTareaDelProyecto(1000, _admin, tarea.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTareaDelProyectoo_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario noAdmin = CrearMiembro();
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea();
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

            _gestor.EliminarTareaDelProyecto(proyecto.Id, noAdmin, tarea.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarTareaDelProyecto_LanzaExcepcionSiSolicitanteNoElAdministradorDelProyecto()
        {
            Usuario otroAdmin = CrearAdminProyecto(10);
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea();
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);

            _gestor.EliminarTareaDelProyecto(proyecto.Id, otroAdmin, tarea.Id);
        }

        [TestMethod]
        public void EliminarTareaDelProyecto_EliminaTareaOK()
        {
            Proyecto proyecto = CrearProyectoCon(_admin);
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea(10);
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
            _gestor.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

            Assert.AreEqual(0, proyecto.Tareas.Count);
            Assert.IsFalse(proyecto.Tareas.Contains(tarea));
        }
        
        
        [TestMethod]
        public void EliminarTareaDelProyecto_NotificaAMiembros()
        {
            Proyecto proyecto = CrearProyectoCon(_admin, new List<Usuario> { _miembro });
            _gestor.CrearProyecto(proyecto, _admin);

            Tarea tarea = CrearTarea(10);
            _gestor.AgregarTareaAlProyecto(proyecto.Id, _admin, tarea);
            _gestor.EliminarTareaDelProyecto(proyecto.Id, _admin, tarea.Id);

            string esperado = $"Se eliminó la tarea (id {tarea.Id}) del proyecto '{proyecto.Nombre}'.";

            foreach (var miembro in proyecto.Miembros)
            {
                Assert.AreEqual(3, miembro.Notificaciones.Count);
                Assert.AreEqual(esperado, miembro.Notificaciones[2].Mensaje);
            }
        }

    }
    
    // TODO:
    // se va a romper cuando ponga en proyecto fecha inicio

}