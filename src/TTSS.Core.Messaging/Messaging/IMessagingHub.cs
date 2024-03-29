﻿namespace TTSS.Core.Messaging;

/// <summary>
/// Defines a messaging hub to encapsulate request/response and publishing interaction patterns.
/// </summary>
public interface IMessagingHub
{
    /// <summary>
    /// Asynchronously send a notification to multiple handlers.
    /// </summary>
    /// <param name="publication">Notification object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    Task PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken = default) where TPublication : IPublication;

    /// <summary>
    /// Asynchronously send an object request to a single handler.
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the type erased handler response</returns>
    Task SendAsync(IRequesting request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously send a request to a single handler with response.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResponse> SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken = default);
}