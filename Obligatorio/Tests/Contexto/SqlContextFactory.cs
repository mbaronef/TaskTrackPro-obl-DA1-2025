using Microsoft.EntityFrameworkCore;
using Repositorios;

namespace Tests.Contexto;

public static class SqlContextFactory
{
    public static SqlContext CreateMemoryContext() 
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlContext>();
        optionsBuilder.UseInMemoryDatabase("TestDB");
        return new SqlContext(optionsBuilder.Options);
    }
}