namespace TTSS.Core.Models;

/// <summary>
/// Contract for a request.
/// </summary>
public interface IRequest;

/// <summary>
/// Contract for a request with a response.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequest<out TResponse>;