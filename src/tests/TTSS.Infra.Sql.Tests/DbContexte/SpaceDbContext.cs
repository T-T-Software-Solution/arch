﻿using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.DbModels;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class SpaceDbContext(DbContextOptions<SpaceDbContext> options) : DbContextBase<SpaceDbContext>(options), IAuditRepository
{
    public DbSet<Astronaut> Astronauts { get; set; }
    public DbSet<Spaceship> Spaceships { get; set; }
    public DbSet<AuditLog> Audits { get; set; }
    public DbSet<SensitivitySpaceStation> SensitivitySpaceStations { get; set; }
    public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
    public DbSet<SeriousLog> SeriousLogs { get; set; }

    public Task AddAuditEntityAsync(IEnumerable<IAuditEntity> entities, CancellationToken cancellationToken = default)
        => Audits.AddRangeAsync(entities.Select(it => (AuditLog)it), cancellationToken);
}

internal class Astronaut : SqlDbModel
{
    [Comment("Name of the astronaut")]
    public string Name { get; set; }

    [Comment("Size of the astronaut")]
    public int Size { get; set; }

    public string GetDisplayName()
        => Name;
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

internal class SensitivitySpaceStation : SqlDbModel, IMaskableEntity
{
    [Comment("Secret of the space station")]
    public string Secret { get; set; }

    public void MaskData()
        => Secret = string.Join(string.Empty, Secret.Reverse());
}

internal class MaintenanceLog : SqlDbModel, ITimeActivityEntity
{
    public int Attempt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}

internal class SeriousLog : SqlDbModel, ITimeActivityEntity, IUserActivityEntity
{
    public int Attempt { get; set; }
    public string CreatedById { get; set; }
    public string LastUpdatedById { get; set; }
    public string DeletedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}