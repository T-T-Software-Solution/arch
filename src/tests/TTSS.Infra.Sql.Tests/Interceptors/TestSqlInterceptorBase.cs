using TTSS.Core.Data;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql.DbContexte;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.Interceptors;

internal abstract class TestSqlInterceptorBase(IDateTimeService dateTimeService) : SqlSaveChangesInterceptorBase
{
    public abstract bool IsManual { get; }
    public static event EventHandler<(object entity, bool isManual)> OnAuditEntityAdded;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlPropertyInfo> properties)> OnCreating;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlPropertyInfo> properties)> OnDeleting;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlUpdatePropertyInfo> properties)> OnUpdating;

    protected override Task OnCreateAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        AddAuditEntity(new AuditLog { Id = Guid.NewGuid().ToString(), Action = "Create", Message = entity.GetType().Name });
        OnCreating?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }

    protected override Task OnDeleteAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        AddAuditEntity(new AuditLog { Id = Guid.NewGuid().ToString(), Action = "Delete", Message = entity.GetType().Name });
        OnDeleting?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }

    protected override Task OnUpdateAsync(IDbModel entity, IEnumerable<SqlUpdatePropertyInfo> properties, CancellationToken cancellationToken)
    {
        AddAuditEntity(new AuditLog { Id = Guid.NewGuid().ToString(), Action = "Update", Message = entity.GetType().Name });
        OnUpdating?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }

    protected override void AddAuditEntity(params IAuditEntity[] entities)
    {
        base.AddAuditEntity(entities);
        foreach (var item in entities)
        {
            OnAuditEntityAdded?.Invoke(this, (item, IsManual));
        }
    }
}
internal class TestSqlInterceptorIoC(IDateTimeService dateTimeService) : TestSqlInterceptorBase(dateTimeService)
{
    public override bool IsManual => false;
}
internal class TestSqlInterceptorManual(IDateTimeService dateTimeService) : TestSqlInterceptorBase(dateTimeService)
{
    public override bool IsManual => true;
}