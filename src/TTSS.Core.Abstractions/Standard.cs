using System.Text.Json;
using System.Text.Json.Serialization;

namespace TTSS.Core;

/// <summary>
/// All configurations from the conventions.
/// </summary>
public static class Standard
{
    /// <summary>
    /// JSON standard.
    /// </summary>
    public static class Json
    {
        #region Properties

        /// <summary>
        /// Default JSON serializer options.
        /// </summary>
        public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        /// <summary>
        /// Convert object to JSON string.
        /// </summary>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="value">Value</param>
        /// <param name="options">Serializer options</param>
        /// <returns>JSON string</returns>
        public static string Serialize<TValue>(TValue value, JsonSerializerOptions? options = default)
            => JsonSerializer.Serialize(value, options ?? DefaultSerializerOptions);

        /// <summary>
        /// Convert JSON string to object.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response</typeparam>
        /// <param name="json">JSON string</param>
        /// <param name="options">Serializer options</param>
        /// <returns>The result</returns>
        public static TResponse? Deserialize<TResponse>(string json, JsonSerializerOptions? options = default)
            => JsonSerializer.Deserialize<TResponse>(json, options ?? DefaultSerializerOptions);

        #endregion
    }
}