using AutoMapper;
using Sample16.RemoteRequest.Shared.Messages;
using Sample16.RemoteRequest.Shared.ViewModels;
using System.Net;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample17.RemoteRequest.WebApp.Pings;

public sealed record PingsGet(int first, int second) : IHttpRequesting<PongVm>
{
    public int First { get; init; } = first;
    public int Second { get; init; } = second;
}

file sealed class Handler(IMessagingHub hub, IMapper mapper) : HttpRequestHandlerAsync<PingsGet, PongVm>
{
    public override async Task<IHttpResponse<PongVm>> HandleAsync(PingsGet request, CancellationToken cancellationToken = default)
    {
        var response = await hub.SendAsync<Ping, Pong>(new Ping
        {
            First = request.First,
            Second = request.Second
        });

        if (response is null)
        {
            return Response(HttpStatusCode.BadGateway, "No response from the server.");
        }

        var vm = mapper.Map<PongVm>(response);
        return Response(HttpStatusCode.OK, vm);
    }
}