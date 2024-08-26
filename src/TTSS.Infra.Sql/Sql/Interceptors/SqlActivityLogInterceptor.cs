using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Core.Data;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql.Interceptors
{
    internal class SqlActivityLogInterceptor(IDateTimeService dateTimeService, ICorrelationContext context) : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var entries = eventData.Context?.ChangeTracker?.Entries() ?? [];
            if (false == entries.Any())
            {
                return await SavingChangesAsync();
            }

            AssignActivityLog(entries, dateTimeService, context);
            return await SavingChangesAsync();

            ValueTask<InterceptionResult<int>> SavingChangesAsync()
                => base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        internal static void AssignActivityLog(IEnumerable<EntityEntry> entityEntries, IDateTimeService timeService, ICorrelationContext context)
        {
            var entries = entityEntries
             .Where(it => it is not null && it.Entity is ITimeActivityEntity or IUserActivityEntity)
             .Select(it => new
             {
                 Entry = it,
                 UserEntity = it.Entity as IUserActivityEntity,
                 TimeEntity = it.Entity as ITimeActivityEntity,
             }) ?? [];

            if (false == entries.Any())
            {
                return;
            }

            var now = timeService.UtcNow;
            foreach (var entry in entries)
            {
                switch (entry.Entry.State)
                {
                    case Microsoft.EntityFrameworkCore.EntityState.Added:
                    case Microsoft.EntityFrameworkCore.EntityState.Detached:
                        if (entry.TimeEntity is not null) entry.TimeEntity.CreatedDate = now;
                        if (entry.UserEntity is not null) entry.UserEntity.CreatedById = context.CurrentUserId;
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                        if (entry.TimeEntity is not null) entry.TimeEntity.DeletedDate = now;
                        if (entry.UserEntity is not null) entry.UserEntity.DeletedById = context.CurrentUserId;
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Modified:
                        if (entry.TimeEntity is not null) entry.TimeEntity.LastUpdatedDate = now;
                        if (entry.UserEntity is not null) entry.UserEntity.LastUpdatedById = context.CurrentUserId;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
