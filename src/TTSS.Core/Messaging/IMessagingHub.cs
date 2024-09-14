using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

public interface IMessagingHub
{
    Task PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken = default)
        where TPublication : IPublication;

    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequest;

    Task<TResponse> SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken = default)
       where TResponse : class;

    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request,
        TimeSpan timeout = default,
        Uri? destinationAddress = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class;
}