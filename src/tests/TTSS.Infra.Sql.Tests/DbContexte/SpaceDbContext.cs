using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql.DbModels;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class SpaceDbContext : DbContextBase<SpaceDbContext>
{
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Spaceship> Spaceships { get; set; }

    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options)
    {
    }
}

internal class Astronaut : SqlDbModel
{
    [Comment("Name of the astronaut")]
    public string Name { get; set; }

    [Comment("Size of the astronaut")]
    public int Size { get; set; }
}

internal class Spaceship : SqlDbModel
{
    [Comment("Name of the spaceship")]
    public string Name { get; set; }

    public double Power { get; set; }
}