using System.Net;
using TTSS.Core.Messaging.Pipelines;
using TTSS.Core.Models;
using HttpResponse = TTSS.Core.Models.HttpResponse;

namespace TTSS.Core.Web.Pipelines;

/// <summary>
/// Validates paging arguments.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <param name="serviceProvider">Service provider</param>
public sealed class HttpPagingPipelineValidator<TRequest, TResponse>(IServiceProvider serviceProvider)
    : PipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest, IPagingRequest
    where TResponse : class, IHttpResponse
{
    #region Methods

    /// <summary>
    /// Pipeline handler.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable the next action in the pipeline</param>
    /// <returns>The result</returns>
    public override TResponse Handle(TRequest request, Func<TResponse> next)
    {
        var areArgumentsValid = request is not null
            && request.PageNo > 0
            && request.PageSize > 0;
        if (false == areArgumentsValid)
        {
            var type = typeof(TResponse);
            var responseType = type.IsGenericType
                ? typeof(HttpResponse<>).MakeGenericType(type.GenericTypeArguments)
            : typeof(HttpResponse);

            var response = (TResponse)ActivatorUtilities.CreateInstance(
                serviceProvider,
                responseType,
                HttpStatusCode.BadRequest, "Invalid paging arguments.");
            return response;
        }

        return next();
    }

    #endregion
}