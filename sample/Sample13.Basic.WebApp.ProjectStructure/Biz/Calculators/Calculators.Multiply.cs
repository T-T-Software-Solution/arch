using System.Net;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample13.Basic.WebApp.ProjectStructure.Biz.Calculators;

public sealed record Multiply(double Operand1, double Operand2) : IHttpRequesting<double>;

file sealed class Handler(IMessagingHub hub) : HttpRequestHandlerAsync<Multiply, double>
{
    public override async Task<IHttpResponse<double>> HandleAsync(Multiply request, CancellationToken cancellationToken = default)
    {
        var sum = 0d;
        for (var i = 0; i < request.Operand2; i++)
        {
            var rsp = await hub.SendAsync(new Add(request.Operand1, sum));
            sum = rsp.Data;
        }
        return Response(HttpStatusCode.OK, sum);
    }
}