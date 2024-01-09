using Microsoft.EntityFrameworkCore;
using System.Collections;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

internal sealed class SqlQueryResult<TEntity> : IQueryResult<TEntity>
{
    #region Fields

    private readonly IQueryable<TEntity> _findResult;
    private readonly CancellationToken _cancellationToken;

    #endregion

    #region Properties

    public long TotalCount => _findResult.LongCount();

    #endregion

    #region Constructors

    public SqlQueryResult(IQueryable<TEntity> findResult, CancellationToken cancellationToken)
    {
        _findResult = findResult ?? throw new ArgumentNullException(nameof(findResult));
        _cancellationToken = cancellationToken;
    }

    #endregion

    #region Methods

    public async Task<IEnumerable<TEntity>> GetAsync()
        => await _findResult.ToListAsync(_cancellationToken);

    public IPagingRepositoryResult<TEntity> ToPaging(bool totalCount = false, int pageSize = 0)
        => new SqlPagingResult<TEntity>(_findResult, totalCount, pageSize);

    public IEnumerator<TEntity> GetEnumerator()
        => _findResult.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _findResult.GetEnumerator();

    #endregion
}