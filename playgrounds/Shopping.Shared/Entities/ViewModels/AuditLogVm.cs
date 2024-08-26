using AutoMapper;

namespace Shopping.Shared.Entities.ViewModels;

[AutoMap(typeof(AuditLog))]
public sealed record AuditLogVm
{
    public required string Description { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}