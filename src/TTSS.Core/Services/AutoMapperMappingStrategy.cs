namespace TTSS.Core.Services;

/// <summary>
/// AutoMapper mapping strategy.
/// </summary>
/// <param name="mapper">AutoMapper instance</param>
public sealed class AutoMapperMappingStrategy(AutoMapper.IMapper mapper) : IMappingStrategy
{
    TDestination IMappingStrategy.Map<TSource, TDestination>(TSource source)
        => mapper.Map<TSource, TDestination>(source);
}