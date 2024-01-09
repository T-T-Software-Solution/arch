namespace TTSS.Core.Data.Models;

public class CustomPrimaryKeyDbModel : IDbModel<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
}