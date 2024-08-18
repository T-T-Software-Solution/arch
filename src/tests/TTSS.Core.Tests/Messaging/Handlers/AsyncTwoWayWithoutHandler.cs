namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayWithoutHandler : IRequesting<AsyncTwoWayWithoutHandlerResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayWithoutHandlerResponse(int value)
{
    public int Value { get; } = value;
}