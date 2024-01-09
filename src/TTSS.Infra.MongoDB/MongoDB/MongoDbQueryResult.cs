using MongoDB.Driver;
using System.Collections;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.MongoDB;

internal sealed class MongoDbQueryResult<T> : IQueryResult<T>
{
    #region Fields

    private readonly IFindFluent<T, T> _findResult;
    private readonly CancellationToken _cancellationToken;

    #endregion

    #region Properties

    public long TotalCount => _findResult.CountDocuments(_cancellationToken);

    #endregion

    #region Constructors

    public MongoDbQueryResult(IFindFluent<T, T> findResult, CancellationToken cancellationToken)
    {
        _findResult = findResult ?? throw new ArgumentNullException(nameof(findResult));
        _cancellationToken = cancellationToken;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<T>> GetAsync()
        => await _findResult.ToListAsync(_cancellationToken);

    public IPagingRepositoryResult<T> ToPaging(bool totalCount = false, int pageSize = 0)
        => new MongoDbPagingResult<T>(_findResult, _cancellationToken, totalCount, pageSize);

    public IEnumerator<T> GetEnumerator()
        => _findResult.ToEnumerable(_cancellationToken).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _findResult.ToEnumerable(_cancellationToken).GetEnumerator();

    #endregion
}