using System.Security.Claims;
using TTSS.Core.Messaging.Pipelines;
using TTSS.Core.Models;

namespace TTSS.Core.Web.Pipelines;

/// <summary>
/// Pipeline behavior to set the current user ID from the HTTP context.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <param name="context">The correlation context</param>
/// <param name="accessor">The HTTP context accessor</param>
public sealed class HttpUserIdentityPipelineBehavior<TRequest, TResponse>(ICorrelationContext context, IHttpContextAccessor accessor)
    : PipelineBehaviorAsync<TRequest, TResponse>
    where TRequest : class, IRequest
{
    #region Methods

    /// <summary>
    /// Pipeline handler.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/>.</returns>
    public override Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        if (context.CurrentUserId is null
            && context is ISetterCorrelationContext setter
            && (accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false))
        {
            var userIdentifier = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            setter.SetCurrentUserId(userIdentifier);
        }

        return next();
    }

    #endregion
}