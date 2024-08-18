using TTSS.Core.Models;

namespace TTSS.Infra.Data.Sql.Contexts;

public class TestContext : CorrelationContext, ISetterCorrelationContext
{
    public TestContext(string userId)
        => SetCurrentUserId(userId);

    public void SetCurrentUserId(string currentUserId)
        => SetCurrentUserIdentity(currentUserId);

    public void SetCorrelationId(string correlationId)
        => SetCorrelationIdentity(correlationId);
}