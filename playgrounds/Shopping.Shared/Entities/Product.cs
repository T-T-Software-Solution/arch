using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace Shopping.Shared.Entities;

[Comment("สินค้า")]
public sealed class Product : SqlModelBase, ITimeActivityEntity, IUserActivityEntity
{
    [Comment("ชื่อสินค้า")]
    public required string Name { get; set; }

    [Comment("ราคา")]
    public required double Price { get; set; }

    [Comment("ตะกร้าสินค้า")]
    public ICollection<Cart> Carts { get; set; } = [];

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string CreatedById { get; set; }
    public string? LastUpdatedById { get; set; }
    public string? DeletedById { get; set; }
}