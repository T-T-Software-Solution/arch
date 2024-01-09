namespace TTSS.Core.Data;

/// <summary>
/// Paging result manager.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public sealed class PagingResult<TEntity>
{
    #region Fields

    private readonly int _pageSize;
    private readonly Func<int> _totalCountFn;
    private readonly Task<IEnumerable<TEntity>> _result;
    private int? _totalCount;
    private int _pageCount;
    private int _nextPage;
    private int _previousPage;
    private bool _hasNextPage;
    private bool _hasPreviousPage;
    private bool _hasComputeParameters;

    #endregion

    #region Properties

    /// <summary>
    /// Current page number.
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// Total entities on the current page.
    /// </summary>
    public int PageCount { get { ComputeParameters(); return _pageCount; } }

    /// <summary>
    /// Next page number.
    /// </summary>
    public int NextPage { get { ComputeParameters(); return _nextPage; } }

    /// <summary>
    /// Previous page number.
    /// </summary>
    public int PreviousPage { get { ComputeParameters(); return _previousPage; } }

    /// <summary>
    /// Can go to next page or not.
    /// </summary>
    public bool HasNextPage { get { ComputeParameters(); return _hasNextPage; } }

    /// <summary>
    /// Can go to previous page or not.
    /// </summary>
    public bool HasPreviousPage { get { ComputeParameters(); return _hasPreviousPage; } }

    /// <summary>
    /// Total entities.
    /// </summary>
    public int TotalCount => _totalCount ??= _totalCountFn();

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initialize new instance of <see cref="PagingResult{TEntity}"/>.
    /// </summary>
    /// <param name="result">The entities result</param>
    /// <param name="pageSize">Configured total entities per page</param>
    /// <param name="currentPage">Target page number</param>
    /// <param name="totalCount">Function to get total entities</param>
    public PagingResult(Task<IEnumerable<TEntity>> result,
        int pageSize,
        int currentPage,
        Func<int> totalCount)
    {
        _result = result;
        _pageSize = pageSize;
        CurrentPage = currentPage;
        _totalCountFn = totalCount;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get data.
    /// </summary>
    /// <returns>The entities</returns>
    public Task<IEnumerable<TEntity>> GetDataAsync()
        => _result;

    /// <summary>
    /// Convert to paging data.
    /// </summary>
    /// <returns>The paging data</returns>
    public async Task<Models.PagingData<TEntity>> ToPagingDataAsync()
    {
        var result = await _result;
        return new()
        {
            CurrentPage = CurrentPage,
            PageSize = _pageSize,
            TotalCount = TotalCount,
            PageCount = PageCount,
            NextPage = NextPage,
            PreviousPage = PreviousPage,
            HasNextPage = HasNextPage,
            HasPreviousPage = HasPreviousPage,
            Result = result ?? Enumerable.Empty<TEntity>(),
        };
    }

    private void ComputeParameters()
    {
        if (_hasComputeParameters) return;
        _hasComputeParameters = true;

        const int PageOffset = 1;
        const int MinimumPage = 0;
        var totalCount = TotalCount;
        _pageCount = _pageSize <= MinimumPage ? MinimumPage : totalCount / _pageSize + (totalCount % _pageSize > MinimumPage ? PageOffset : MinimumPage);
        var lastPage = Math.Max(MinimumPage, _pageCount - PageOffset);
        _hasNextPage = CurrentPage < _pageCount - PageOffset;
        _nextPage = HasNextPage ? Math.Min(lastPage, Math.Max(-PageOffset, CurrentPage) + PageOffset) : lastPage;
        _hasPreviousPage = CurrentPage > MinimumPage;
        _previousPage = HasPreviousPage ? Math.Max(MinimumPage, Math.Min(lastPage + PageOffset, CurrentPage) - PageOffset) : MinimumPage;
    }

    #endregion
}