using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.Interceptors;

internal abstract class TestSqlInterceptorBase : SqlSaveChangesInterceptorBase
{
    public abstract bool IsManual { get; }
    public static event EventHandler<(object entity, bool isManual)> OnCreating;
    public static event EventHandler<(object entity, bool isManual)> OnDeleting;
    public static event EventHandler<(object entity, bool isManual, IEnumerable<PropertyUpdateInfo> infos)> OnUpdating;

    protected override Task OnCreateAsync(IDbModel entity, CancellationToken cancellationToken)
    {
        OnCreating?.Invoke(this, (entity, IsManual));
        return Task.CompletedTask;
    }

    protected override Task OnDeleteAsync(IDbModel entity, CancellationToken cancellationToken)
    {
        OnDeleting?.Invoke(this, (entity, IsManual));
        return Task.CompletedTask;
    }

    protected override Task OnUpdateAsync(IDbModel entity, IEnumerable<PropertyUpdateInfo> infos, CancellationToken cancellationToken)
    {
        OnUpdating?.Invoke(this, (entity, IsManual, infos));
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