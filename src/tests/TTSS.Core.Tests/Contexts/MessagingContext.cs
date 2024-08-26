using TTSS.Core.Models;

namespace TTSS.Core.Contexts;

public class MessagingContext : CorrelationContext, ISetterCorrelationContext
{
    public int Summary { get; set; }
    public string MessageFromThirdHandler { get; set; }

    public bool FirstHandlerCanReceive { get; set; }
    public bool SecondHandlerCanReceive { get; set; }
    public bool ThirdHandlerCanReceive { get; set; }
    public string FirstHandlerReceivedCorrelationId { get; set; }
    public string SecondHandlerReceivedCorrelationId { get; set; }
    public string ThirdHandlerReceivedCorrelationId { get; set; }

    public void SetCorrelationId(string correlationId)
        => SetCorrelationIdentity(correlationId);

    public void SetCurrentUserId(string currentUserId)
        => SetCurrentUserIdentity(currentUserId);
}