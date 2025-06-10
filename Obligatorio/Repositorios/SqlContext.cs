using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios;

public class SqlContext : DbContext
{
    public DbSet<Recurso> Recursos { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> opciones) : base(opciones)
    {
        /*if (!Database.IsInMemory())
        {
            Database.Migrate();
        }*/
    }
    
    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CategoryConfiguration.Configure(modelBuilder);
        modelBuilder.Entity<Movie>()
            .HasMany<Actor>(x => x.Actors)
            .WithMany(x => x.Movies);
    }*/
    
}