using AutoMapper;

namespace Shipping.Shared.Entities.ViewModels;

[AutoMap(typeof(User))]
public sealed record UserVm
{
    public required string Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}