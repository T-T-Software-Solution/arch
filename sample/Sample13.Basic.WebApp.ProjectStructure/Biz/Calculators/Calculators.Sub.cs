using System.Net;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample13.Basic.WebApp.ProjectStructure.Biz.Calculators;

public sealed record Sub(double Operand1, double Operand2) : IHttpRequesting<double>;

file sealed class Handler : HttpRequestHandler<Sub, double>
{
    public override IHttpResponse<double> Handle(Sub request)
        => Response(HttpStatusCode.OK, request.Operand1 - request.Operand2);
}