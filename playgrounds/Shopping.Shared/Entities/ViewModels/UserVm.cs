using AutoMapper;
using TTSS.Core.Models;

namespace Shopping.Shared.Entities.ViewModels;

[AutoMap(typeof(User))]
public sealed record UserVm : IHaveOrderNumber
{
    public required string Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public int OrderNo { get; set; }
}