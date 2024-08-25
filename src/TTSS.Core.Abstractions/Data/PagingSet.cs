using TTSS.Core.Models;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

/// <summary>
/// Represents a paginated set of items with page metadata.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <remarks>
/// Initialize new instance of <see cref="PagingSet{TEntity}"/>.
/// </remarks>
/// <param name="resultTask">The entities result</param>
/// <param name="pageSize">Configured total entities per page</param>
/// <param name="currentPage">Target page number</param>
/// <param name="totalCount">Function to get total entities</param>
/// <param name="mappingStrategy">Mapping strategy</param>
public sealed class PagingSet<TEntity>(Task<IEnumerable<TEntity>> resultTask, int pageSize, int currentPage, Func<int> totalCount, IMappingStrategy mappingStrategy)
{
    #region Fields

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
    public int CurrentPage => currentPage;

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
    public int TotalCount => _totalCount ??= totalCount();

    #endregion Properties

    #region Methods

    /// <summary>
    /// Get entities.
    /// </summary>
    /// <returns>The entities</returns>
    public Task<IEnumerable<TEntity>> GetDataAsync()
        => resultTask;

    /// <summary>
    /// Get view models.
    /// </summary>
    /// <typeparam name="TViewModel">Target view model type</typeparam>
    /// <returns>The view models</returns>
    public async Task<IEnumerable<TViewModel>> GetDataAsync<TViewModel>()
        where TViewModel : class
    {
        var result = await resultTask;
        return result.Select(it => mappingStrategy.Map<TViewModel>(it!));
    }

    /// <summary>
    /// Start to execute the paging data.
    /// </summary>
    /// <returns>The paging data</returns>
    public async Task<Paging<TEntity>> ExecuteAsync()
    {
        var result = await resultTask;
        return CreateDisplayablePaging(result);
    }

    /// <summary>
    /// Start to execute the paging data with view model.
    /// </summary>
    /// <typeparam name="TViewModel">Target view model type</typeparam>
    /// <returns>The paging data</returns>
    public async Task<Paging<TViewModel>> ExecuteAsync<TViewModel>()
        where TViewModel : class
    {
        var result = await GetDataAsync<TViewModel>();
        return CreateDisplayablePaging(result);
    }

    private Paging<TData> CreateDisplayablePaging<TData>(IEnumerable<TData> result)
    {
        var itemNumber = currentPage * pageSize;
        var contents = result.ToList();

        if (typeof(TData).IsAssignableTo(typeof(IHaveOrderNumber)))
        {
            var orderableQry = contents
                .Where(it => it is IHaveOrderNumber)
                .Cast<IHaveOrderNumber>();
            foreach (var item in orderableQry)
            {
                item.OrderNo = ++itemNumber;
            }
        }

        const int Offset = 1;
        var pageCount = PageCount == default ? Offset : PageCount;
        var nextPage = NextPage == default ? default : NextPage + Offset;
        var previousPage = PreviousPage == default && CurrentPage != Offset ? default : PreviousPage + Offset;
        return new()
        {
            CurrentPage = CurrentPage + Offset,
            PageSize = pageSize,
            TotalCount = TotalCount,
            PageCount = pageCount,
            NextPage = nextPage,
            PreviousPage = previousPage,
            HasNextPage = HasNextPage,
            HasPreviousPage = HasPreviousPage,
            Contents = contents ?? [],
        };
    }

    private void ComputeParameters()
    {
        if (_hasComputeParameters) return;
        _hasComputeParameters = true;

        const int PageOffset = 1;
        const int MinimumPage = 0;
        var totalCount = TotalCount;
        _pageCount = pageSize <= MinimumPage ? MinimumPage : totalCount / pageSize + (totalCount % pageSize > MinimumPage ? PageOffset : MinimumPage);
        var lastPage = Math.Max(MinimumPage, _pageCount - PageOffset);
        _hasNextPage = CurrentPage < _pageCount - PageOffset;
        _nextPage = HasNextPage ? Math.Min(lastPage, Math.Max(-PageOffset, CurrentPage) + PageOffset) : lastPage;
        _hasPreviousPage = CurrentPage > MinimumPage;
        _previousPage = HasPreviousPage ? Math.Max(MinimumPage, Math.Min(lastPage + PageOffset, CurrentPage) - PageOffset) : MinimumPage;
    }

    #endregion
}