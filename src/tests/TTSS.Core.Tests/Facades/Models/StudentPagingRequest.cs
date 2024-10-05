using TTSS.Core.Models;

namespace TTSS.Core.Facades.Models;

internal class StudentPagingRequest : IPagingRequest
{
    public int PageNo { get; init; }
    public int PageSize { get; init; }
    public IEnumerable<string> Sort { get; init; }
    public Dictionary<string, string> Filter { get; init; }
}