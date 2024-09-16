namespace Sample07.Basic.ConsoleApp.ModelMapping.Models;

public sealed record Easy
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime BirthDate { get; set; }
}