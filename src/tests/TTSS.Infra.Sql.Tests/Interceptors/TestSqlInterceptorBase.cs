using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.Interceptors;

internal abstract class TestSqlInterceptorBase : SqlSaveChangesInterceptorBase
{
    public abstract bool IsManual { get; }
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlPropertyInfo> properties)> OnCreating;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlPropertyInfo> properties)> OnDeleting;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<SqlUpdatePropertyInfo> properties)> OnUpdating;

    protected override Task OnCreateAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        OnCreating?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }

    protected override Task OnDeleteAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        OnDeleting?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }

    protected override Task OnUpdateAsync(IDbModel entity, IEnumerable<SqlUpdatePropertyInfo> properties, CancellationToken cancellationToken)
    {
        OnUpdating?.Invoke(this, (entity, IsManual, properties));
        return Task.CompletedTask;
    }
}
internal class TestSqlInterceptorIoC : TestSqlInterceptorBase
{
    public override bool IsManual => false;
}
internal class TestSqlInterceptorManual : TestSqlInterceptorBase
{
    public override bool IsManual => true;
}