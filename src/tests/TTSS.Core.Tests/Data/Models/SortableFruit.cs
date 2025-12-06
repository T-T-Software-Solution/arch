namespace TTSS.Core.Data.Models;

public class SortableFruit : IDbModel<string>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}
