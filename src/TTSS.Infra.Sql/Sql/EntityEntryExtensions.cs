using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

internal static class EntityEntryExtensions
{
    public static EntityState GetState(this EntityEntry target)
    {
        if (target.Entity is ITimeActivityEntity time)
        {
            return time.DeletedDate.HasValue ? EntityState.Deleted : target.State;
        }
        return target.State;
    }
}