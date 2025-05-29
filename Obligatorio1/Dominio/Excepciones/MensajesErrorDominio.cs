namespace Dominio.Excepciones;

public static class MensajesErrorDominio
{
    // Usuario
    public const string NotificacionNoExiste = "No existe la notificación.";
    public const string AtributoVacio = "El atributo {0} no puede ser vacío.";
    public const string EdadMinima = "El usuario debe tener más de {0} años.";
    public const string EdadMaxima = "El usuario debe tener menos de {0} años.";
    public const string EmailInvalido = "El email tiene un formato inválido.";

    // Dependencia
    public const string TipoDependenciaInvalido = "El tipo de dependencia debe ser 'FS' o 'SS'.";
    public const string TareaNula = "Una tarea no puede ser nula.";

    // Proyecto
    public const string DescripcionMuyLarga = "La descripción no puede superar los {0} caracteres.";
    public const string TareaYaAgregada = "La tarea ya fue agregada al proyecto.";
    public const string NoPuedeEliminarAdmin = "No se puede eliminar al administrador actual. Asigne un nuevo administrador antes.";
    public const string MiembroYaEnProyecto = "El miembro ya pertenece al proyecto.";
    public const string FechaInicioMenorAHoy = "La fecha de inicio no puede ser anterior a hoy.";
    public const string FechaInicioMayorQueTareas = "La fecha de inicio no puede ser posterior a la de alguna tarea.";
    public const string FechaFinMenorQueInicio = "La fecha de fin más temprana no puede ser anterior a la fecha de inicio del proyecto.";
    public const string FechaFinMenorQueTareas = "La fecha de fin más temprana no puede ser menor que la fecha de fin de una tarea.";
    public const string FechaInicioMayorQueFin = "La fecha de inicio no puede ser mayor que la fecha de fin más temprana.";
    public const string FechaInicioIgualFin = "La fecha de inicio no puede ser la misma que la fecha de fin más temprana.";
    public const string NombreProyectoVacio = "El nombre del proyecto no puede estar vacío o null.";
    public const string DescripcionProyectoVacia = "La descripción del proyecto no puede estar vacía o null.";
    public const string ProyectoSinAdministrador = "El proyecto debe tener un administrador.";
    public const string MiembrosNull = "La lista de miembros no puede ser null.";
    public const string TareaNull = "No se puede agregar una tarea null.";
    public const string TareaNoPertenece = "La tarea no pertenece al proyecto.";
    public const string MiembroNull = "No se puede agregar un miembro null.";
    public const string UsuarioNoEsMiembro = "El usuario no es miembro del proyecto.";



    // Recurso
    public const string RecursoYaEsExclusivo = "El recurso ya es exclusivo de otro proyecto.";
    public const string CantidadTareasRecursoNegativa = "La cantidad de tareas usando este recurso no puede ser menor a cero.";

    // Notificación
    public const string MensajeNotificacionVacio = "El mensaje de la notificación no puede estar vacío o null.";

    // Tarea
    public const string FechaTareaInvalida = "La fecha no puede ser anterior a hoy.";
    public const string DuracionTareaInvalida = "La duración no puede ser cero o negativa.";
    public const string UsuariosAsignadosVacio = "La lista de usuarios asignados está vacía o no está inicializada.";
    public const string FechaInicioInvalida = "La fecha de inicio debe ser igual o posterior a la fecha de hoy.";
    public const string EstadoInvalido_Pendiente = "No se puede cambiar una tarea finalizada a pendiente.";
    public const string EstadoInvalido_Proceso = "No se puede cambiar una tarea finalizada a en proceso.";
    public const string EstadoInvalido_Bloqueada = "No se puede cambiar una tarea finalizada a bloqueada.";
    public const string EstadoInvalido_ProcesoAPendiente = "No se puede cambiar una tarea en proceso a pendiente.";
    public const string EstadoInvalido_PendienteACompletada = "No se puede cambiar una tarea pendiente a completada.";
    public const string EstadoInvalido_BloqueadaACompletada = "No se puede cambiar una tarea bloqueada a completada.";
    public const string UsuarioYaAgregado = "El usuario ya fue agregado a la tarea.";
    public const string RecursoYaAgregado = "El recurso ya fue agregado.";
    public const string DependenciaYaAgregada = "La dependencia ya fue agregada.";
    public const string TituloTareaVacio = "El título de la tarea no puede estar vacío o ser nulo.";
    public const string DescripcionTareaVacia = "La descripción de la tarea no puede ser vacía o nula";
    public const string UsuarioNullEnAsignacion = "No se puede asignar una tarea a un usuario null.";
    public const string UsuarioNoAsignado = "El usuario no está asignado a la tarea.";
    public const string RecursoNullEnTarea = "No se puede agregar un recurso null.";
    public const string RecursoNoNecesario = "El recurso no se encuentra dentro de los recursos necesarios.";
    public const string DependenciaNullEnTarea = "No se puede agregar una dependencia null.";
    public const string DependenciaNoExisteEnTarea = "La dependencia no se encuentra dentro de la lista de dependencias.";
    

}