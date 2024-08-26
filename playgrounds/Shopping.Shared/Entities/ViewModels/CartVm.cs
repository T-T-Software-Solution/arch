namespace Shopping.Shared.Entities.ViewModels;

public sealed record CartVm
{
    public string Id { get; init; }
    public string OwnerName { get; set; }
    public IEnumerable<ProductVm> Products { get; init; } = [];
    public double TotalPrice => Products?.Sum(it => it.Price) ?? 0;
    public DateTime CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
    public DateTime? DeletedDate { get; init; }
}