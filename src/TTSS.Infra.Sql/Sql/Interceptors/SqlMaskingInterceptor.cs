using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql.Interceptors
{
    internal class SqlMaskingInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var maskableQry = eventData.Context?.ChangeTracker?.Entries()
                .Where(it => it is not null
                    && it.Entity is IMaskableEntity
                    && it.State is EntityState.Added or EntityState.Modified or EntityState.Detached)
                .Select(it => it.Entity)
                .Cast<IMaskableEntity>() ?? [];

            foreach (var maskable in maskableQry)
            {
                maskable.MaskData();
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
