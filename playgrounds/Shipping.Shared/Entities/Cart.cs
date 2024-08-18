﻿using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace Shipping.Shared.Entities;

[Comment("ตะกร้าสินค้า")]
public sealed class Cart : SqlModelBase, ITimeActivityEntity
{
    [Comment("เจ้าของ")]
    public required User Owner { get; set; }

    [Comment("รายการสินค้า")]
    public Collection<Product> Products { get; set; } = [];

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}