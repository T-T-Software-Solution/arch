using Sample01.Basic.ConsoleApp.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample01.Basic.ConsoleApp.Handlers;

public sealed class PingHandler : RequestHandler<Ping, Pong>
{
    public override Pong Handle(Ping request)
    {
        return new Pong
        {
            Response = $"Pong, {request.Name}!"
        };
    }
}
