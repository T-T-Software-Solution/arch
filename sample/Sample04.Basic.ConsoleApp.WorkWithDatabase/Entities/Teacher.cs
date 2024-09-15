using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;

[Comment("อาจารย์")]
public sealed class Teacher : SqlModelBase, ITimeActivityEntity
{
    [Comment("ชื่อและนามสกุล")]
    public required string FullName { get; set; }

    [Comment("เงินเดือน")]
    public double Salary { get; set; }

    [Comment("นักเรียนที่ต้องดูแล")]
    public ICollection<Student> Students { get; set; } = [];

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}