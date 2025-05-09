using Dominio;
using Servicios.Excepciones;

namespace Servicios.Utilidades;

public static class CaminoCritico
{
    public static List<Tarea> OrdenarTopologicamente(List<Tarea> tareas)
    {
        List<Tarea> tareasOrdenadas = new List<Tarea>();

        Dictionary<Tarea, int> gradosDeEntrada = new Dictionary<Tarea, int>();
        Dictionary<Tarea, List<Tarea>> sucesoras = new Dictionary<Tarea, List<Tarea>>();
        foreach (Tarea tarea in tareas)
        {
            gradosDeEntrada[tarea] = 0;
            sucesoras[tarea] = new List<Tarea>();
        }

        foreach (Tarea tarea in tareas)
        {
            foreach (Dependencia dependencia in tarea.Dependencias)
            {
                Tarea anterior = dependencia.Tarea;
                gradosDeEntrada[tarea]++;
                sucesoras[anterior].Add(tarea);
            }
        }

        Queue<Tarea> cola = new Queue<Tarea>(tareas.Where(t => gradosDeEntrada[t] == 0));
        while (cola.Any())
        {
            Tarea tareaActual = cola.Dequeue();
            tareasOrdenadas.Add(tareaActual);

            foreach (Tarea siguiente in sucesoras[tareaActual])
            {
                gradosDeEntrada[siguiente]--;
                if (gradosDeEntrada[siguiente] == 0)
                {
                    cola.Enqueue(siguiente);
                }
            }
        }

        if (tareasOrdenadas.Count != tareas.Count)
        {
            // Validación: si hay ciclo, no se procesaron todas
            throw new ExcepcionServicios("El grafo de tareas tiene dependencias cíclicas.");
        }

        return tareasOrdenadas;
    }
}