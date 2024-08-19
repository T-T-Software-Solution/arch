using Microsoft.EntityFrameworkCore;
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
                 State = it.GetState(),
                 UserActivity = it.Entity as IUserActivityEntity,
                 TimeActivity = it.Entity as ITimeActivityEntity,
             }) ?? [];

            if (false == entries.Any())
            {
                return;
            }

            EnsureCommitHasUserId();

            var now = timeService.UtcNow;
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Detached:
                        {
                            if (entry.TimeActivity is not null) entry.TimeActivity.CreatedDate = now;
                            if (entry.UserActivity is not null) entry.UserActivity.CreatedById = context.CurrentUserId!;
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            if (entry.TimeActivity is not null) entry.TimeActivity.DeletedDate = now;
                            if (entry.UserActivity is not null) entry.UserActivity.DeletedById = context.CurrentUserId;
                            break;
                        }
                    case EntityState.Modified:
                        {
                            if (entry.TimeActivity is not null) entry.TimeActivity.LastUpdatedDate = now;
                            if (entry.UserActivity is not null) entry.UserActivity.LastUpdatedById = context.CurrentUserId;
                            break;
                        }
                    default:
                        break;
                }
            }

            void EnsureCommitHasUserId()
            {
                if (context.CurrentUserId is null
                    && entries.Any(it => it.UserActivity is not null))
                {
                    throw new InvalidOperationException("Current user id is not set.");
                }
            }
        }
    }
}
