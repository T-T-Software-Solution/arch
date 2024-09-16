namespace Sample05.Basic.ConsoleApp.WorkWithCorrelation.Models;

public sealed record Animal
{
    public int Weight { get; init; }
    public required string Name { get; init; }
}