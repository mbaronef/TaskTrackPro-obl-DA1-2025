using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios;

public class SqlContext : DbContext
{
    public DbSet<Recurso> Recursos { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> opciones) : base(opciones)
    { 
        if (Database.IsRelational())
        {
            Database.Migrate();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .Property<string>("_contrasenaEncriptada") 
            .HasColumnName("ContrasenaEncriptada")   
            .IsRequired();
        
        modelBuilder.Entity<Dependencia>()
            .HasKey(d => new { d.TareaId, d.Tipo });
        
        modelBuilder.Entity<Dependencia>()
            .HasOne(d => d.Tarea)
            .WithMany(t => t.Dependencias)
            .HasForeignKey(d => d.TareaId);
        
        modelBuilder.Entity<Proyecto>()
            .HasOne(p => p.Administrador)
            .WithMany()
            .HasForeignKey("AdministradorId") 
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Proyecto>()
            .Navigation(p => p.Administrador).AutoInclude();
    }
    
}