namespace TTSS.Core.Messaging;

/// <summary>
/// Allows for generic type constraints of objects implementing <see cref="Pipelines.PipelineBehavior{TRequest, TResponse}"/> or <see cref="Pipelines.PipelineBehaviorAsync{TRequest, TResponse}"/> base interfaces.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IPipelineBehaviorBase<in TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull;