using Sample16.RemoteRequest.Shared.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample17.RemoteRequest.WebApp.Dogs;

public sealed record DogsCreate : IHttpRequesting
{
    public int Age { get; set; }
    public required string Name { get; set; }
}

file sealed class Handler(IRepository<Dog> repository) : HttpRequestHandlerAsync<DogsCreate>
{
    public override async Task<IHttpResponse> HandleAsync(DogsCreate request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Response(HttpStatusCode.BadRequest, "Name is required.");
        }

        var entity = new Dog
        {
            Id = Guid.NewGuid().ToString(),
            Age = request.Age,
            Name = request.Name
        };
        await repository.InsertAsync(entity, cancellationToken);
        return Response(HttpStatusCode.Created);
    }
}