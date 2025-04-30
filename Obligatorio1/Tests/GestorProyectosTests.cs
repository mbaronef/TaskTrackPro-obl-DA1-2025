using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;
using NuGet.Frameworks;

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
            solicitante.EstaAdministrandoProyecto = true; 

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
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CrearProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            Usuario adminSistema = new Usuario();
            adminSistema.EsAdministradorProyecto = true;
            adminSistema.EstaAdministrandoProyecto = false;

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario>();

            Proyecto proyecto1 = new Proyecto("nombre", "descripcion1", adminSistema, miembros);
            gestor.CrearProyecto(proyecto1, adminSistema);

            Proyecto proyecto2 = new Proyecto("nombre", "descripcion2", adminSistema, miembros);
            gestor.CrearProyecto(proyecto2, adminSistema); 
        }
        
        [TestMethod]
        public void CrearProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario solicitante = new Usuario();
            solicitante.EsAdministradorProyecto = true;
            solicitante.EstaAdministrandoProyecto = false;

            Usuario miembro1 = new Usuario();
            Usuario miembro2 = new Usuario();
            List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto Notificado", "Descripción", solicitante, miembros);

            gestor.CrearProyecto(proyecto, solicitante);

            Assert.AreEqual(1, miembro1.Notificaciones.Count);
            Assert.AreEqual(1, miembro2.Notificaciones.Count);
            Assert.AreEqual("Se creó el proyecto 'Proyecto Notificado'.", miembro1.Notificaciones[0].Mensaje);
            Assert.AreEqual("Se creó el proyecto 'Proyecto Notificado'.", miembro2.Notificaciones[0].Mensaje);
        }   
        
        // eliminarProyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarProyecto_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            List<Usuario> miembros = new List<Usuario> { noAdmin }; 
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, admin); 
            
            Assert.IsFalse(proyecto.EsAdministrador(noAdmin)); 

            gestor.EliminarProyecto(proyecto.Id, noAdmin); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario admin = new Usuario( );
            admin.EsAdministradorProyecto = true;
            GestorProyectos gestor = new GestorProyectos();

            gestor.EliminarProyecto(1000, admin);   
        }
        
        [TestMethod]
        public void EliminarProyecto_EliminaDeListaAlProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> ();
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            gestor.CrearProyecto(proyecto, admin);
            
            Assert.IsTrue(admin.EstaAdministrandoProyecto);
            Assert.AreEqual(1, gestor.Proyectos.Count);
            
            gestor.EliminarProyecto(proyecto.Id, admin);
            
            Assert.AreEqual(0, gestor.Proyectos.Count); 
        }
        
        [TestMethod]
        public void EliminarProyecto_CambiaLaFLagEstaAdministrandoProyectoDelAdministrador()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> ();
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            gestor.CrearProyecto(proyecto, admin);
            
            Assert.IsTrue(admin.EstaAdministrandoProyecto);
            Assert.AreEqual(1, gestor.Proyectos.Count);
            
            gestor.EliminarProyecto(proyecto.Id, admin);
            
            Assert.IsFalse(admin.EstaAdministrandoProyecto); 
        }
        
        [TestMethod]
        public void EliminarProyecto_NotificaAMiembrosDelProyecto()
        {
            Usuario solicitante = new Usuario();
            solicitante.EsAdministradorProyecto = true;
            solicitante.EstaAdministrandoProyecto = false;

            Usuario miembro1 = new Usuario();
            Usuario miembro2 = new Usuario();
            List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto A Eliminar", "Descripción", solicitante, miembros);

            gestor.CrearProyecto(proyecto, solicitante);
            gestor.EliminarProyecto(proyecto.Id, solicitante);

            Assert.AreEqual(2, miembro1.Notificaciones.Count);
            Assert.AreEqual(2, miembro2.Notificaciones.Count);
            Assert.AreEqual("Se eliminó el proyecto 'Proyecto A Eliminar'.", miembro1.Notificaciones[1].Mensaje);
            Assert.AreEqual("Se eliminó el proyecto 'Proyecto A Eliminar'.", miembro2.Notificaciones[1].Mensaje);
        }  
        
        //modificacion de nombre del proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", noAdmin);
        }
        
        [TestMethod]
        public void ModificarNombreDelProyecto_ModificaNombreDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", admin);
            
            Assert.AreEqual(proyecto.Nombre, "Nuevo Nombre");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiNombreYaExiste()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto A", "desc", admin, miembros);
            
            
            Usuario admin2 = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin2 = new Usuario();
            List<Usuario> miembros2 = new List<Usuario> { noAdmin2 };
            Proyecto proyecto2 = new Proyecto("Proyecto B", "desc", admin2, miembros2);
            
            gestor.CrearProyecto(proyecto, admin);
            gestor.CrearProyecto(proyecto2, admin2);

            gestor.ModificarNombreDelProyecto(proyecto2.Id, "Proyecto A", admin2);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarNombreDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            GestorProyectos gestor = new GestorProyectos();

            gestor.ModificarNombreDelProyecto(100, "nuevo", adminProyecto);
        }
        
        
        [TestMethod]
        public void ModificarNombreDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", admin);
            
            Assert.AreEqual(2, noAdmin.Notificaciones.Count);
            Assert.AreEqual(2, admin.Notificaciones.Count);
            Assert.AreEqual("Se cambió el nombre del proyecto 'Proyecto B' a 'Nuevo Nombre'.", noAdmin.Notificaciones[1].Mensaje);
            Assert.AreEqual("Se cambió el nombre del proyecto 'Proyecto B' a 'Nuevo Nombre'.", admin.Notificaciones[1].Mensaje);
        }
        
        
        // modificacion de la descripcion del proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", noAdmin);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarDescripcionDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            GestorProyectos gestor = new GestorProyectos();

            gestor.ModificarDescripcionDelProyecto(100, "nueva desc", adminProyecto);
        }
        
        [TestMethod]
        public void ModificarDescripcionDelProyecto_ModificaDescripcionDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", admin);
            
            Assert.AreEqual(proyecto.Descripcion, "Nueva Descripcion");
        }
        
        [TestMethod]
        public void ModificarDescripcionDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion", admin);
            
            Assert.AreEqual(2, noAdmin.Notificaciones.Count);
            Assert.AreEqual(2, admin.Notificaciones.Count);
            Assert.AreEqual("Se cambió la descripción del proyecto 'Proyecto B' a 'Nueva Descripcion'.", noAdmin.Notificaciones[1].Mensaje);
            Assert.AreEqual("Se cambió la descripción del proyecto 'Proyecto B' a 'Nueva Descripcion'.", admin.Notificaciones[1].Mensaje);
        }
        
        // modificacion de la fecha de inicio del proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);
            DateTime nuevaFecha = DateTime.Now;

            gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, noAdmin);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            GestorProyectos gestor = new GestorProyectos();
            DateTime nuevaFecha = DateTime.Now;

            gestor.ModificarFechaDeInicioDelProyecto(100, nuevaFecha, adminProyecto);
        }
        
        [TestMethod]
        
        public void ModificarFechaDeInicioDelProyecto_ModificaFechaDeInicioDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);
            
            DateTime nuevaFecha = DateTime.Now;

            gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, admin);
            
            Assert.AreEqual(proyecto.FechaInicio, nuevaFecha);
        }
        
        [TestMethod]
        public void ModificarFechaDeInicioDelProyecto_NotificaALosMiembrosDelProyecto()
        {
            Usuario admin = new Usuario();
            admin.EsAdministradorProyecto = true;
            Usuario noAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { noAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);
            
            DateTime nuevaFecha = DateTime.Now;

            gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, admin);
            
            Assert.AreEqual(2, noAdmin.Notificaciones.Count);
            Assert.AreEqual(2, admin.Notificaciones.Count);
            Assert.AreEqual($"Se cambió la fecha de inicio del proyecto 'Proyecto B' a '{nuevaFecha:dd/MM/yyyy}'.", noAdmin.Notificaciones[1].Mensaje);
            Assert.AreEqual($"Se cambió la fecha de inicio del proyecto 'Proyecto B' a '{nuevaFecha:dd/MM/yyyy}'.", admin.Notificaciones[1].Mensaje);
        }
        
        // cambiar administrador de proyecto
        [TestMethod]
        public void CambiarAdministradorDeProyecto_AsignaNuevoAdminOK()
        {
            Usuario adminSistema = new Usuario { EsAdministradorSistema = true };
            Usuario adminProyectoActual = new Usuario { EsAdministradorProyecto = true };
            Usuario nuevoAdmin = new Usuario { EsAdministradorProyecto = true };

            List<Usuario> miembros = new List<Usuario>{ nuevoAdmin };
            Proyecto proyecto = new Proyecto("Proyecto X", "Desc", adminProyectoActual, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, adminProyectoActual);
            
            gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, nuevoAdmin.Id);
            
            Assert.AreSame(nuevoAdmin, proyecto.Administrador);
        }
        
        [TestMethod]
        public void CambiarAdministradorDeProyecto_DesactivaFlagDelAdminAnterior()
        {
            Usuario adminSis   = new Usuario { EsAdministradorSistema = true };

            Usuario adminViejo = new Usuario { EsAdministradorProyecto = true};

            Usuario adminNuevo = new Usuario { EsAdministradorProyecto = true };

            List<Usuario> miembros = new List<Usuario> { adminViejo, adminNuevo };
            Proyecto proyecto = new Proyecto("P", "Desc", adminViejo, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, adminViejo);
            
            gestor.CambiarAdministradorDeProyecto(adminSis, proyecto.Id, adminNuevo.Id); 

            Assert.IsFalse(adminViejo.EstaAdministrandoProyecto);
        }
        
        [TestMethod]
        public void CambiarAdministradorDeProyecto_ActivaFlagDelAdminNuevo()
        {
            Usuario adminSis   = new Usuario { EsAdministradorSistema = true };

            Usuario adminViejo = new Usuario { EsAdministradorProyecto = true};

            Usuario adminNuevo = new Usuario { EsAdministradorProyecto = true };

            List<Usuario> miembros = new List<Usuario> { adminViejo, adminNuevo };
            Proyecto proyecto = new Proyecto("P", "Desc", adminViejo, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, adminViejo);
            
            gestor.CambiarAdministradorDeProyecto(adminSis, proyecto.Id, adminNuevo.Id); 

            Assert.IsTrue(adminNuevo.EstaAdministrandoProyecto);
        }

        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiSolicitanteNoEsAdminSistema()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            admin.EsAdministradorSistema = false;
            Usuario miembroASerNuevoAdmin = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            List<Usuario> miembros = new List<Usuario> { miembroASerNuevoAdmin };
            Proyecto proyecto = new Proyecto("Proyecto B", "desc", admin, miembros);
            
            gestor.CrearProyecto(proyecto, admin);

            gestor.CambiarAdministradorDeProyecto(admin, proyecto.Id, miembroASerNuevoAdmin.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminSistema = new Usuario { EsAdministradorSistema = true };
            GestorProyectos gestor = new GestorProyectos();

            gestor.CambiarAdministradorDeProyecto(adminSistema, 99, 1);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionSiNuevoAdminNoEsMiembro()
        {
            Usuario adminSis = new Usuario { EsAdministradorSistema = true };
            Usuario adminProj = new Usuario { EsAdministradorProyecto = true };
            Usuario externo = new Usuario();
            
            List<Usuario> miembros = new List<Usuario>{adminSis};

            Proyecto proyecto = new Proyecto("Proyecto", "Desc", adminProj, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, adminProj);

            gestor.CambiarAdministradorDeProyecto(adminSis, proyecto.Id, externo.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcionNuevoAdminYaAdministraOtroProyecto()
        {
            Usuario adminSistema   = new Usuario { EsAdministradorSistema = true };
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            Usuario adminB = new Usuario { EsAdministradorProyecto = true, EstaAdministrandoProyecto = true };

            Proyecto proyecto1 = new Proyecto("P1", "Desc", adminProyecto, new List<Usuario>{ adminB });
            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto1, adminProyecto);
            
            gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto1.Id, adminB.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void CambiarAdministradorDeProyecto_LanzaExcepcion_NuevoAdminNoTienePermisosDeAdminProyecto()
        {
            Usuario adminSistema = new Usuario { EsAdministradorSistema = true };
            Usuario adminproyectoActual = new Usuario { EsAdministradorProyecto = true };
            Usuario candidato = new Usuario(); // EsAdministradorProyecto = false

            List<Usuario> miembros = new() { candidato };
            Proyecto proyecto = new Proyecto("Proyecto Z", "Desc", adminproyectoActual, miembros);

            GestorProyectos gestor = new GestorProyectos();
            gestor.CrearProyecto(proyecto, adminproyectoActual);
            
            gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, candidato.Id);
        }

        [TestMethod]
        public void CambiarAdministradorDeProyecto_NotificaALosMiembros()
        {
            Usuario adminSistema = new Usuario { EsAdministradorSistema = true };
            Usuario adminProjecto = new Usuario { EsAdministradorProyecto = true };
            Usuario candidato = new Usuario() {EsAdministradorProyecto = true};
            Usuario miembro = new Usuario();
            
            List<Usuario> miembros = new List<Usuario>{candidato, miembro};
            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto B", "descrp", adminProjecto, miembros);
            
            gestor.CrearProyecto(proyecto, adminProjecto);
            gestor.CambiarAdministradorDeProyecto(adminSistema, proyecto.Id, candidato.Id);
            
            
            Assert.AreEqual(2, candidato.Notificaciones.Count);
            Assert.AreEqual(2, miembro.Notificaciones.Count);
            Assert.AreEqual($"Se cambió el administrador del proyecto 'Proyecto B'. El nuevo administrador es '{candidato}'.", candidato.Notificaciones[1].Mensaje);
            Assert.AreEqual($"Se cambió el administrador del proyecto 'Proyecto B'. El nuevo administrador es '{candidato}'.", miembro.Notificaciones[1].Mensaje);
        }
        
        //agregar miembro al proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario adminSis = new Usuario { EsAdministradorProyecto = true, EstaAdministrandoProyecto = true};
            GestorProyectos gestor   = new GestorProyectos();
            Usuario aAgregar = new Usuario();
            gestor.AgregarMiembroAProyecto(99, adminSis, aAgregar);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
        {
            Usuario adminProj = new Usuario { EsAdministradorProyecto = true };
            Usuario solicitante = new Usuario(); // tiene EsAdminDeProyecto = false  
            Usuario miembro = new Usuario();
            GestorProyectos gestor = new GestorProyectos();

            Proyecto proyecto = new Proyecto("Proyecto","Descripcion", adminProj, new List<Usuario>{ miembro });
            gestor.CrearProyecto(proyecto, adminProj);

            gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, miembro);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarMiembro_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        { 
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            Usuario solicitante = new Usuario { EsAdministradorProyecto = true };
            Usuario nuevo = new Usuario();

            GestorProyectos gestor   = new GestorProyectos();
            Proyecto proyecto = new Proyecto("P","D", adminProyecto, new List<Usuario>());
            gestor.CrearProyecto(proyecto, adminProyecto);

            gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, nuevo);
        }

        [TestMethod]
        public void AgregarMiembro_AgregaElMiembroALaListaOK()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto", "Descripcion", admin, new List<Usuario> ());
            gestor.CrearProyecto(proyecto, admin); 

            gestor.AgregarMiembroAProyecto(proyecto.Id, admin, miembro);

            Assert.IsTrue(proyecto.Miembros.Contains(miembro));
        }

        [TestMethod]
        public void AgregarMiembro_NotificaALosMiembros()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario nuevoMiembro = new Usuario();
            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto", "Descripcion", admin, new List<Usuario> {});
            gestor.CrearProyecto(proyecto, admin);
            gestor.AgregarMiembroAProyecto(proyecto.Id, admin, nuevoMiembro);
            
            string esperado = $"Se agregó a un nuevo miembro (id {nuevoMiembro.Id}) al proyecto 'Proyecto'.";
            Assert.AreEqual(2, admin.Notificaciones.Count);  
            Assert.AreEqual(1, nuevoMiembro.Notificaciones.Count);
            Assert.AreEqual(esperado, admin.Notificaciones[1].Mensaje);
            Assert.AreEqual(esperado, nuevoMiembro.Notificaciones[0].Mensaje);
        }
        
        //eliminar miembro del proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_ProyectoNoExiste_LanzaExcepcion()
        {
            Usuario admin  = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario();

            GestorProyectos gestor = new GestorProyectos();

            gestor.EliminarMiembroDelProyecto(100, admin, miembro.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin   = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();
            Usuario miembro = new Usuario();

            GestorProyectos gestor   = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto","Descripcion" ,admin, new List<Usuario>{ miembro });
            gestor.CrearProyecto(proyecto, admin);

            gestor.EliminarMiembroDelProyecto(proyecto.Id, noAdmin, miembro.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
        { 
            Usuario adminProyecto = new Usuario { EsAdministradorProyecto = true };
            Usuario solicitante = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario();

            GestorProyectos gestor   = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto","Descripcion", adminProyecto, new List<Usuario>{miembro});
            gestor.CrearProyecto(proyecto, adminProyecto);

            gestor.EliminarMiembroDelProyecto(proyecto.Id, solicitante, miembro.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void EliminarMiembroDelProyecto_LanzaExcepcionSiUsuarioNoEsMiembroDelProyecto()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario noMiembro = new Usuario();

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto","Descripcion" ,admin,new List<Usuario>());
            gestor.CrearProyecto(proyecto, admin);

            gestor.EliminarMiembroDelProyecto(proyecto.Id, admin, noMiembro.Id);
        }

        [TestMethod]
        public void EliminarMiembroDelProyecto_EliminaMiembroOK()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario();
            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto", "Descripcion", admin, new() { miembro });
            gestor.CrearProyecto(proyecto, admin);

            gestor.EliminarMiembroDelProyecto(proyecto.Id, admin, miembro.Id);

            Assert.IsFalse(proyecto.Miembros.Contains(miembro));
        }
        
        [TestMethod]
        public void EliminarMiembroDelProyecto_NotificaALosMiembros()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro = new Usuario();
            Usuario miembro2 = new Usuario();
            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto = new Proyecto("Proyecto", "Descripcion", admin, new List<Usuario> {miembro, miembro2});
            gestor.CrearProyecto(proyecto, admin);
            gestor.EliminarMiembroDelProyecto(proyecto.Id, admin, miembro.Id);
            
            string esperado = $"Se eliminó a el miembro (id {miembro.Id}) del proyecto 'Proyecto'.";
            Assert.AreEqual(2, admin.Notificaciones.Count);  
            Assert.AreEqual(2, miembro2.Notificaciones.Count);
            Assert.AreEqual(esperado, admin.Notificaciones[1].Mensaje);
            Assert.AreEqual(esperado, miembro2.Notificaciones[1].Mensaje);
        }

        // obtenerTodosLosProyectos

        [TestMethod]
        public void ObtenerTodosLosProyectos_DevuelveListaCompleta()
        {
            Usuario admin1 = new Usuario { EsAdministradorProyecto = true };
            Usuario admin2 = new Usuario { EsAdministradorProyecto = true };

            GestorProyectos gestor = new GestorProyectos();
            Proyecto proyecto1 = new Proyecto("Proyecto 1", "Desc", admin1, new List<Usuario> { });
            Proyecto proyecto2 = new Proyecto("Proyecto 2", "Desc", admin2, new List<Usuario> { });

            gestor.CrearProyecto(proyecto1, admin1);
            gestor.CrearProyecto(proyecto2, admin2);

            List<Proyecto> lista = gestor.ObtenerTodosLosProyectos();

            Assert.AreEqual(2, lista.Count);
            CollectionAssert.Contains(lista, proyecto1);
            CollectionAssert.Contains(lista, proyecto2);
        }

        // obtenerProyectosPorUsuario
        
        [TestMethod]
        public void ObtenerProyectosPorUsuario_DevuelveProyectosDelMiembro()
        {
            Usuario admin1 = new Usuario { EsAdministradorProyecto = true };
            Usuario admin2 = new Usuario { EsAdministradorProyecto = true };
            Usuario miembro1 = new Usuario();
            Usuario miembro2 = new Usuario();

            GestorProyectos gestor = new GestorProyectos();

            Proyecto proyecto1 = new Proyecto("P1", "Desc", admin1, new List<Usuario>{ miembro1, miembro2});
            Proyecto proyecto2 = new Proyecto("P2", "Desc", admin2, new List<Usuario>{miembro1});
            gestor.CrearProyecto(proyecto1, admin1);
            gestor.CrearProyecto(proyecto2, admin2);

            List<Proyecto> listaProyectosMiembro1 = gestor.ObtenerProyectosPorUsuario(miembro1.Id);
            List<Proyecto> listaProyectosMiembro2 = gestor.ObtenerProyectosPorUsuario(miembro2.Id);

            Assert.AreEqual(2, listaProyectosMiembro1.Count);
            Assert.AreEqual(proyecto1, listaProyectosMiembro1[0]);
            Assert.AreEqual(proyecto2, listaProyectosMiembro1[1]);
            
            Assert.AreEqual(1, listaProyectosMiembro2.Count);
            Assert.AreEqual(proyecto1, listaProyectosMiembro2[0]);
        }
        
        //agregr tarea al proyecto
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiProyectoNoExiste()
        {
            Usuario admin = new Usuario { EsAdministradorProyecto = true };
            GestorProyectos gestor = new GestorProyectos();
            Tarea tarea = new Tarea();

            gestor.AgregarTareaAlProyecto(100, admin, tarea);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
        {
            Usuario admin   = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario();
            GestorProyectos gestor  = new GestorProyectos();

            Proyecto proyecto = new Proyecto("Proyecto X","Descripcion",admin,new List<Usuario>());
            gestor.CrearProyecto(proyecto, admin);

            gestor.AgregarTareaAlProyecto(proyecto.Id, noAdmin, new Tarea());
        }

        [TestMethod] [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTareaAlProyecto_LanzaExcepcionSiSolicitanteNoElAdministradorDelProyecto()
        {
            Usuario admin   = new Usuario { EsAdministradorProyecto = true };
            Usuario noAdmin = new Usuario{EsAdministradorProyecto = true};
            GestorProyectos gestor  = new GestorProyectos();

            Proyecto proyecto = new Proyecto("Proyecto X","Descripcion", admin,new List<Usuario>());
            gestor.CrearProyecto(proyecto, admin);
            Tarea  tarea = new Tarea();

            gestor.AgregarTareaAlProyecto(proyecto.Id, noAdmin, tarea);
        }
        
        //eliminar tarea del proyecto
        
        
        
        //TODO:
        
        // agregarTareaDelProyecto (lo puede hacer si es admin de proyecto) seria crearTarea (cuando se crea se agrega a la lista de tareas)
        // manda notificacion a cada uno de los miembros del proyecto 
        
        // eliminarTareaDelProyecto (admin de proyecto) seria eliminarTarea
        // manda notificacion a cada uno de los miembros del proyecto
        
    }
}