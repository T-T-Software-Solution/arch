using Microsoft.Extensions.Logging;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample10.Basic.ConsoleApp.LongRunningTask.Handlers;

public sealed record EchoRequest(string Message) : IRequesting;

file sealed class Handler(ILogger<EchoRequest> logger) : RequestHandler<EchoRequest>
{
    private const string LogFormat = "Echo: {@Message}";

    public override void Handle(EchoRequest request)
    {
        logger.LogTrace(LogFormat, request.Message);
        logger.LogDebug(LogFormat, request.Message);
        logger.LogInformation(LogFormat, request.Message);
        logger.LogWarning(LogFormat, request.Message);
        logger.LogError(LogFormat, request.Message);
        logger.LogCritical(LogFormat, request.Message);
    }
}