using AutoMapper;

namespace Shipping.Shared.Entities.ViewModels;

[AutoMap(typeof(Product))]
public sealed record ProductVm
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
}