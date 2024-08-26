using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql.Models;

namespace Shopping.Shared.Entities;

[Comment("ผู้ใช้")]
public sealed class User : SqlModelBase
{
    [Comment("ชื่อ")]
    public string? FirstName { get; set; }

    [Comment("นามสกุล")]
    public string? LastName { get; set; }
}