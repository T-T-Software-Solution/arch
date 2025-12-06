using TTSS.Core.Models;

namespace TTSS.Core.Data.Models;

public class OrderableDbModel : IDbModel<int>, IHaveOrderNumber
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrderNo { get; set; }
}
