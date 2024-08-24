namespace TTSS.Core.Services;

/// <summary>
/// AutoMapper mapping strategy.
/// </summary>
/// <param name="mapper">AutoMapper instance</param>
public sealed class AutoMapperMappingStrategy(AutoMapper.IMapper mapper) : IMappingStrategy
{
    TDestination IMappingStrategy.Map<TDestination>(object source)
        => mapper.Map<TDestination>(source);
}