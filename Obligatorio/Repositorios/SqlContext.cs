using Dominio;
using Microsoft.EntityFrameworkCore;
using Utilidades;

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
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("TaskTrackPro@2025");
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "Sistema",
                Email = "admin@sistema.com",
                FechaNacimiento = new DateTime(1999, 01, 01),
                EsAdministradorSistema = true,
                ContrasenaEncriptada = contrasenaEncriptada
            });
        
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