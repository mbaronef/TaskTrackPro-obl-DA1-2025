using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionDependencia
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dependencia>().HasKey(d => new { d.TareaId, d.Tipo });

        modelBuilder.Entity<Dependencia>()
            .HasOne(d => d.Tarea)
            .WithMany(t => t.Dependencias)
            .HasForeignKey(d => d.TareaId);

        modelBuilder.Entity<Dependencia>().Property(d => d.Tipo)
            .IsRequired()
            .HasMaxLength(2);
        modelBuilder.Entity<Dependencia>().HasCheckConstraint("CK_Dependencia_Tipo", "[Tipo] IN ('SS', 'FS')");
        
        
        // ver si funciona (en vez de las 2 primeras):
        modelBuilder.Entity<Dependencia>()
            .HasKey("TareaId", "Tipo");  // clave compuesta con shadow property para TareaId

        modelBuilder.Entity<Dependencia>()
            .HasOne(d => d.Tarea)
            .WithMany(t => t.Dependencias)
            .HasForeignKey("TareaId"); // FK shadow property
        // si anda, podemos sacar TareaId de Dependencia.cs y dejarlo como shadow property
    }
}