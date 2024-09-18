using TTSS.Infra.Data.Sql.Models;

namespace Sample16.RemoteRequest.Shared.Entities;

public class Dog : SqlModelBase
{
    public int Age { get; set; }
    public required string Name { get; set; }
}