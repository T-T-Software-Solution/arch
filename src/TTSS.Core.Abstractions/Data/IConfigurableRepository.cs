namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with configuration.
/// </summary>
public interface IConfigurableRepository<TEntity>
{
    /// <summary>
    /// Configure the repository.
    /// </summary>
    /// <param name="collection">The collection</param>
    /// <returns>The repository</returns>
    void Configure(Func<IQueryable<TEntity>, IQueryable<TEntity>> collection);
}