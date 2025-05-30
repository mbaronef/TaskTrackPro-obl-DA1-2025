namespace Servicios.Notificaciones;

public static class MensajesNotificacion
{
    public static string FechaFinMasTempranaActualizada(string nombreProyecto, DateTime nuevaFecha) =>
        $"Se cambió la fecha de fin más temprana del proyecto '{nombreProyecto}' a '{nuevaFecha:dd/MM/yyyy}'.";
    
    public static string ProyectoCreado(string nombre) =>
        $"Se creó el proyecto '{nombre}'.";
    public static string ProyectoEliminado(string nombre) =>
        $"Se eliminó el proyecto '{nombre}'.";
    public static string NombreProyectoModificado(string anterior, string nuevo) =>
        $"Se cambió el nombre del proyecto '{anterior}' a '{nuevo}'.";
    public static string DescripcionProyectoModificada(string nombre, string nuevaDescripcion) =>
        $"Se cambió la descripción del proyecto '{nombre}' a '{nuevaDescripcion}'.";
    public static string FechaInicioProyectoModificada(string nombre, DateTime nuevaFecha) =>
        $"Se cambió la fecha de inicio del proyecto '{nombre}' a '{nuevaFecha:dd/MM/yyyy}'.";
    public static string AdministradorProyectoModificado(string nombreProyecto, string nuevoAdmin) =>
        $"Se cambió el administrador del proyecto '{nombreProyecto}'. El nuevo administrador es '{nuevoAdmin}'.";
    public static string MiembroAgregado(string nombreProyecto, int idMiembro) =>
        $"Se agregó a un nuevo miembro (id {idMiembro}) al proyecto '{nombreProyecto}'.";
    public static string MiembroEliminado(string nombreProyecto, int idMiembro) =>
        $"Se eliminó al miembro (id {idMiembro}) del proyecto '{nombreProyecto}'.";

}