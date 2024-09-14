namespace TTSS.Core.Services;

/// <summary>
/// Helper extensions for <see cref="System.Uri"/>.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Gets the base URI of the <see cref="System.Uri"/>.
    /// </summary>
    /// <param name="uri">The URI</param>
    /// <returns>Base URI with tailing slash</returns>
    public static string GetBaseUri(this Uri uri)
    {
        var builder = new UriBuilder(uri.Scheme, uri.Host)
        {
            Port = uri.IsDefaultPort ? -1 : uri.Port
        };
        return builder.Uri.ToString();
    }
}