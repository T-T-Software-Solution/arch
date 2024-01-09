namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with bulk insert operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IInsertBulkRepository<TEntity> : IRepositoryBase
{
    /// <summary>
    /// Insert entities in bulk operation.
    /// </summary>
    /// <param name="entities">Entities to insert bulk</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task InsertBulkAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}