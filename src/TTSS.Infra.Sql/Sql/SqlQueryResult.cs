using Microsoft.EntityFrameworkCore;
using System.Collections;
using TTSS.Core.Data;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql;

internal sealed class SqlQueryResult<TEntity>(IQueryable<TEntity> findResult, IMappingStrategy mappingStrategy, CancellationToken cancellationToken) : IQueryResult<TEntity>
{
    #region Fields

    private readonly IQueryable<TEntity> _findResult = findResult ?? throw new ArgumentNullException(nameof(findResult));

    #endregion

    #region Properties

    public long TotalCount => _findResult.LongCount();

    #endregion

    #region Methods

    public async Task<IEnumerable<TEntity>> GetAsync()
        => await _findResult.ToListAsync(cancellationToken);

    public IPagingRepositoryResult<TEntity> ToPaging(bool totalCount = false, int pageSize = 0)
        => new SqlPagingResult<TEntity>(_findResult, mappingStrategy, totalCount, pageSize);

    public IEnumerator<TEntity> GetEnumerator()
        => _findResult.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _findResult.GetEnumerator();

    #endregion
}