namespace TTSS.Core.Models;

/// <summary>
/// Contract for a remote request message.
/// </summary>
public interface IRemoteRequest : IRequest;

/// <summary>
/// Contract for a remote request message with a response.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRemoteRequest<out TResponse> : IRequest<TResponse> where TResponse : class;