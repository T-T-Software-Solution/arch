using Shopping.Shared.Entities;
using System.Text;
using TTSS.Core.Data;
using TTSS.Core.Models;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql.Interceptors;
using TTSS.Infra.Data.Sql.Models;

namespace Shopping.Shared.Interceptors;

public sealed class AuditInterceptor(IDateTimeService dateTimeService, ICorrelationContext context) : SqlSaveChangesInterceptorBase(dateTimeService, context)
{
    protected override Task OnCreateAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        var qry = properties.Select(it => $"({it.Remark ?? it.ColumnName}: {it.Value})");
        var sb = new StringBuilder()
            .Append($"เพิ่มข้อมูลใหม่ลงตาราง {entity.GetType().Name} โดยมีรายละเอียดดังนี้ ")
            .Append(string.Join(' ', qry));
        AddAuditEntity(new AuditLog
        {
            Id = Guid.NewGuid().ToString(),
            Description = sb.ToString(),
        });
        return Task.CompletedTask;
    }

    protected override Task OnDeleteAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken)
    {
        var qry = properties
            .Where(it => it.ColumnName == "Id")
            .Select(it => $"({it.Remark ?? it.ColumnName}: {it.Value})");
        var sb = new StringBuilder()
            .Append($"ลบข้อมูลออกจากตาราง {entity.GetType().Name} โดยมีรายละเอียดดังนี้ ")
            .Append(string.Join(' ', qry));
        AddAuditEntity(new AuditLog
        {
            Id = Guid.NewGuid().ToString(),
            Description = sb.ToString(),
        });
        return Task.CompletedTask;
    }

    protected override Task OnUpdateAsync(IDbModel entity, IEnumerable<SqlUpdatePropertyInfo> properties, CancellationToken cancellationToken)
    {
        var qry = properties.Select(it => $"(ข้อมูล '{it.Remark ?? it.ColumnName}' ค่าเดิม {it.Value} ถูกแก้ไขเป็น {it.NewValue})");
        var sb = new StringBuilder()
            .Append($"แก้ไขมูลในตาราง {entity.GetType().Name} โดยมีรายละเอียดดังนี้ ")
            .Append(string.Join(' ', qry));
        AddAuditEntity(new AuditLog
        {
            Id = Guid.NewGuid().ToString(),
            Description = sb.ToString(),
        });
        return Task.CompletedTask;
    }
}