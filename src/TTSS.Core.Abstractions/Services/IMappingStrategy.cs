namespace TTSS.Core.Services;

/// <summary>
/// Contract for mapping strategy.
/// </summary>
public interface IMappingStrategy
{
    /// <summary>
    /// Map source to destination.
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <typeparam name="TDestination">Destination type</typeparam>
    /// <param name="source">The source</param>
    /// <returns>The destination</returns>
    TDestination Map<TSource, TDestination>(TSource source);
}
