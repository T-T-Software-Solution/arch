using TTSS.Core.Models;

namespace Sample06.Basic.ConsoleApp.CustomCorrelationContext.Models;

public sealed class DemoContext : CorrelationContext
{
    public string? Name { get; set; }
    public int Score { get; set; }
    public string? Value { get; set; }
    public Dog? MyDog { get; set; }
}