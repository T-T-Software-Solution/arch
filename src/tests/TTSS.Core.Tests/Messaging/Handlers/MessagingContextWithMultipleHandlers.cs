using TTSS.Core.Contexts;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Handlers;

public class FirstRequest : IRequesting<FirstResponse>;
public class FirstResponse : IResponse<MessagingContext>
{
    public MessagingContext Data { get; set; }
    public string Message { get; set; }
}
public class FirstHandler(IMessagingHub hub, ITestInterface testInterface, ICorrelationContext context) : RequestHandlerAsync<FirstRequest, FirstResponse>
{
    public override async Task<FirstResponse> HandleAsync(FirstRequest request, CancellationToken cancellationToken = default)
    {
        var messageContext = (MessagingContext)context;
        messageContext.Summary++;
        messageContext.FirstHandlerCanReceive = true;
        messageContext.FirstHandlerReceivedCorrelationId = context.CorrelationId;

        await testInterface.ExecuteAsync(request, cancellationToken);
        await hub.SendAsync(new SecondRequest(), cancellationToken);
        return new FirstResponse { Data = messageContext, Message = GetType().Name };
    }
}

public class SecondRequest : IRequesting;
public class SecondHandler(IMessagingHub hub, ITestInterface testInterface, ICorrelationContext context) : RequestHandler<SecondRequest>
{
    public override void Handle(SecondRequest request)
    {
        var messageContext = (MessagingContext)context;
        messageContext.Summary++;
        messageContext.SecondHandlerCanReceive = true;
        messageContext.SecondHandlerReceivedCorrelationId = context.CorrelationId;

        testInterface.Execute(request);
        var task = hub.SendAsync(new ThirdRequest());
        task.Wait();
        messageContext.MessageFromThirdHandler = task.Result.Message;
    }
}

public class ThirdRequest : IRequesting<ThirdResponse>;
public class ThirdResponse : IResponse
{
    public string Message { get; set; }
}

public class ThirdHandler(ITestInterface testInterface, ICorrelationContext context) : RequestHandler<ThirdRequest, ThirdResponse>
{
    public override ThirdResponse Handle(ThirdRequest request)
    {
        var messageContext = (MessagingContext)context;
        messageContext.Summary++;
        messageContext.ThirdHandlerCanReceive = true;
        messageContext.ThirdHandlerReceivedCorrelationId = context.CorrelationId;

        testInterface.Execute(request);
        return new ThirdResponse { Message = GetType().Name };
    }
}