using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionRecurso
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recurso>().Property(r => r.Nombre)
            .IsRequired();

        modelBuilder.Entity<Recurso>().Property(r => r.Descripcion)
            .IsRequired();

        modelBuilder.Entity<Recurso>().Property(r => r.Tipo)
            .IsRequired();

        modelBuilder.Entity<Recurso>().Property(r => r.CantidadDeTareasUsandolo)
            .IsRequired();


    }
}