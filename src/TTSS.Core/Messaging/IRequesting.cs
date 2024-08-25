namespace TTSS.Core.Messaging;

/// <summary>
/// Contract for a request message.
/// </summary>
public interface IRequesting : Models.IRequest, MediatR.IRequest;

/// <summary>
/// Contract for a request message with a response.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequesting<out TResponse> : Models.IRequest, MediatR.IRequest<TResponse>;