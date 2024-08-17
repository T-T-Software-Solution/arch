using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Core.Data;
using TTSS.Core.Data.Models;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql.Interceptors
{
    internal class SqlActivityLogInterceptor(IDateTimeService dateTimeService) : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var entries = eventData.Context?.ChangeTracker?.Entries() ?? [];
            if (false == entries.Any())
            {
                return await SavingChangesAsync();
            }

            AssignActivityLog(entries, dateTimeService);
            return await SavingChangesAsync();

            ValueTask<InterceptionResult<int>> SavingChangesAsync()
                => base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        internal static void AssignActivityLog(IEnumerable<EntityEntry> entityEntries, IDateTimeService timeService)
        {
            var entries = entityEntries
             .Where(it => it is not null && it.Entity is IHaveActivityLog)
             .Select(it => new { Entry = it, Entity = (IHaveActivityLog)it.Entity, }) ?? [];

            if (false == entries.Any())
            {
                return;
            }

            var now = timeService.UtcNow;
            foreach (var entry in entries)
            {
                entry.Entity.ActivityLog ??= new ActivityLog { CreatedDate = now };
                switch (entry.Entry.State)
                {
                    case Microsoft.EntityFrameworkCore.EntityState.Added:
                    case Microsoft.EntityFrameworkCore.EntityState.Detached:
                        entry.Entity.ActivityLog.CreatedDate = now;
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                        entry.Entity.ActivityLog.DeletedDate = now;
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Modified:
                        entry.Entity.ActivityLog.LastUpdatedDate = now;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
