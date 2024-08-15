using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.DbModels;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class SpaceDbContext : DbContextBase<SpaceDbContext>, IAuditRepository
{
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Spaceship> Spaceships { get; set; }
    public DbSet<AuditLog> Audits { get; set; }

    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options)
    {
    }

    public Task AddAuditEntityAsync(IEnumerable<IAuditEntity> entities, CancellationToken cancellationToken = default)
        => Audits.AddRangeAsync(entities.Select(it => (AuditLog)it), cancellationToken);
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

internal class AuditLog : SqlDbModel, IAuditEntity
{
    public string Action { get; set; }
    public string Message { get; set; }
}