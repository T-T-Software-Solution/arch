using MongoDB.Driver;
using System.Collections;
using TTSS.Core.Data;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.MongoDB;

internal sealed class MongoDbQueryResult<TEntity>(IFindFluent<TEntity, TEntity> findResult, IMappingStrategy mappingStrategy, CancellationToken cancellationToken) : IQueryResult<TEntity>
{
    #region Fields

    private readonly IFindFluent<TEntity, TEntity> _findResult = findResult ?? throw new ArgumentNullException(nameof(findResult));
    private readonly CancellationToken _cancellationToken = cancellationToken;

    #endregion

    #region Properties

    public long TotalCount => _findResult.CountDocuments(_cancellationToken);

    #endregion

    #region Methods

    public async Task<IEnumerable<TEntity>> GetAsync()
        => await _findResult.ToListAsync(_cancellationToken);

    public IPagingRepository<TEntity> ToPaging(bool totalCount = false, int pageSize = 0)
        => new MongoDbPagingRepository<TEntity>(_findResult, mappingStrategy, _cancellationToken, totalCount, pageSize);

    public IEnumerator<TEntity> GetEnumerator()
        => _findResult.ToEnumerable(_cancellationToken).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _findResult.ToEnumerable(_cancellationToken).GetEnumerator();

    #endregion
}