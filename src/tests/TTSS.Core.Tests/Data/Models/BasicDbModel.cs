namespace TTSS.Core.Data.Models;

public class BasicDbModel : IDbModel<string>
{
    public string Id { get; set; }
    public string Name { get; set; }
}
