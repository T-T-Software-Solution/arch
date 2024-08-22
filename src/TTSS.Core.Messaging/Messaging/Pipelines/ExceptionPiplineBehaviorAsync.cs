using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;
using TTSS.Core.Annotations;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Pipelines;

/// <summary>
/// Defines a pipeline behavior for handling exceptions.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <param name="serviceProvider">Service provider</param>
public sealed class ExceptionPiplineBehaviorAsync<TRequest, TResponse>(IServiceProvider serviceProvider)
    : PipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest
    where TResponse : class, IHttpResponse
{
    #region Methods

    /// <summary>
    /// Handles the exception.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable the next action in the pipeline</param>
    /// <returns>The result</returns>
    public override TResponse Handle(TRequest request, Func<TResponse> next)
    {
        try
        {
            return next();
        }
        catch (Exception error)
        {
            // TODO: Logging

            var type = typeof(TResponse);
            var responseType = type.IsGenericType
                ? typeof(HttpResponse<>).MakeGenericType(type.GenericTypeArguments)
                : typeof(HttpResponse);

            var description = typeof(TRequest).GetCustomAttribute<OperationDescriptionAttribute>();
            // TODO: AppText
            var errorMessage = description is not null ? $"{description.Operation} {description.ModuleName} failed" : error.Message;

            var response = (TResponse)ActivatorUtilities.CreateInstance(
                serviceProvider,
                responseType,
                HttpStatusCode.InternalServerError, errorMessage);

            return response;
        }
    }

    #endregion
}