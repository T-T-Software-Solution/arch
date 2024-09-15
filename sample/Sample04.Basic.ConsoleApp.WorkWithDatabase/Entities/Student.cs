using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;

[Comment("นักเรียน")]
public sealed class Student : SqlModelBase, ITimeActivityEntity
{
    [Comment("ชื่อและนามสกุล")]
    public required string FullName { get; set; }

    [Comment("เกรด")]
    public double GPA { get; set; }

    [Comment("อาจารย์ที่ปรึกษา")]
    public Teacher? Teacher { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}