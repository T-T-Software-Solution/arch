namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayWithoutHandler : IRequesting<AsyncTwoWayWithoutHandlerResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayWithoutHandlerResponse
{
    public AsyncTwoWayWithoutHandlerResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}