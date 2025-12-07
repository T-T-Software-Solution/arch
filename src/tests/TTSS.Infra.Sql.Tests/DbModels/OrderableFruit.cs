using TTSS.Core.Models;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.DbModels;

internal class OrderableFruit : SqlModelBase, IHaveOrderNumber
{
    public int Price { get; set; }
    public string Name { get; set; }
    public int OrderNo { get; set; }
}
