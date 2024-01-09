using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class FruitDbContext : DbContextBase<FruitDbContext>
{
    public DbSet<Apple> Apple { get; set; }
    public DbSet<Banana> Banana { get; set; }

    public FruitDbContext(DbContextOptions<FruitDbContext> options) : base(options)
    {
    }
}

internal abstract class FruitBase : SqlModelBase
{
    public int Price { get; set; }
    public string Name { get; set; }
}

internal class Apple : FruitBase { }
internal class Banana : FruitBase { }