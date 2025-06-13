using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionProyecto
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proyecto>()
            .Property(p => p.Nombre)
            .IsRequired();

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.Descripcion)
            .IsRequired()
            .HasMaxLength(400);

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.FechaInicio)
            .IsRequired();

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.FechaFinMasTemprana)
            .IsRequired();
    }
    
}