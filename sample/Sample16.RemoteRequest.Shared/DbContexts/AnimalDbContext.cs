using Microsoft.EntityFrameworkCore;
using Sample16.RemoteRequest.Shared.Entities;
using TTSS.Infra.Data.Sql;

namespace Sample16.RemoteRequest.Shared.DbContexts;

public sealed class AnimalDbContext(DbContextOptions<AnimalDbContext> options)
    : DbContextBase<AnimalDbContext>(options)
{
    public DbSet<Dog> Dogs { get; set; }
}