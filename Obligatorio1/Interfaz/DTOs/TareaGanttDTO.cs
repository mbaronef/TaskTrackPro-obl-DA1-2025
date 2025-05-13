namespace Interfaz.DTOs;

public class TareaGanttDTO
{
    public string id { get; set; } 
    public string name { get; set; }
    public string start { get; set; }  // en formato YYYY-MM-DD
    public string end { get; set; } 
    public int progress { get; set; }
    public string dependencies { get; set; }
}