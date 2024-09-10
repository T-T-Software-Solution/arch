using Microsoft.Extensions.Logging;
using Shopping.Shared.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.BackgroundTasks.Biz;

sealed record CheckStock : IRequesting<bool>;

file sealed class Handler(ICorrelationContext context, ILogger<CheckStock> logger, IRepository<Product> repository) : RequestHandlerAsync<CheckStock, bool>
{
    public override async Task<bool> HandleAsync(CheckStock request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Checking stock...");
        logger.LogDebug("CorrelationId: {@CorrelationId}", context.CorrelationId);
        var totalProducts = repository.Get(cancellationToken).Count();
        await Console.Out.WriteLineAsync($"Total products: {totalProducts}");
        return totalProducts > 0;
    }
}