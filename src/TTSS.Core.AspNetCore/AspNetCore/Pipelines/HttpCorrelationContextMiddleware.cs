
using TTSS.Core.Models;

namespace TTSS.Core.AspNetCore.Pipelines;

/// <summary>
/// Middleware for correlation context.
/// </summary>
/// <param name="context">The correlation context</param>
/// <param name="accessor">The HTTP context accessor</param>
public class HttpCorrelationContextMiddleware(ICorrelationContext context, IHttpContextAccessor accessor) : IMiddleware
{
    #region Methods

    /// <summary>
    /// Try to set the current user ID and correlation ID.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (context is not ISetterCorrelationContext setter)
        {
            return next(httpContext);
        }

        if (context.CorrelationId is null)
        {
            setter.SetCorrelationId(Guid.NewGuid().ToString());
        }

        if (context.CurrentUserId is null
            && (accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false))
        {
            setter.SetCurrentUserId(accessor.HttpContext.User.Identity?.Name);
        }

        return next(httpContext);
    }

    #endregion
}