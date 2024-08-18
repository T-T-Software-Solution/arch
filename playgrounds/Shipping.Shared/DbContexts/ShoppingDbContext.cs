using Microsoft.EntityFrameworkCore;
using Shipping.Shared.Entities;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql;

namespace Shipping.Shared.DbContexts;

public sealed class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
    : DbContextBase<ShoppingDbContext>(options), IAuditRepository
{
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public Task AddAuditEntityAsync(IEnumerable<IAuditEntity> entities, CancellationToken cancellationToken = default)
        => AuditLogs.AddRangeAsync(entities.Cast<AuditLog>(), cancellationToken);
}