using Grpc.Core;
using System.Text.Json;
using TTSS.Core.Loggings;

namespace TTSS.Core.gRPC.Interceptors;

internal sealed class GrpcExceptionParameter<TRequest> where TRequest : class
{
    #region Fields

    private string? _requestJson;

    #endregion

    #region Properties

    public string RequestJson => _requestJson ??= JsonSerializer.Serialize(Request);
    public TRequest Request { get; }
    public IActivity? Activity { get; }
    public string CorrelationId { get; }
    public ServerCallContext Context { get; }

    #endregion

    #region Constructors

    public GrpcExceptionParameter(TRequest request, ServerCallContext context, IActivity? activity)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Activity = activity ?? throw new ArgumentNullException(nameof(activity));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        CorrelationId = activity?.RootId ?? activity?.ParentId ?? activity?.CurrentId ?? "Unknown";
    }

    #endregion
}