using Grpc.Core;
using Grpc.Core.Interceptors;
using TTSS.Core.Loggings;

namespace TTSS.Core.gRPC.Interceptors;

/// <summary>
/// Interceptor for logging exceptions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GrpcExceptionLogInterceptor"/> class.
/// </remarks>
/// <param name="activityFactory">The activity factory</param>
/// <exception cref="ArgumentNullException">The activity factory is required</exception>
public class GrpcExceptionLogInterceptor(IActivityFactory activityFactory) : Interceptor
{
    #region Fields

    private readonly IActivityFactory _activityFactory = activityFactory ?? throw new ArgumentNullException(nameof(activityFactory));

    #endregion

    /// <summary>
    /// Server-side handler for intercepting and incoming unary call.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method</typeparam>
    /// <typeparam name="TResponse">Response message type for this method</typeparam>
    /// <param name="request">The request value of the incoming invocation</param>
    /// <param name="context">The invocation context</param>
    /// <param name="continuation">The next handler to invoke</param>
    /// <returns>The response</returns>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try { return await continuation(request, context); }
        catch (Exception e) { throw e.Handle<TRequest>(new(request, context, CreateActivity(context.Method))); }
    }

    /// <summary>
    /// Server-side handler for intercepting client streaming call.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method</typeparam>
    /// <typeparam name="TResponse">Response message type for this method</typeparam>
    /// <param name="requestStream">The request stream of the incoming invocation</param>
    /// <param name="context">The invocation context</param>
    /// <param name="continuation">The next handler to invoke</param>
    /// <returns>The response</returns>
    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { return await continuation(requestStream, context); }
        catch (Exception e) { throw e.Handle<TRequest>(new(requestStream.Current, context, CreateActivity(context.Method))); }
    }

    /// <summary>
    /// Server-side handler for intercepting server streaming call.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method</typeparam>
    /// <typeparam name="TResponse">Response message type for this method</typeparam>
    /// <param name="request">The request value of the incoming invocation</param>
    /// <param name="responseStream">The response stream of the incoming invocation</param>
    /// <param name="context">The invocation context</param>
    /// <param name="continuation">The next handler to invoke</param>
    /// <returns>The response</returns>
    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { await continuation(request, responseStream, context); }
        catch (Exception e) { throw e.Handle<TRequest>(new(request, context, CreateActivity(context.Method))); }
    }

    /// <summary>
    /// Server-side handler for intercepting bidirectional streaming calls.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method</typeparam>
    /// <typeparam name="TResponse">Response message type for this method</typeparam>
    /// <param name="requestStream">The request stream of the incoming invocation</param>
    /// <param name="responseStream">The response stream of the incoming invocation</param>
    /// <param name="context">The invocation context</param>
    /// <param name="continuation">The next handler to invoke</param>
    /// <returns>The response</returns>
    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try { await continuation(requestStream, responseStream, context); }
        catch (Exception e) { throw e.Handle<TRequest>(new(requestStream.Current, context, CreateActivity(context.Method))); }
    }

    private IActivity? CreateActivity(string methodPath)
    {
        var categoryName = methodPath.Split("/", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? methodPath;
        var callerMethodName = Path.GetFileName(methodPath) ?? "UnknownMethod";
        return _activityFactory.CreateActivity(categoryName, callerMethodName, System.Diagnostics.ActivityKind.Server);
    }
}