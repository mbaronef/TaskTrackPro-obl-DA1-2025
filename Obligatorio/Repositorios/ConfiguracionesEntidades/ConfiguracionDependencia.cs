using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionDependencia
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dependencia>().Property(d => d.Tipo)
            .IsRequired()
            .HasMaxLength(2);
        modelBuilder.Entity<Dependencia>().HasCheckConstraint("CK_Dependencia_Tipo", "[Tipo] IN ('SS', 'FS')");
        
        modelBuilder.Entity<Dependencia>()
            .HasKey("TareaId", "Tipo");  // clave compuesta con shadow property para TareaId
        
        modelBuilder.Entity<Dependencia>()
            .HasOne(d => d.Tarea)
            .WithMany(t => t.Dependencias)
            .HasForeignKey("TareaId");
    }
}