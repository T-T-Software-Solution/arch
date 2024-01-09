using Microsoft.AspNetCore.Mvc;
using System.Net;
using TTSS.Core.Models;

namespace TTSS.Core.AspNetCore.Controllers;

/// <summary>
/// Helper extension methods for converting <see cref="IResponse"/> to <see cref="IActionResult"/>.
/// </summary>
public static class IResponseExtensions
{
    /// <summary>
    /// Converts a <see cref="IResponse"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="response">The response to convert</param>
    /// <returns>The converted response</returns>
    public static async Task<IActionResult> ToActionResultAsync(this Task<IResponse> response)
        => ToActionResult(await response);

    /// <summary>
    /// Converts a <see cref="IResponse{TData}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the response data</typeparam>
    /// <param name="response">The response to convert</param>
    /// <returns>The converted response</returns>
    public static async Task<IActionResult> ToActionResultAsync<TData>(this Task<IResponse<TData>> response)
        => ToActionResult(await response);

    /// <summary>
    /// Converts a <see cref="IResponse"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="response">The response to convert</param>
    /// <returns>The converted response</returns>
    public static IActionResult ToActionResult(this IResponse response)
    {
        if (response is IHttpResponse httpResponse) return CreateActionResult(httpResponse);
        else return new ObjectResult(response);
    }

    /// <summary>
    /// Converts a <see cref="IResponse{TData}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the response data</typeparam>
    /// <param name="response">The response to convert</param>
    /// <returns>The converted response</returns>
    public static IActionResult ToActionResult<TData>(this IResponse<TData> response)
    {
        if (response is IHttpResponse httpResponse) return CreateActionResult(httpResponse, response.Data);
        else return new ObjectResult(response);
    }

    private static IActionResult CreateActionResult(IHttpResponse response, object? content = null)
        => response.StatusCode switch
        {
            HttpStatusCode.OK => content is null ? new OkResult() : new OkObjectResult(content),
            HttpStatusCode.Created => new OkObjectResult(content) { StatusCode = response.StatusCodeNumber },
            HttpStatusCode.Accepted => new AcceptedResult(),
            HttpStatusCode.NoContent => new NoContentResult(),
            _ => new ObjectResult(response) { StatusCode = response.StatusCodeNumber },
        };
}