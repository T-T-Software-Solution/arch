using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace Shopping.Shared.Entities;

[Comment("บันทึกการตรวจสอบ")]
public sealed class AuditLog : SqlModelBase, IAuditEntity, ITimeActivityEntity
{
    [Comment("รายละเอียดการดำเนินการ")]
    public required string Description { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}