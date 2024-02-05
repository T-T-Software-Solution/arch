using Grpc.Core;
using System.Text.Json;
using TTSS.Core.Loggings;

namespace TTSS.Core.gRPC.Interceptors;

internal sealed class GrpcExceptionParameter<TRequest>(TRequest request, ServerCallContext context, IActivity? activity)
    where TRequest : class
{
    #region Fields

    private string? _requestJson;

    #endregion

    #region Properties

    public string RequestJson => _requestJson ??= JsonSerializer.Serialize(Request);
    public TRequest Request { get; } = request ?? throw new ArgumentNullException(nameof(request));
    public IActivity? Activity { get; } = activity ?? throw new ArgumentNullException(nameof(activity));
    public string CorrelationId { get; } = activity?.RootId ?? activity?.ParentId ?? activity?.CurrentId ?? "Unknown";
    public ServerCallContext Context { get; } = context ?? throw new ArgumentNullException(nameof(context));

    #endregion
}