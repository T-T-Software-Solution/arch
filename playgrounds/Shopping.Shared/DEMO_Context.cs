using TTSS.Core.Models;

namespace Shopping.Shared;

public sealed class DEMO_Context : CorrelationContext, ISetterCorrelationContext
{
    public void SetCorrelationId(string correlationId)
        => SetCorrelationIdentity(correlationId);

    public void SetCurrentUserId(string? currentUserId)
        => SetCurrentUserIdentity(currentUserId);
}