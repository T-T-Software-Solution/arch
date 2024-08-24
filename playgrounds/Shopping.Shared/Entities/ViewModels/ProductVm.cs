using AutoMapper;
using TTSS.Core.Models;

namespace Shopping.Shared.Entities.ViewModels;

[AutoMap(typeof(Product))]
public sealed record ProductVm : IHaveOrderNumber
{
    public string Id { get; init; }
    public string Name { get; init; }
    public double Price { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
    public DateTime? DeletedDate { get; init; }
    public string CreatedById { get; init; }
    public string? LastUpdatedById { get; init; }
    public string? DeletedById { get; init; }
    public int OrderNo { get; set; }
}