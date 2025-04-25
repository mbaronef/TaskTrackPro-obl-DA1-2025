using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

namespace Tests

{
    [TestClass]
    public class ProyectoTests
    {
        [TestMethod]
        public void ConstructorConParametrosAsignadosCorrectamente()
        {
            string nombre = "Proyecto 1";
            string descripcion = "Descripción";
            List<Tarea> tareas = new List<Tarea>();
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin }; // debe agregar al admin a la lista de miembros
            
            Proyecto proyecto = new Proyecto (nombre, descripcion, tareas, admin, miembros);
            
            Assert.AreEqual(nombre, proyecto.Nombre);
            Assert.AreEqual(descripcion, proyecto.Descripcion);
            Assert.AreEqual(tareas, proyecto.Tareas);
            Assert.AreEqual(admin, proyecto.Administrador);
            Assert.AreEqual(miembros, proyecto.Miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void Constructor_DeberiaLanzarExcepcionSiDescripcionSupera400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 401); 
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);
        }
        
        [TestMethod]
        public void Constructor_DeberiaPermitirDescripcionConMenosDe400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 399);
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        [TestMethod]
        public void Constructor_DeberiaPermitirDescripcionDeHasta400Caracteres()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            string descripcion = new string('a', 400); // 400 caracteres exactos
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Proyecto", descripcion, tareas, admin, miembros);

            Assert.AreEqual(descripcion, proyecto.Descripcion);
        }
        
        
        
        [TestMethod]
        public void FechaInicioInicializadaConFechaActual()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            DateTime antes = DateTime.Now;
            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);
            DateTime despues = DateTime.Now;

            Assert.IsTrue(proyecto.FechaInicio >= antes && proyecto.FechaInicio <= despues); // porque DateTime.Now cambia
        }
        
        [TestMethod]
        public void FechaFinMasTempranaInicializadaConMinValue()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            var proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);

            Assert.AreEqual(DateTime.MinValue, proyecto.FechaFinMasTemprana);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiNombreEsVacio()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("", "Descripción válida", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiNombreEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto(null, "Descripción válida", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiDescripcionEsVacia()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre válido", "", tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiDescripcionEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();
            
            Proyecto proyecto = new Proyecto("Nombre válido", null, tareas, admin, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiAdministradorEsNull()
        {
            List<Usuario> miembros = new List<Usuario> { new Usuario() };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, null, miembros);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiMiembrosEsNull()
        {
            Usuario admin = new Usuario();
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void ConstructorLanzaExcepcionSiMiembrosNoContieneAdministrador() // este no se si alguna vez pasa porque lo pusimos que cuando se crea lo agrega
        {
            Usuario admin = new Usuario();
            Usuario otro = new Usuario();
            List<Usuario> miembros = new List<Usuario> { otro };
            List<Tarea> tareas = new List<Tarea>();

            Proyecto proyecto = new Proyecto("Nombre", "Descripción", tareas, admin, miembros);
        }
        
        [TestMethod]
        public void AgregarTarea_DeberiaAgregarUnaTareaALaLista()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            Tarea tarea1 = new Tarea();
            
            proyecto.AgregarTarea(tarea1); 
            
            Assert.IsTrue(proyecto.Tareas.Contains(tarea1));
            Assert.AreEqual(1, proyecto.Tareas.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_DeberiaLanzarExcepcionSiTareaEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.AgregarTarea(null); 
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AgregarTarea_DeberiaLanzarExcepcionSiTareaYaEstaEnTareas()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Tarea  tarea1 = new Tarea();
            List<Tarea> tareas = new List<Tarea> { tarea1 };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción",  tareas, admin, miembros);
            
            proyecto.AgregarTarea(tarea1);
        }

        [TestMethod]
        public void AsignarMiembro_DeberiaAgregarUsuarioALaListaDeMiembros()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            Usuario nuevoMiembro = new Usuario();

            proyecto.AsignarMiembro(nuevoMiembro);

            Assert.IsTrue(proyecto.Miembros.Contains(nuevoMiembro));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_DeberiaLanzarExcepcionSiUsuarioEsNull()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            List<Tarea> tareas = new List<Tarea> { new Tarea() };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.AsignarMiembro(null); 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionDominio))]
        public void AsignarMiembro_DeberiaLanzarExcepcionSiUsuarioYaEstaEnMiembros()
        {
            Usuario admin = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin };
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", new List<Tarea>(), admin, miembros);

            proyecto.AsignarMiembro(admin); 
        }
        
        
        [TestMethod]
        public void EliminarMiembro_DeberiaEliminarUsuarioDeLaLista() // en realidad deberia recibir el id del miembro a eliminar
        {
            Usuario admin = new Usuario();
            Usuario miembro = new Usuario();
            List<Usuario> miembros = new List<Usuario> { admin, miembro };
            List<Tarea> tareas = new List<Tarea>();
            Proyecto proyecto = new Proyecto("Proyecto 1", "Descripción", tareas, admin, miembros);

            proyecto.EliminarMiembro(miembro);

            Assert.IsFalse(proyecto.Miembros.Contains(miembro));
            Assert.AreEqual(1, proyecto.Miembros.Count);
        }
        
        // falta:
        // que no deje eliminar usuario que no exista en miembros
        // que no deje eliminar al admin "se debe asignar a otro administrador de proyecto previamente"
        
        // metodos faltantes:
        // calcularRutaCritica()
        // darRecursosFaltantes()
        // esAdministrador(Usuario usuario)
        // modificaciones (ver que cosas modificar)
        // notificarMiembros(string mensaje)
        // notificarAdministradores (string mensaje)
        // falta eliminar tarea que no esta en UML !!!!!!!
        
    }
}