using Sample11.Basic.WebApp.SetupProject.Messages;
using System.Net;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample11.Basic.WebApp.SetupProject.Handlers;

file sealed class GreetingHandler : HttpRequestHandler<Greeting>
{
    public override IHttpResponse Handle(Greeting request)
    {
        Console.WriteLine($"Received: {request.Message}");
        return Response(HttpStatusCode.OK);
    }
}