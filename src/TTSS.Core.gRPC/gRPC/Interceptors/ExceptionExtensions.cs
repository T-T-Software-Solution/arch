using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TTSS.Core.Loggings;

namespace TTSS.Core.gRPC.Interceptors;

internal static class ExceptionExtensions
{
    private const string RequestBody = nameof(RequestBody);
    private const string CorrelationId = nameof(CorrelationId);

    public static RpcException Handle<TRequest>(this Exception exception, GrpcExceptionParameter<TRequest> param) where TRequest : class
        => exception switch
        {
            RpcException => HandleRpcException((RpcException)exception, param),
            TimeoutException => CreateRpcException(param, StatusCodes.Status408RequestTimeout, StatusCode.Internal, exception, "An external resource did not answer within the time limit"),
            _ => CreateRpcException(param, StatusCodes.Status500InternalServerError, StatusCode.Internal, exception),
        };

    private static RpcException HandleRpcException<TRequest>(RpcException exception, GrpcExceptionParameter<TRequest> param) where TRequest : class
    {
        var metadata = new Metadata { { CorrelationId, param.CorrelationId } };
        return CreateRpcException(param, StatusCodes.Status500InternalServerError, exception.StatusCode, exception, metadata);
    }

    private static RpcException CreateRpcException<TRequest>(GrpcExceptionParameter<TRequest> param, int httpStatusCode, StatusCode status, Exception exception) where TRequest : class
        => CreateRpcException(param, httpStatusCode, status, exception, exception.Message);
    private static RpcException CreateRpcException<TRequest>(GrpcExceptionParameter<TRequest> param, int httpStatusCode, StatusCode status, Exception exception, string customErrorMsg) where TRequest : class
        => CreateAndWriteLogRpcException(param, httpStatusCode, status, exception, customErrorMsg, new Metadata { { CorrelationId, param.CorrelationId } });
    private static RpcException CreateRpcException<TRequest>(GrpcExceptionParameter<TRequest> param, int httpStatusCode, StatusCode status, Exception exception, Metadata metadata) where TRequest : class
        => CreateAndWriteLogRpcException(param, httpStatusCode, status, exception, exception.Message, metadata);
    private static RpcException CreateAndWriteLogRpcException<TRequest>(GrpcExceptionParameter<TRequest> param, int httpStatusCode, StatusCode status, Exception exception, string errorMsg, Metadata metadata) where TRequest : class
    {
        param.Activity?.LogError(exception, errorMsg);
        param.Activity?.AddTag(RequestBody, param.RequestJson);
        param.Activity?.AddTag(nameof(Metadata), JsonSerializer.Serialize(metadata));
        var httpContext = param.Context.GetHttpContext();
        httpContext.Response.StatusCode = httpStatusCode;
        return new RpcException(new Status(status, errorMsg), metadata);
    }
}