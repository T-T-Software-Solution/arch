using System.Net;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample13.Basic.WebApp.ProjectStructure.Biz.Calculators;

public sealed record Divide(double Operand1, double Operand2) : IHttpRequesting<double>;

file sealed class Handler(IMessagingHub hub) : HttpRequestHandlerAsync<Divide, double>
{
    public override async Task<IHttpResponse<double>> HandleAsync(Divide request, CancellationToken cancellationToken = default)
    {
        var round = 0;
        var remainder = request.Operand1;
        while (remainder > request.Operand2)
        {
            round++;
            var rsp = await hub.SendAsync(new Sub(remainder, request.Operand2));
            remainder = rsp.Data;
        }
        return Response(HttpStatusCode.OK, round);
    }
}