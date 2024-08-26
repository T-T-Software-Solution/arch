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
        var contents = result.ToList();

        if (typeof(TData).IsAssignableTo(typeof(IHaveOrderNumber)))
        {
            var itemNumber = currentPage * pageSize;
            var orderableQry = contents
                .Where(it => it is IHaveOrderNumber)
                .Cast<IHaveOrderNumber>();
            foreach (var item in orderableQry)
            {
                item.OrderNo = ++itemNumber;
            }
        }

        return new()
        {
            CurrentPage = CurrentPage,
            PageSize = pageSize,
            TotalCount = TotalCount,
            PageCount = PageCount,
            NextPage = NextPage,
            PreviousPage = PreviousPage,
            HasNextPage = HasNextPage,
            HasPreviousPage = HasPreviousPage,
            Contents = contents ?? [],
        };
    }

    private void ComputeParameters()
    {
        if (_hasComputeParameters)
        {
            return;
        }
        _hasComputeParameters = true;

        var totalCount = TotalCount;
        var currentPage = CurrentPage > 0 ? CurrentPage : 1;
        _pageCount = totalCount > pageSize ? ((int)Math.Ceiling((double)totalCount / pageSize)) : 1;
        _hasNextPage = _pageCount > currentPage;
        _nextPage = _hasNextPage ? currentPage + 1 : _pageCount;
        _hasPreviousPage = currentPage > 1;
        _previousPage = _hasPreviousPage ? (currentPage - 1 > NextPage ? NextPage : currentPage - 1) : 1;
    }

    #endregion
}